using TestTgBotWorker.Db;
using TestTgBotWorker.Services;
using TestTgBotWorker.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTgBotWorker.Repositories.Abst;
using TestTgBotWorker.Repositories.Impl;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<DbOptions>()
    .Bind(builder.Configuration.GetSection("Db"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<BotOptions>()
    .Bind(builder.Configuration.GetSection("Bot"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddPooledDbContextFactory<BotDbContext>((isp, opt) =>
{
    var options = isp.GetRequiredService<IOptions<DbOptions>>();
    opt.UseNpgsql(options.Value.ConnectionString);
});
builder.Services.AddScoped<BotDbContext>();

builder.Services.AddHostedService<BotPollingWorker>();
builder.Services.AddScoped<IBotUserRepository, BotUserRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<BotDbContext>>();
    using var db = dbFactory.CreateDbContext();
    db.Database.Migrate();
}

var webApp = WebApplication.Create();
webApp.MapGet("/ping", () => "pong");
_ = Task.Run(() => webApp.RunAsync());

await app.RunAsync();