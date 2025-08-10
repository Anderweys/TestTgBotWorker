using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using TestTgBot.Application.Configuration.Groq;
using TestTgBot.Application.Services.Abst.Groq;
using TestTgBot.Application.Services.Impl.Groq;

namespace TestTgBot.Application.Web;

public static class GroqStartup
{
    public static IServiceCollection AddGroqServices(this IServiceCollection services)
    {
        services.AddHttpClient<IGroqHttpClient, GroqHttpClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<GroqOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
        });
        services.AddScoped<IGroqService, GroqService>();

        return services;
    }
}