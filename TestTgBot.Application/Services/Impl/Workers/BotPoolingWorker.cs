using Microsoft.Extensions.Hosting;
using TestTgBot.Application.Services.Abst.Telegram;

namespace TestTgBot.Application.Services.Impl.Workers;

public class BotPoolingWorker : BackgroundService
{
    private readonly ITelegramHandler _telegramHandler;

    public BotPoolingWorker(ITelegramHandler telegramHandler)
    {
        _telegramHandler=telegramHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _telegramHandler.ProcessBatch(cancellationToken);
            }
            catch
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}