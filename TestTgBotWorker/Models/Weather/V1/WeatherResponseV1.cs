using System.Text.Json.Serialization;
using TestTgBotWorker.Models.Weather.V1.Models;

namespace TestTgBotWorker.Models.Weather.V1;

public sealed record WeatherResponseV1
{
    [JsonPropertyName("current_weather")]
    public required CurrentWeatherV1 CurrentWeather { get; set; }
}