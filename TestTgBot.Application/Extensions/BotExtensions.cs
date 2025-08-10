using System.Reflection;
using TestTgBot.Application.Models.Attributes;
using TestTgBot.Application.Models.Enum;

namespace TestTgBot.Application.Extensions;

public static class BotExtensions
{
    public static BotUpdateType? GetCommandType(this BotUpdateCommandType code)
    {
        var memberInfo = typeof(BotUpdateCommandType)
            .GetMember(code.ToString())
            .FirstOrDefault();

        if (memberInfo == null)
        {
            return null;
        }

        var attr = memberInfo.GetCustomAttribute<BotUpdateAttribute>();
        return attr?.Type;
    }

    public static BotUpdateCommandType? GetCommandByText(string? text)
    {
        foreach (var value in Enum.GetValues<BotUpdateCommandType>())
        {
            var memberInfo = typeof(BotUpdateCommandType)
                .GetMember(value.ToString())
                .FirstOrDefault();

            var attr = memberInfo?.GetCustomAttribute<BotUpdateAttribute>();
            if (attr?.Command.Equals(text, StringComparison.OrdinalIgnoreCase) == true)
            {
                return value;
            }
        }

        return null;
    }
}