namespace TestTgBot.Application.Services.Abst.Telegram;

public interface ITelegramHandler
{
    Task ProcessBatch(CancellationToken cancellationToken);
}