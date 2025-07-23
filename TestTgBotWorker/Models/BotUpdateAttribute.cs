using TestTgBotWorker.Models.Enum;

namespace TestTgBotWorker.Models;

public class BotUpdateAttribute : Attribute
{
    public BotUpdateType Type { get; }
    public string Command { get; }

    public BotUpdateAttribute(BotUpdateType type, string command)
    {
        Type=type;
        Command=command;
    }
}