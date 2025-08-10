namespace TestTgBot.Application.Configuration;

public class BotOptions
{
    public required string Token { get; set; }
    public required int Timeout { get; set; }
    public required int Limit { get; set; }

    public required TimeSpan DebounceDelay { get; set; }
    public required TimeSpan DataLifeTime { get; set; }
    public required int DataCommandSize { get; set; }

    public required TimeSpan SessionTimeout { get; set; }
    public required TimeSpan SessionDelay { get; set; }

    public required TimeSpan RuleDelay { get; set; }
    public required TimeSpan RuleTimeout { get; set; }

    public required TimeSpan GlobalDelay { get; set; }
}