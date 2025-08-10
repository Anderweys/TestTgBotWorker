using TestTgBot.Application.Models.Domain.RuleThirtyFour;

namespace TestTgBot.Application.Services.Abst.RuleThirtyFour;

public interface IRuleThirtyFourService
{
    Task<RuleThirtyFourMediaResult?> GetRandomMediaUrl(string tags, CancellationToken cancellationToken);
    Task<Stream> GetMediaStream(RuleThirtyFourMediaResult mediaResult, CancellationToken cancellationToken);
}