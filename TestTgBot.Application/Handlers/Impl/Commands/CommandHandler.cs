using Telegram.Bot.Types;
using TestTgBot.Application.Extensions;
using TestTgBot.Application.Handlers.Abst.Commands;
using TestTgBot.Application.Models.Domain.Handlers;
using TestTgBot.Application.Models.Enum;

namespace TestTgBot.Application.Handlers.Impl.Commands;

public class CommandHandler : ICommandHandler
{
    private readonly IAiCommandHandler _aiHandler;
    private readonly IOperationCommandHandler _operationHandler;

    public CommandHandler(IAiCommandHandler aiHandler, IOperationCommandHandler operationHandler)
    {
        _aiHandler=aiHandler;
        _operationHandler=operationHandler;
    }

    public async Task<HandlerUpdateResult> HandleCommand(Update update, BotUpdateCommandType updateType, string text, CancellationToken cancellationToken)
    {
        var command = updateType.GetCommandType();

        return command switch
        {
            BotUpdateType.Operation => await _operationHandler.HandleCommand(update, updateType, text, cancellationToken),
            BotUpdateType.Ai => await _aiHandler.HandleCommand(update, updateType, text, cancellationToken),
            _ or null => new(update.Message!.Chat.Id, Text: "Not implemented command")
        };
    }
}