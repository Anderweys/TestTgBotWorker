using TestTgBotWorker.Db.Models;
using TestTgBotWorker.Models.Enum;

namespace TestTgBotWorker.Repositories.Abst;

public interface IBotUserRepository
{
    Task AddRole(string name, BotRole role, CancellationToken cancellationToken);
    Task<BotUserDb?> GetUserByName(string name, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}