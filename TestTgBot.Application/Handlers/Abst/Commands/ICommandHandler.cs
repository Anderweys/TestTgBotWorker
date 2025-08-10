using Telegram.Bot.Types;
using TestTgBot.Application.Models.Domain.Handlers;
using TestTgBot.Application.Models.Enum;

namespace TestTgBot.Application.Handlers.Abst.Commands;

public interface ICommandHandler
{
    Task<HandlerUpdateResult> HandleCommand(Update update, BotUpdateCommandType updateType, string text, CancellationToken cancellationToken);
}