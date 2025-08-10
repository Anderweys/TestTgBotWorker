using System.Text.Json.Serialization;

namespace TestTgBot.Application.Models.Rest.V1.DeepSeek.Models;

public record ApiErrorV1
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}