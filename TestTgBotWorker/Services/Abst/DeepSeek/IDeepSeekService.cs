namespace TestTgBotWorker.Services.Abst.DeepSeek;

public interface IDeepSeekService
{
    Task<string> SendSingleMessage(string message, CancellationToken cancellationToken);
}
