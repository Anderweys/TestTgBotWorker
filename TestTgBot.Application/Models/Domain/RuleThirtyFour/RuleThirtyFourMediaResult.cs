using TestTgBot.Application.Models.Enum;

namespace TestTgBot.Application.Models.Domain.RuleThirtyFour;

public record RuleThirtyFourMediaResult
{
    public string Url { get; set; } = default!;
    public ChatChannelType Type { get; set; }
    public string FileName => Path.GetFileName(Url);
}