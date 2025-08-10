using TestTgBot.Application.Models.Enum;

namespace TestTgBot.Application.Models.Attributes;

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