namespace TestTgBotWorker.Services.Abst.Telegram;

public interface ITelegramDebounceCommandService
{
    public bool IsCommandAllowed(string command);
}