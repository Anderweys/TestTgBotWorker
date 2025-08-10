using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using TestTgBot.Application.Configuration;
using TestTgBot.Application.Services.Abst.Telegram;

namespace TestTgBot.Application.Services.Impl.Telegram;

public sealed class TelegramDebounceCommandService : ITelegramDebounceCommandService
{
    private readonly BotOptions _options;
    private readonly ConcurrentDictionary<string, DateTime> _lastCommandTime = [];

    public TelegramDebounceCommandService(IOptions<BotOptions> options)
    {
        _options=options.Value;
    }

    public bool IsCommandAllowed(string key)
    {
        var now = DateTime.UtcNow;
        var debounceDelay = _options.DebounceDelay;

        if (_lastCommandTime.TryGetValue(key, out var lastTime))
        {
            if (now - lastTime > _options.DataLifeTime)
            {
                _lastCommandTime.TryRemove(key, out _);
            }
            else if (now - lastTime < _options.DebounceDelay)
            {
                return false;
            }
        }

        _lastCommandTime[key] = now;

        if (_lastCommandTime.Count > _options.DataCommandSize)
        {
            _ = Task.Run(CleanupOldEntries);
        }

        return true;
    }

    private void CleanupOldEntries()
    {
        var cutoffTime = DateTime.UtcNow - _options.DataLifeTime;
        var keysToRemove = _lastCommandTime
            .Where(_ => _.Value < cutoffTime)
            .Select(_ => _.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _lastCommandTime.TryRemove(key, out _);
        }
    }
}