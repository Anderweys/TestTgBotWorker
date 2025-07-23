using Microsoft.EntityFrameworkCore;
using TestTgBotWorker.Db;
using TestTgBotWorker.Db.Models;
using TestTgBotWorker.Models.Enum;
using TestTgBotWorker.Repositories.Abst;

namespace TestTgBotWorker.Repositories.Impl;

public class BotUserRepository : IBotUserRepository
{
    private readonly BotDbContext _context;

    public BotUserRepository(BotDbContext context)
    {
        _context=context;
    }

    public async Task AddRole(string name, BotRole role, CancellationToken cancellationToken)
    {
        var user = new BotUserDb
        {
            Username = name,
            Role = role
        };
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<BotUserDb?> GetUserByName(string name, CancellationToken cancellationToken)
        => await _context.Users.SingleOrDefaultAsync(_ => _.Username == name, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken) 
        => await _context.SaveChangesAsync(cancellationToken);
}