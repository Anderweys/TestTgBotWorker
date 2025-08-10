namespace TestTgBot.Db.Models;

public record BotUserDb
{
    public long TelegramId { get; set; }
    public string? Username { get; set; }
    public BotRoleTypeDb Role { get; set; }
}