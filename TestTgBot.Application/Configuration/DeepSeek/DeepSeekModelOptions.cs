namespace TestTgBot.Application.Configuration.DeepSeek;

public class DeepSeekModelOptions
{
    public required int MaxTokens { get; set; }
    public required double Temperature { get; set; }
}