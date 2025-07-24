using TestTgBotWorker.Models.Weather.V1;
using TestTgBotWorker.Models.Weather.V1.Models;

namespace TestTgBotWorker.Services.Abst;

public interface IWeatherClient
{
    Task<WeatherResponseV1?> GetCurrentWeather(CancellationToken cancellationToken);
    string ConvertToPrettyMessage(CurrentWeatherV1 weather);
}