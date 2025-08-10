using TestTgBot.Application.Models.Attributes;

namespace TestTgBot.Application.Models.Enum;

public enum BotUpdateCommandType
{
    [BotUpdate(BotUpdateType.Ai, "/ds")]
    DeepSeek,

    [BotUpdate(BotUpdateType.Ai, "/gq")]
    Groq,

    [BotUpdate(BotUpdateType.Ai, "/gqc")]
    GroqCleanup,

    [BotUpdate(BotUpdateType.Ai, "/gqs")]
    GroqSession,

    [BotUpdate(BotUpdateType.Ai, "/gqsc")]
    GroqSessionCleanup,

    [BotUpdate(BotUpdateType.Operation, "/ping")]
    Ping,

    [BotUpdate(BotUpdateType.Operation, "/weather")]
    Weather,

    [BotUpdate(BotUpdateType.Operation, "/r34")]
    RuleThirtyFour,

    [BotUpdate(BotUpdateType.Operation, "/meme")]
    Meme
}