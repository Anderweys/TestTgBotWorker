using Microsoft.Extensions.DependencyInjection;
using TestTgBot.Application.Handlers.Abst.Commands;
using TestTgBot.Application.Handlers.Impl.Commands;

namespace TestTgBot.Application.Web;

public static class HandlerCommandStartup
{
    public static void AddHandlerServices(this IServiceCollection services)
    {
        #region Commands

        services.AddScoped<ICommandHandler, CommandHandler>();
        services.AddScoped<IAiCommandHandler, AiCommandHandler>();
        services.AddScoped<IOperationCommandHandler, OperationCommandHandler>();

        #endregion
    }
}