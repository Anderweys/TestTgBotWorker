using TestTgBot.Application.Models.Rest.V1.Groq;

namespace TestTgBot.Application.Services.Abst.Groq;

public interface IGroqHttpClient
{
    Task<string> Post(GroqRequestV1 request, CancellationToken cancellationToken);
}