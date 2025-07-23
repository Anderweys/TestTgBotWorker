using TestTgBotWorker.Models.Enum;

namespace TestTgBotWorker.Db.Models;

public record BotUserDb
{
    public long TelegramId { get; set; }
    public string? Username { get; set; }
    public BotRole Role { get; set; }
}