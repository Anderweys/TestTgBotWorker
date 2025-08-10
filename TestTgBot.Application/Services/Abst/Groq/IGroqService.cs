namespace TestTgBot.Application.Services.Abst.Groq;

public interface IGroqService
{
    Task<string> SendSingleMessage(string message, CancellationToken cancellationToken);

    string SendConflictSession(string userName, long chatId, long userId, int messageId, CancellationToken cancellationToken);

    Task<string> SendWithPersonalHistory(long chatId, long userId, string userLogin, string message, int userMessageId, CancellationToken cancellationToken);

    Task<string> SendWithSharedHistory(long chatId, string message, int userMessageId, CancellationToken cancellationToken);

    Task ClearSharedSession(long chatId, CancellationToken cancellationToken);

    Task ClearPersonalSession(long chatId, long userId, CancellationToken cancellationToken);
}