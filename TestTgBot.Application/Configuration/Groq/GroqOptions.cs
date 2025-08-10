namespace TestTgBot.Application.Configuration.Groq;

public class GroqOptions
{
    public required string ApiKey { get; set; }
    public required string BaseUrl { get; set; }
    public required string Model { get; set; }
    public required int MaxMessages { get; set; }
    public required int CompressEvery { get; set; }
    public required GroqRoleOptions Role { get; set; }
}