namespace TestTgBotWorker.Services.Abst.Telegram;

public interface ITelegramHandler
{
    Task Run(CancellationToken cancellationToken);
}