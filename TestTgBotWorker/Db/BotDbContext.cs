using Microsoft.EntityFrameworkCore;
using TestTgBotWorker.Db.Models;

namespace TestTgBotWorker.Db;

public class BotDbContext(DbContextOptions<BotDbContext> options) : DbContext(options)
{
    public DbSet<BotUserDb> Users => Set<BotUserDb>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BotUserDb>()
            .HasKey(_ => _.TelegramId);
    }
}