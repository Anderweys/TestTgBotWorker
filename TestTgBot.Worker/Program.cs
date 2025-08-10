using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTgBot.Application.Configuration;
using TestTgBot.Application.Configuration.DeepSeek;
using TestTgBot.Application.Configuration.Groq;
using TestTgBot.Application.Services.Abst.Weather;
using TestTgBot.Application.Services.Impl.Weather;
using TestTgBot.Application.Web;

var builder = Host.CreateApplicationBuilder(args);

#region UserSecrets

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

#endregion

#region Configurations

builder.Services.AddOptions<BotOptions>()
    .Bind(builder.Configuration.GetSection("Bot"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<DeepSeekOptions>()
    .Bind(builder.Configuration.GetSection("Ai:DeepSeek"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<GroqOptions>()
    .Bind(builder.Configuration.GetSection("Ai:Groq"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<WeatherOptions>()
    .Bind(builder.Configuration.GetSection("Weather"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<RuleThirtyFourOptions>()
    .Bind(builder.Configuration.GetSection("RuleThirtyFour"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

#endregion

#region Services

builder.Services.AddTelegramServices();
builder.Services.AddHandlerServices();
builder.Services.AddDeepSeekServices();
builder.Services.AddGroqServices();
builder.Services.AddRuleThirtyFourServices();

builder.Services.AddHttpClient<IWeatherHttpClient, WeatherHttpClient>();

#endregion

var app = builder.Build();

var webApp = WebApplication.Create();
webApp.MapMethods("/ping", ["GET", "HEAD"], () => "pong");
_ = Task.Run(() => webApp.RunAsync());

await app.RunAsync();