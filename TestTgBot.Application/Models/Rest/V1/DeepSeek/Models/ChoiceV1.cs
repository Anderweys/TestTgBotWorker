using System.Text.Json.Serialization;

namespace TestTgBot.Application.Models.Rest.V1.DeepSeek.Models;

public record ChoiceV1
{
    [JsonPropertyName("message")]
    public ChatMessageV1 Message { get; set; } = new();
}