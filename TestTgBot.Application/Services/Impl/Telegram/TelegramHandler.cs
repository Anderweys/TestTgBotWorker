using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TestTgBot.Application.Configuration;
using TestTgBot.Application.Extensions;
using TestTgBot.Application.Handlers.Abst.Commands;
using TestTgBot.Application.Models.Domain;
using TestTgBot.Application.Models.Domain.Handlers;
using TestTgBot.Application.Models.Enum;
using TestTgBot.Application.Services.Abst.Telegram;

namespace TestTgBot.Application.Services.Impl.Telegram;

public sealed class TelegramHandler : ITelegramHandler
{
    private int _offset = 0;
    private bool _isInitialized = false;

    private readonly BotOptions _options;
    private readonly ITelegramBotClient _bot;
    private readonly ITelegramDebounceCommandService _debounceService;
    private readonly ITelegramSessionStorage _storage;
    private readonly IServiceProvider _serviceProvider;

    public TelegramHandler(ITelegramBotClient bot, ITelegramDebounceCommandService debounceService, IOptions<BotOptions> options, IServiceProvider serviceProvider, ITelegramSessionStorage storage)
    {
        _bot=bot;
        _debounceService=debounceService;
        _options=options.Value;
        _serviceProvider=serviceProvider;
        _storage=storage;
    }

    public async Task ProcessBatch(CancellationToken cancellationToken)
    {
        if (!_isInitialized)
        {
            await _bot.DeleteWebhook(dropPendingUpdates: true, cancellationToken: cancellationToken);
            _isInitialized = true;
        }

        using var scope = _serviceProvider.CreateScope();
        var commandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler>();

        await BackgroundHandling(commandHandler, cancellationToken);
    }

    private async Task BackgroundHandling(ICommandHandler commandHandler, CancellationToken cancellationToken)
    {
        var updates = await _bot.GetUpdates(
            offset: _offset,
            timeout: _options.Timeout,
            limit: _options.Limit,
            allowedUpdates: [UpdateType.Message],
            cancellationToken: cancellationToken
        );

        foreach (var update in updates)
        {
            try
            {
                await HandleUpdate(commandHandler, update, cancellationToken);
            }
            catch (Exception ex)
            {
                if (update.Message is not null)
                {
                    await _bot.SendMessage(update.Message.Chat.Id, $"Error: {ex.Message}, command: {update.Message.Text ?? ""}", cancellationToken: cancellationToken);
                }
            }
            _offset = update.Id + 1;
        }
    }
    private async Task HandleUpdate(ICommandHandler commandHandler, Update update, CancellationToken cancellationToken)
    {
        var validationResult = ValidateUpdate(update);
        if (validationResult is null)
        {
            return;
        }

        var message = await commandHandler.HandleCommand(update, validationResult.UpdateType, validationResult.Text, cancellationToken);

        await SendByChannelType(message, validationResult.UpdateType, update.Message?.MessageId, cancellationToken);
    }
    private ValidateUpdateResult? ValidateUpdate(Update update)
    {
        if (update.Message is null)
        {
            return null;
        }

        var fullText = update.Message.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(fullText) && fullText.StartsWith('/'))
        {
            var firstSpaceIndex = fullText.IndexOf(' ');
            var commandText = firstSpaceIndex > 0 ? fullText[..firstSpaceIndex] : fullText;
            var messageText = firstSpaceIndex > 0 && firstSpaceIndex < fullText.Length - 1
                ? fullText[(firstSpaceIndex + 1)..]
                : string.Empty;

            var updateType = BotExtensions.GetCommandByText(commandText);
            if (!updateType.HasValue)
            {
                return null;
            }

            var userId = update.Message.From?.Id.ToString();
            var debounceKey = $"{userId}:{commandText}";
            if (!_debounceService.IsCommandAllowed(debounceKey))
            {
                CacheMessage(null, updateType.Value, null, update.Message.Id, update.Message.Chat.Id);
                return null;
            }

            return new(messageText, updateType.Value);
        }

        if (update.Message.ReplyToMessage?.From?.IsBot == true
            && update.Message.ReplyToMessage?.Text is not null
            && update.Message.ReplyToMessage.Text.StartsWith("Groq"))
        {
            var text = update.Message.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return new(text, update.Message.ReplyToMessage.Text.StartsWith("Groq to")
                ? BotUpdateCommandType.GroqSession
                : BotUpdateCommandType.Groq);
        }

        return null;
    }

    private async Task SendByChannelType(HandlerUpdateResult updateResult, BotUpdateCommandType updateType, int? messageId, CancellationToken cancellationToken)
    {
        if (updateResult.ChatId is null)
        {
            return;
        }

        Message? message = null;

        switch (updateResult.ChannelType)
        {
            case ChatChannelType.Message:
                message = await _bot.SendMessage(
                    updateResult.ChatId,
                    updateResult.Text!,
                    replyParameters: updateResult.ReplyMessageId,
                    cancellationToken: cancellationToken);
                break;

            case ChatChannelType.Photo:
                message = await _bot.SendPhoto(
                    updateResult.ChatId,
                    photo: updateResult.File!,
                    hasSpoiler: updateResult.HasSpoiler,
                    caption: updateResult.Caption,
                    parseMode: updateResult.ParseMode,
                    cancellationToken: cancellationToken);
                break;

            case ChatChannelType.Gif:
                message = await _bot.SendAnimation(
                    updateResult.ChatId,
                    animation: updateResult.File!,
                    hasSpoiler: updateResult.HasSpoiler,
                    caption: updateResult.Caption,
                    parseMode: updateResult.ParseMode,
                    cancellationToken: cancellationToken);
                break;

            case ChatChannelType.Video:
                message = await _bot.SendVideo(
                    updateResult.ChatId,
                    video: updateResult.File!,
                    hasSpoiler: updateResult.HasSpoiler,
                    caption: updateResult.Caption,
                    parseMode: updateResult.ParseMode,
                    cancellationToken: cancellationToken);
                break;

            case ChatChannelType.Unknown:
                message = await _bot.SendMessage(
                    updateResult.ChatId,
                    "Channel not unknow.",
                    cancellationToken: cancellationToken);
                break;

            default:
                message = await _bot.SendMessage(
                    updateResult.ChatId,
                    "Channel not found.",
                    cancellationToken: cancellationToken);
                break;
        }

        CacheMessage(message, updateType, updateResult, messageId);
    }

    private void CacheMessage(Message? message, BotUpdateCommandType updateType, HandlerUpdateResult? updateResult, int? messageId = null, long? chatId = null)
    {
        if (updateType == BotUpdateCommandType.Groq ||  updateType == BotUpdateCommandType.GroqSession
            && message is not null
            && updateResult is not null
            && updateResult.UserId.HasValue
            && updateResult.IsAiReply)
        {
            _storage.AddMessageId(updateResult!.ChatId!.Value, updateResult.UserId!.Value, message!.MessageId);
        }
        else if (updateType == BotUpdateCommandType.RuleThirtyFour && messageId is not null)
        {
            if (message is not null && updateResult is not null)
            {
                _storage.AddRuleMessage(updateResult.ChatId!.Value, message!.MessageId, DateTime.UtcNow);
                _storage.AddRuleMessage(updateResult.ChatId!.Value, messageId.Value, DateTime.UtcNow);
            }
            else if (message is null && chatId.HasValue)
            {
                _storage.AddRuleMessage(chatId.Value, messageId.Value, DateTime.UtcNow);
            }
        }
    }
}