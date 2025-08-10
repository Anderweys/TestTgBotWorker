using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TestTgBot.Application.Configuration;
using TestTgBot.Application.Services.Abst.Telegram;

namespace TestTgBot.Application.Services.Impl.Workers;

public sealed class CleanupWorker : BackgroundService
{
    private readonly TimeSpan _sessionDelay = DateTime.UtcNow.TimeOfDay;
    private readonly TimeSpan _ruleDelay = DateTime.UtcNow.TimeOfDay;
    private const int ruleMessageDeletingSize = 100;

    private readonly ITelegramSessionStorage _storage;
    private readonly ITelegramBotClient _bot;
    private readonly BotOptions _options;

    public CleanupWorker(ITelegramSessionStorage storage, ITelegramBotClient bot, IOptions<BotOptions> options)
    {
        _storage = storage;
        _bot = bot;
        _options=options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {

            await HandleSession(cancellationToken);
            await HandleRule(cancellationToken);

            await Task.Delay(_options.GlobalDelay, cancellationToken);
        }
    }

    private async Task HandleSession(CancellationToken cancellationToken)
    {
        if (_sessionDelay <= DateTime.UtcNow.TimeOfDay)
        {
            return;
        }

        foreach (var (chatId, userId) in _storage.GetAllSessions())
        {
            var lastUsed = _storage.GetLastUsed(chatId, userId);
            if (lastUsed.HasValue && DateTimeOffset.UtcNow - lastUsed > _options.SessionTimeout)
            {
                var ids = _storage.GetMessageIds(chatId, userId);
                try
                {
                    await _bot.DeleteMessages(chatId, ids, cancellationToken);
                }
                catch
                {
                    continue;
                }

                _storage.ClearMessageIds(chatId, userId);
                _storage.ClearMessages(chatId, userId);
            }
        }

        _sessionDelay.Add(_options.SessionDelay);
    }
    private async Task HandleRule(CancellationToken cancellationToken)
    {
        if (_ruleDelay >= DateTime.UtcNow.TimeOfDay)
        {
            return;
        }
        var ruleTimeout = DateTime.UtcNow - _options.RuleTimeout;
        
        var ruleMessages = _storage.GetRuleMessagesByDate(ruleTimeout);
        var groupRules = ruleMessages.GroupBy(x => x.chatId);

        foreach (var groupRule in groupRules)
        {
            var ids = groupRule.Select(_ => _.messageId).ToList();
            for (int i = 0; i < ids.Count; i += ruleMessageDeletingSize)
            {
                var batch = ids.Skip(i).Take(ruleMessageDeletingSize).ToList();
                await _bot.DeleteMessages(groupRule.Key, batch, cancellationToken);
                _storage.ClearRuleMessagesByIds(groupRule.Key, ids);
            }
        }

        _sessionDelay.Add(_options.RuleDelay);
    }
}