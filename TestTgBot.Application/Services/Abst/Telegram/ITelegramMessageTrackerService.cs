namespace TestTgBot.Application.Services.Abst.Telegram;

public interface ITelegramMessageTrackerService
{
    void TrackMessage(long chatId, long userId, int messageId);
    IReadOnlyList<int> GetMessagesToDelete(long chatId, long userId);
    void ClearMessages(long chatId, long userId);
}