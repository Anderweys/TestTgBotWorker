using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TestTgBot.Application.Handlers.Abst.Commands;
using TestTgBot.Application.Models.Domain.Handlers;
using TestTgBot.Application.Models.Enum;
using TestTgBot.Application.Services.Abst.RuleThirtyFour;
using TestTgBot.Application.Services.Abst.Weather;

namespace TestTgBot.Application.Handlers.Impl.Commands;

public sealed class OperationCommandHandler : IOperationCommandHandler
{
    private readonly IWeatherHttpClient _weatherClient;
    private readonly IRuleThirtyFourService _ruleThirtyFourService;

    public OperationCommandHandler(IWeatherHttpClient weatherClient, IRuleThirtyFourService ruleThirtyFourService)
    {
        _weatherClient=weatherClient;
        _ruleThirtyFourService=ruleThirtyFourService;
    }

    public async Task<HandlerUpdateResult> HandleCommand(Update update, BotUpdateCommandType updateType, string text, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;

        return updateType switch
        {
            BotUpdateCommandType.Ping => new(chatId, Text: "pong"),
            BotUpdateCommandType.RuleThirtyFour => await HandleRuleThirtyFour(chatId, text, cancellationToken),
            BotUpdateCommandType.Weather => await HandleWeather(chatId, text, cancellationToken),
            _ => new(update.Message!.Chat.Id, Text: "Not implemented command"),
        };
    }

    private async Task<HandlerUpdateResult> HandleRuleThirtyFour(long chatId, string text, CancellationToken cancellationToken)
    {
        var ruleMediaResponse = await _ruleThirtyFourService.GetRandomMediaUrl(text, cancellationToken);
        if (ruleMediaResponse is null)
        {
            return new(chatId, Text: "😔 Не удалось найти подходящий файл.");
        }

        return ruleMediaResponse.Type switch
        {
            ChatChannelType.Gif => new(
                chatId,
                File: InputFile.FromUri(ruleMediaResponse.Url),
                HasSpoiler: true,
                Caption: $"🔞 Результат по: `{text}`",
                ParseMode: ParseMode.Markdown,
                ChannelType: ChatChannelType.Gif),

            ChatChannelType.Photo => new(
                chatId,
                File: InputFile.FromUri(ruleMediaResponse.Url),
                HasSpoiler: true,
                Caption: $"🔞 Результат по: `{text}`",
                ParseMode: ParseMode.Markdown,
                ChannelType: ChatChannelType.Photo),

            ChatChannelType.Video => new(
                chatId,
                File: InputFile.FromUri(ruleMediaResponse.Url),
                Caption: $"🔞 Видео по: `{text}`",
                ParseMode: ParseMode.Markdown,
                ChannelType: ChatChannelType.Video),

            _ => new(chatId, Text: "Формат файла не поддерживается.")
        };
    }

    private async Task<HandlerUpdateResult> HandleWeather(long chatId, string text, CancellationToken cancellationToken)
    {
        var response = await _weatherClient.GetCurrentWeather(cancellationToken);
        if (response is null)
        {
            return new(chatId, Text: "Cound't got weather.");
        }

        var message = _weatherClient.ConvertToPrettyMessage(response!.CurrentWeather);
        return new(chatId, Text: message);
    }
}
