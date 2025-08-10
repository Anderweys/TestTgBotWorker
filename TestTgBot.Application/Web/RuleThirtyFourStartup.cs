using Microsoft.Extensions.DependencyInjection;
using TestTgBot.Application.Services.Abst.RuleThirtyFour;
using TestTgBot.Application.Services.Impl.RuleThirtyFour;

namespace TestTgBot.Application.Web;

public static class RuleThirtyFourStartup
{
    public static void AddRuleThirtyFourServices(this IServiceCollection services)
    {
        services.AddHttpClient<IRuleThirtyFourHttpClient, RuleThirtyFourHttpClient>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        });
        services.AddScoped<IRuleThirtyFourService, RuleThirtyFourService>();
    }
}