using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TestTgBot.Application.Configuration;
using TestTgBot.Application.Services.Abst.Telegram;
using TestTgBot.Application.Services.Impl.Telegram;
using TestTgBot.Application.Services.Impl.Workers;

namespace TestTgBot.Application.Web;

public static class TelegramStartup
{
    public static void AddTelegramServices(this IServiceCollection services)
    {
        services.AddSingleton<ITelegramDebounceCommandService, TelegramDebounceCommandService>();
        services.AddSingleton<ITelegramSessionStorage, TelegramSessionStorage>();
        services.AddSingleton<ITelegramBotClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<BotOptions>>();
            return new TelegramBotClient(options.Value.Token);
        });
        services.AddSingleton<ITelegramHandler, TelegramHandler>();

        services.AddHostedService<BotPoolingWorker>();
        services.AddHostedService<CleanupWorker>();
    }
}