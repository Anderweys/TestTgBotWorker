using System.Text.Json.Serialization;
using TestTgBotWorker.Models.Groq.V1.Models;

namespace TestTgBotWorker.Models.Groq.V1;

public sealed record GroqResponseV1(
    [property: JsonPropertyName("choices")] GroqChoiceV1[] Choices
);
