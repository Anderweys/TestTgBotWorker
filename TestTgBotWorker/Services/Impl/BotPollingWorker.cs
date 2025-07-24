using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTgBotWorker.Services.Abst.Telegram;

namespace TestTgBotWorker.Services.Impl;

public class BotPollingWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public BotPollingWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider=serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var telegramHandler = scope.ServiceProvider.GetRequiredService<ITelegramHandler>();
        await telegramHandler.Run(cancellationToken);
    }
}