namespace TestTgBot.Application.Services.Abst.Telegram;

public interface ITelegramDebounceCommandService
{
    public bool IsCommandAllowed(string command);
}