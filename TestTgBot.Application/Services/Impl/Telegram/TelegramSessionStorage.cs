using System.Collections.Concurrent;
using TestTgBot.Application.Models.Rest.V1.Groq.Models;
using TestTgBot.Application.Services.Abst.Telegram;

namespace TestTgBot.Application.Services.Impl.Telegram;

public sealed class TelegramSessionStorage : ITelegramSessionStorage
{
    private readonly ConcurrentDictionary<(long chatId, long userId), List<RoleV1>> _messages = new();
    private readonly ConcurrentDictionary<(long chatId, long userId), List<int>> _messageIds = new();
    private readonly ConcurrentDictionary<(long chatId, long userId), DateTimeOffset> _lastUsed = new();
    private readonly ConcurrentDictionary<(long chatId, int messageId), DateTime> _ruleMessages = new();

    public void AddMessage(long chatId, long userId, RoleV1 message)
    {
        var key = (chatId, userId);
        var list = _messages.GetOrAdd(key, _ => []);
        list.Add(message);
        UpdateLastUsed(chatId, userId);
    }

    public List<RoleV1> GetMessages(long chatId, long userId)
    {
        var key = (chatId, userId);
        return _messages.TryGetValue(key, out var list) ? list : [];
    }

    public void ClearMessages(long chatId, long userId) => _messages.TryRemove((chatId, userId), out _);

    public void AddMessageId(long chatId, long userId, int messageId)
    {
        var key = (chatId, userId);
        var list = _messageIds.GetOrAdd(key, _ => []);
        list.Add(messageId);
    }

    public List<int> GetMessageIds(long chatId, long userId)
    {
        var key = (chatId, userId);
        return _messageIds.TryGetValue(key, out var list) ? list : [];
    }

    public void ClearMessageIds(long chatId, long userId) => _messageIds.TryRemove((chatId, userId), out _);

    public void UpdateLastUsed(long chatId, long userId) => _lastUsed[(chatId, userId)] = DateTimeOffset.UtcNow;

    public DateTimeOffset? GetLastUsed(long chatId, long userId)
    {
        return _lastUsed.TryGetValue((chatId, userId), out var ts) ? ts : null;
    }

    public IEnumerable<(long chatId, long userId)> GetAllSessions() => _lastUsed.Keys;

    public void AddRuleMessage(long chatId, int messageId, DateTime ttl) =>
        _ruleMessages.TryAdd((chatId, messageId), ttl);

    public IEnumerable<(long chatId, int messageId)> GetRuleMessagesByDate(DateTime date)
    {
        return [.. _ruleMessages
            .Where(_ => _.Value < date)
            .Select(_ => _.Key)];
    }

    public void ClearRuleMessagesByIds(long key, List<int> ids)
    {
        foreach (var id in ids)
        {
            _ruleMessages.TryRemove((key, id), out _);
        }
    }
}