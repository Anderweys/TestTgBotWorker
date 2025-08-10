using TestTgBot.Application.Models.Enum;

namespace TestTgBot.Application.Models.Domain;

public record ValidateUpdateResult(string Text, BotUpdateCommandType UpdateType);