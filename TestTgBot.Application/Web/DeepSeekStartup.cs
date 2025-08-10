using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TestTgBot.Application.Configuration.DeepSeek;
using TestTgBot.Application.Services.Abst.DeepSeek;
using TestTgBot.Application.Services.Impl.DeepSeek;

namespace TestTgBot.Application.Web;

public static class DeepSeekStartup
{
    public static IServiceCollection AddDeepSeekServices(this IServiceCollection services)
    {
        services.AddHttpClient<IDeepSeekHttpClient, DeepSeekHttpClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<DeepSeekOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
            client.DefaultRequestHeaders.Add("HTTP-Referer", options.SiteUrl);
            client.DefaultRequestHeaders.Add("X-Title", options.SiteName);
            client.DefaultRequestHeaders.Add("User-Agent", "TestTgBot/1.0");
        });
        services.AddScoped<IDeepSeekService, DeepSeekService>();

        return services;
    }
}