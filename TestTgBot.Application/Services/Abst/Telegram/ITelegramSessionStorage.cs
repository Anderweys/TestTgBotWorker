using TestTgBot.Application.Models.Rest.V1.Groq.Models;

namespace TestTgBot.Application.Services.Abst.Telegram;

public interface ITelegramSessionStorage
{
    void AddMessage(long chatId, long userId, RoleV1 message);
    List<RoleV1> GetMessages(long chatId, long userId);
    void ClearMessages(long chatId, long userId);

    void AddMessageId(long chatId, long userId, int messageId);
    List<int> GetMessageIds(long chatId, long userId);
    void ClearMessageIds(long chatId, long userId);

    void UpdateLastUsed(long chatId, long userId);
    DateTimeOffset? GetLastUsed(long chatId, long userId);
    IEnumerable<(long chatId, long userId)> GetAllSessions();

    void AddRuleMessage(long chatId, int messageId, DateTime ttl);
    IEnumerable<(long chatId, int messageId)> GetRuleMessagesByDate(DateTime date);
    void ClearRuleMessagesByIds(long key, List<int> ids);
}