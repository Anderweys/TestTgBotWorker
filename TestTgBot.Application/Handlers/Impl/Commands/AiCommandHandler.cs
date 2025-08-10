using Telegram.Bot;
using Telegram.Bot.Types;
using TestTgBot.Application.Handlers.Abst.Commands;
using TestTgBot.Application.Models.Domain.Handlers;
using TestTgBot.Application.Models.Enum;
using TestTgBot.Application.Services.Abst.DeepSeek;
using TestTgBot.Application.Services.Abst.Groq;

namespace TestTgBot.Application.Handlers.Impl.Commands;

public sealed class AiCommandHandler : IAiCommandHandler
{
    private readonly IDeepSeekService _deepSeekService;
    private readonly IGroqService _groqService;
    private readonly ITelegramBotClient _bot;

    public AiCommandHandler(IDeepSeekService deepSeekService, IGroqService groqService, ITelegramBotClient bot)
    {
        _deepSeekService = deepSeekService;
        _groqService = groqService;
        _bot=bot;
    }

    public async Task<HandlerUpdateResult> HandleCommand(Update update, BotUpdateCommandType updateType, string text, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;
        var messageId = update.Message.MessageId;
        var isReplyToBot = update.Message.ReplyToMessage?.From?.IsBot == true;

        return updateType switch
        {
            BotUpdateCommandType.DeepSeek => await HandleDeepSeek(chatId, text, cancellationToken),
            BotUpdateCommandType.Groq => await HandleGroq(chatId, text, isReplyToBot, messageId, cancellationToken),
            BotUpdateCommandType.GroqSession => await HandleGroqSession(update, text, messageId, cancellationToken),
            BotUpdateCommandType.GroqCleanup => await HandleGroqCleanup(update, cancellationToken),
            BotUpdateCommandType.GroqSessionCleanup => await HandleGroqSessionCleanup(update, cancellationToken),
            _ => new(chatId, Text: $"Not implemented command: {updateType}"),
        };
    }

    private async Task<HandlerUpdateResult> HandleDeepSeek(long chatId, string text, CancellationToken cancellationToken)
    {
        var deepSeekMessage = await _deepSeekService.SendSingleMessage(text, cancellationToken: cancellationToken);
        return new(chatId, Text: deepSeekMessage)
        {
            IsAiReply = true
        };
    }

    private async Task<HandlerUpdateResult> HandleGroq(long chatId, string text, bool isReplyToBot, int messageId, CancellationToken cancellationToken)
    {
        var response = await _groqService.SendWithSharedHistory(
            chatId: chatId,
            message: text,
            userMessageId: messageId,
            cancellationToken: cancellationToken);

        return new(chatId, Text: response)
        {
            UserId = 0,
            IsAiReply = true
        };
    }

    private async Task<HandlerUpdateResult> HandleGroqSession(Update update, string text, int messageId, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;
        var userId = update.Message.From!.Id;
        var fromMessage = update.Message.ReplyToMessage?.Text;

        if(fromMessage is not null)
        {
            var prefix = "Groq to "; // TODO: remake, maybe in const.
            var userName = fromMessage.StartsWith(prefix) 
                ? fromMessage[prefix.Length..].Split('\n').FirstOrDefault()?.Trim() 
                : null;
            if (userName != update.Message.From.Username)
            {
                var message = _groqService.SendConflictSession(
                    userName: userName!,
                    chatId: chatId,
                    userId: userId,
                    messageId: messageId,
                    cancellationToken: cancellationToken);

                return new(chatId, Text: message)
                {
                    UserId = userId
                };
            }
        }

        var response = await _groqService.SendWithPersonalHistory(
            chatId: chatId,
            userId: userId,
            userLogin: update.Message.From.Username!,
            message: text,
            userMessageId: messageId,
            cancellationToken: cancellationToken);

        return new(chatId, Text: response)
        {
            UserId = userId,
            IsAiReply = true,
            ReplyMessageId = messageId
        };
    }

    private async Task<HandlerUpdateResult> HandleGroqCleanup(Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;
        await _groqService.ClearSharedSession(chatId, cancellationToken);

        await _bot.DeleteMessage(chatId, update.Message.MessageId, cancellationToken);
        return new HandlerUpdateResult(ChatId: null);
    }

    private async Task<HandlerUpdateResult> HandleGroqSessionCleanup(Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;
        var userId = update.Message.From!.Id;
        await _groqService.ClearPersonalSession(chatId, userId, cancellationToken);

        await _bot.DeleteMessage(chatId, update.Message.MessageId, cancellationToken);
        return new HandlerUpdateResult(ChatId: null);
    }
}