using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Telegram.Bot;
using TestTgBotWorker.Configuration;
using TestTgBotWorker.Configuration.DeepSeek;
using TestTgBotWorker.Configuration.Groq;
using TestTgBotWorker.Db;
using TestTgBotWorker.Repositories.Abst;
using TestTgBotWorker.Repositories.Impl;
using TestTgBotWorker.Services.Abst;
using TestTgBotWorker.Services.Abst.DeepSeek;
using TestTgBotWorker.Services.Abst.Groq;
using TestTgBotWorker.Services.Abst.Telegram;
using TestTgBotWorker.Services.Impl;
using TestTgBotWorker.Services.Impl.DeepSeek;
using TestTgBotWorker.Services.Impl.Groq;
using TestTgBotWorker.Services.Impl.Telegram;

var builder = Host.CreateApplicationBuilder(args);

#region UserSecrets

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

#endregion

#region Configurations

builder.Services.AddOptions<DbOptions>()
    .Bind(builder.Configuration.GetSection("Db"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

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

#endregion

#region EF Core

//builder.Services.AddPooledDbContextFactory<BotDbContext>((isp, opt) =>
//{
//    var options = isp.GetRequiredService<IOptions<DbOptions>>();
//    opt.UseNpgsql(options.Value.ConnectionString);
//});
//builder.Services.AddScoped<BotDbContext>();

#endregion

#region Services

builder.Services.AddHttpClient(nameof(DeepSeekHttpClient), (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<DeepSeekOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.ApiKey}");
    client.DefaultRequestHeaders.Add("HTTP-Referer", options.SiteUrl);
    client.DefaultRequestHeaders.Add("X-Title", options.SiteName);
    client.DefaultRequestHeaders.Add("User-Agent", "TestTgBot/1.0");
});
builder.Services.AddScoped<IDeepSeekHttpClient, DeepSeekHttpClient>();

builder.Services.AddHttpClient(nameof(GroqHttpClient), (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<GroqOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
});
builder.Services.AddScoped<IGroqHttpClient, GroqHttpClient>();

builder.Services.AddHostedService<BotPollingWorker>();
builder.Services.AddSingleton(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<BotOptions>>();
    return new TelegramBotClient(options.Value.Token);
});
builder.Services.AddSingleton<ITelegramDebounceCommandService, TelegramDebounceCommandService>();

builder.Services.AddScoped<ITelegramHandler, TelegramHandler>();
builder.Services.AddScoped<IGroqService, GroqService>();
builder.Services.AddScoped<IDeepSeekService, DeepSeekService>();
builder.Services.AddScoped<IWeatherClient, WeatherClient>();
//builder.Services.AddScoped<IBotUserRepository, BotUserRepository>();

#endregion

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<BotDbContext>>();
//    using var db = dbFactory.CreateDbContext();
//    db.Database.Migrate();
//}

var webApp = WebApplication.Create();
webApp.MapMethods("/ping", ["GET", "HEAD"], () => "pong");
_ = Task.Run(() => webApp.RunAsync());

await app.RunAsync();