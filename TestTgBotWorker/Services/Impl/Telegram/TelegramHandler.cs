using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Options;
using TestTgBotWorker.Extensions;
using TestTgBotWorker.Models.Enum;
using TestTgBotWorker.Configuration;
using TestTgBotWorker.Services.Abst.Telegram;
using TestTgBotWorker.Services.Abst;
using TestTgBotWorker.Services.Abst.DeepSeek;
using TestTgBotWorker.Services.Abst.Groq;

namespace TestTgBotWorker.Services.Impl.Telegram;

public sealed class TelegramHandler : ITelegramHandler
{
    private int _offset = 0;
    private readonly BotOptions _options;
    private readonly TelegramBotClient _bot;
    private readonly IWeatherClient _weatherClient;
    private readonly IGroqService _groqService;
    private readonly IDeepSeekService _deepSeekService;
    private readonly ITelegramDebounceCommandService _debounceService;

    public TelegramHandler(TelegramBotClient bot, ITelegramDebounceCommandService debounceService, IOptions<BotOptions> options, IWeatherClient weatherClient, IGroqService groqService, IDeepSeekService deepSeekService)
    {
        _bot=bot;
        _debounceService=debounceService;
        _options=options.Value;
        _weatherClient=weatherClient;
        _groqService=groqService;
        _deepSeekService=deepSeekService;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await BackgroundHandling(cancellationToken);
            }
            catch
            {
                continue;
            }
        }
    }

    private async Task BackgroundHandling(CancellationToken cancellationToken)
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
                await HandleUpdate(update, cancellationToken);
            }
            catch (Exception ex)
            {
                _offset = update.Id + 1;
                if (update.Message is not null)
                {
                    await _bot.SendMessage(update.Message.Chat.Id, $"Error: {ex.Message}, command: {update.Message.Text ?? ""}", cancellationToken: cancellationToken);
                }
            }
        }
    }

    private async Task HandleUpdate(Update update, CancellationToken cancellationToken)
    {
        var validationResult = ValidateUpdate(update);
        if (!validationResult.HasValue)
        {
            _offset = update.Id + 1;
            return;
        }

        var command = validationResult.Value.UpdateType.GetCommandType();
        switch (command)
        {
            case BotUpdateType.Operation:
                await HandleOperationCommand(update, validationResult.Value.UpdateType, validationResult.Value.Text, cancellationToken);
                break;

            case BotUpdateType.User:
                await HandleUserCommand(update, validationResult.Value.UpdateType, validationResult.Value.Text, cancellationToken);
                break;

            case BotUpdateType.Ai:
                await HandleAiCommand(update, validationResult.Value.UpdateType, validationResult.Value.Text, cancellationToken);
                break;

            default:
                await _bot.SendMessage(update.Message!.Chat.Id, "Not implemented command", cancellationToken: cancellationToken);
                break;
        }
        _offset = update.Id + 1;
    }

    private (string Text, BotUpdateCommandType UpdateType)? ValidateUpdate(Update update)
    {
        if (update.Message is null 
            || update.Message.Text is not null 
            && !update.Message.Text.StartsWith('/'))
        {
            return null;
        }

        var fullText = update.Message?.Text?.Trim();
        var firstSpaceIndex = fullText?.IndexOf(' ') ?? -1;
        var commandText = firstSpaceIndex > 0 ? fullText?[..firstSpaceIndex] : fullText;
        var messageText = firstSpaceIndex > 0 && firstSpaceIndex < fullText?.Length - 1
            ? fullText?[(firstSpaceIndex + 1)..]
            : string.Empty;

        var updateType = BotExtensions.GetCommandByText(commandText);
        if (!updateType.HasValue)
        {
            return null;
        }

        var userId = update.Message!.From?.Id.ToString();
        var debounceKey = $"{userId}:{commandText}";
        if (!_debounceService.IsCommandAllowed(debounceKey))
        {
            return null;
        }

        return (messageText, updateType.Value)!;
    }

    private async Task HandleOperationCommand(Update update, BotUpdateCommandType updateType, string text, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;

        switch (updateType)
        {
            case BotUpdateCommandType.Ping:
                await _bot.SendMessage(chatId, "pong", cancellationToken: cancellationToken);
                break;

            case BotUpdateCommandType.Weather:
                var response = await _weatherClient.GetCurrentWeather(cancellationToken);
                if(response == null)
                {
                    await _bot.SendMessage(chatId, "Cound't got weather.", cancellationToken: cancellationToken);
                }
                var message = _weatherClient.ConvertToPrettyMessage(response!.CurrentWeather);
                await _bot.SendMessage(chatId, message, cancellationToken: cancellationToken);
                break;

            default:
                await _bot.SendMessage(update.Message!.Chat.Id, "Not implemented command", cancellationToken: cancellationToken);
                break;
        }
    }

    private async Task HandleUserCommand(Update update, BotUpdateCommandType updateType, string text, CancellationToken cancellationToken)
    {
        await _bot.SendMessage(
            update.Message!.Chat.Id,
            "Temporary not working...",
            cancellationToken: cancellationToken);
    }

    private async Task HandleAiCommand(Update update, BotUpdateCommandType updateType, string text, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;

        switch (updateType)
        {
            case BotUpdateCommandType.DeepSeek:
                var deepSeekMessage = await _deepSeekService.SendSingleMessage(text, cancellationToken: cancellationToken);
                await _bot.SendMessage(chatId, deepSeekMessage, cancellationToken: cancellationToken);
                break;

            case BotUpdateCommandType.Groq:
                var groqMessage = await _groqService.SendSingleMessage(text, cancellationToken);
                await _bot.SendMessage(chatId, groqMessage, cancellationToken: cancellationToken);
                break;

            default:
                await _bot.SendMessage(update.Message!.Chat.Id, "Not implemented command", cancellationToken: cancellationToken);
                break;
        }
    }
}