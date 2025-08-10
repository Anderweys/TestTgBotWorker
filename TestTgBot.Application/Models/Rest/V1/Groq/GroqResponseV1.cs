using System.Text.Json.Serialization;
using TestTgBot.Application.Models.Rest.V1.Groq.Models;

namespace TestTgBot.Application.Models.Rest.V1.Groq;

public sealed record GroqResponseV1(
    [property: JsonPropertyName("choices")] GroqChoiceV1[] Choices
);
