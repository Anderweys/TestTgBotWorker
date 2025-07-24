using TestTgBotWorker.Models.Groq.V1;

namespace TestTgBotWorker.Services.Abst.Groq;

public interface IGroqHttpClient
{
    Task<string> Post(GroqRequestV1 request, CancellationToken cancellationToken);
}