using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TestTgBot.Application.Models.Enum;

namespace TestTgBot.Application.Models.Domain.Handlers;

public record HandlerUpdateResult
(
    long? ChatId = null,
    long? UserId = null,
    string? Text = null,
    InputFile? File = null,
    bool HasSpoiler = false,
    string? Caption = null,
    ParseMode ParseMode = ParseMode.None,
    ChatChannelType ChannelType = ChatChannelType.Message,
    bool IsAiReply = false,
    int? ReplyMessageId = null
);