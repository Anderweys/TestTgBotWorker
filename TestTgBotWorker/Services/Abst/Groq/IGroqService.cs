namespace TestTgBotWorker.Services.Abst.Groq;

public interface IGroqService
{
    Task<string> SendSingleMessage(string message, CancellationToken cancellationToken);
}