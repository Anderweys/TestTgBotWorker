namespace TestTgBotWorker.Models.Enum;

public enum BotUpdateCommandType
{
    [BotUpdate(BotUpdateType.Operation, "/ping")]
    Ping,

    [BotUpdate(BotUpdateType.Operation, "/weather")]
    Weather,

    [BotUpdate(BotUpdateType.User, "/addRole")]
    AddRole
}