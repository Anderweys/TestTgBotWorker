using Microsoft.Extensions.Options;
using Telegram.Bot;
using TestTgBot.Application.Configuration.Groq;
using TestTgBot.Application.Models.Rest.V1.Groq;
using TestTgBot.Application.Models.Rest.V1.Groq.Models;
using TestTgBot.Application.Services.Abst.Groq;
using TestTgBot.Application.Services.Abst.Telegram;

namespace TestTgBot.Application.Services.Impl.Groq;

public class GroqService : IGroqService
{
    private const long SharedUserId = 0;
    private readonly IGroqHttpClient _client;
    private readonly GroqOptions _options;
    private readonly ITelegramSessionStorage _storage;
    private readonly ITelegramBotClient _bot;

    public GroqService(
        IGroqHttpClient client,
        IOptions<GroqOptions> options,
        ITelegramSessionStorage storage,
        ITelegramBotClient bot)
    {
        _client = client;
        _options = options.Value;
        _storage = storage;
        _bot=bot;
    }

    public Task<string> SendSingleMessage(string message, CancellationToken cancellationToken)
    {
        var request = new GroqRequestV1
        {
            Model = _options.Model,
            Messages =
            [
                new RoleV1
                {
                    Role = _options.Role.Role,
                    Content = string.Format(_options.Role.Content, message)
                }
            ]
        };

        return _client.Post(request, cancellationToken);
    }

    public string SendConflictSession(string userName, long chatId, long userId, int messageId, CancellationToken cancellationToken)
    {
        _storage.AddMessageId(chatId, userId, messageId);

        return $"⚠️ Это не ваша личная AI-сессия.\nЭто чат общения {userName}";
    }

    public async Task<string> SendWithSharedHistory(long chatId, string message, int userMessageId, CancellationToken cancellationToken)
    {
        var history = _storage.GetMessages(chatId, SharedUserId);

        var userMessage = new RoleV1 { Role = "user", Content = message };
        history.Add(userMessage);

        await MaybeCompress(history, cancellationToken);

        var request = new GroqRequestV1
        {
            Model = _options.Model,
            Messages = [.. history]
        };

        var response = await _client.Post(request, cancellationToken);

        _storage.AddMessageId(chatId, SharedUserId, userMessageId);

        _storage.AddMessage(chatId, SharedUserId, userMessage);
        _storage.AddMessage(chatId, SharedUserId, new RoleV1 { Role = "assistant", Content = response });

        _storage.UpdateLastUsed(chatId, SharedUserId);

        return "Groq\n\n" + response;
    }

    public async Task<string> SendWithPersonalHistory(long chatId, long userId, string userLogin, string message, int userMessageId, CancellationToken cancellationToken)
    {
        var history = _storage.GetMessages(chatId, userId);

        var userMessage = new RoleV1 { Role = "user", Content = message };
        history.Add(userMessage);

        await MaybeCompress(history, cancellationToken);

        var request = new GroqRequestV1
        {
            Model = _options.Model,
            Messages = [.. history]
        };

        var response = await _client.Post(request, cancellationToken);

        _storage.AddMessageId(chatId, userId, userMessageId);

        _storage.AddMessage(chatId, userId, userMessage);
        _storage.AddMessage(chatId, userId, new RoleV1 { Role = "assistant", Content = response });

        _storage.UpdateLastUsed(chatId, userId);

        return $"Groq to {userLogin}\n\n" + response;
    }

    private async Task MaybeCompress(List<RoleV1> history, CancellationToken cancellationToken)
    {
        var realMessages = history.Count(x => x.Role is "user" or "assistant");

        if (realMessages > _options.MaxMessages)
        {
            var toSummarize = history
                .Where(x => x.Role is "user" or "assistant")
                .Take(_options.CompressEvery * 2)
                .ToList();

            foreach (var msg in toSummarize)
            {
                history.Remove(msg);
            }

            var summary = await SummarizeConversation(toSummarize, cancellationToken);

            history.Insert(0, new RoleV1
            {
                Role = "system",
                Content = $"🧠 Сжатый контекст ({_options.CompressEvery} сообщений): {summary}"
            });
        }
    }

    private async Task<string> SummarizeConversation(List<RoleV1> messages, CancellationToken cancellationToken)
    {
        var prompt = new List<RoleV1>
        {
            new() { Role = "system", Content = 
            "Ты сжимаешь диалог между человеком и ИИ в одно краткое сообщение, сохраняя суть обсуждения." +
            "Не дели на разговоры, не анализируй, просто сделай одно короткое резюме диалога." },
            new() { Role = "user", Content = "Вот диалог. Сожми его и сохрани суть:\n\n" +
                    string.Join("\n\n", messages.Select(m => $"{m.Role}: {m.Content}")) }
        };

        var request = new GroqRequestV1
        {
            Model = _options.Model,
            Messages = [.. prompt]
        };

        return await _client.Post(request, cancellationToken);
    }

    public async Task ClearSharedSession(long chatId, CancellationToken cancellationToken)
    {
        var ids = _storage.GetMessageIds(chatId, 0);
        try 
        { 
            await _bot.DeleteMessages(chatId, ids, cancellationToken);
            _storage.ClearMessages(chatId, SharedUserId);
            _storage.ClearMessageIds(chatId, SharedUserId);
        }
        catch
        {
        }
    }

    public async Task ClearPersonalSession(long chatId, long userId, CancellationToken cancellationToken)
    {
        var ids = _storage.GetMessageIds(chatId, userId);
        try
        {
            await _bot.DeleteMessages(chatId, ids, cancellationToken);
            _storage.ClearMessages(chatId, SharedUserId);
            _storage.ClearMessageIds(chatId, SharedUserId);
        }
        catch
        {
        }
    }
}