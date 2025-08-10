using TestTgBot.Application.Models.Rest.V1.Weather;
using TestTgBot.Application.Models.Rest.V1.Weather.Models;

namespace TestTgBot.Application.Services.Abst.Weather;

public interface IWeatherHttpClient
{
    Task<WeatherResponseV1?> GetCurrentWeather(CancellationToken cancellationToken);
    string ConvertToPrettyMessage(CurrentWeatherV1 weather);
}