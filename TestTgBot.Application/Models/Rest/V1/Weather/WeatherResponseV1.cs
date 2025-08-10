using System.Text.Json.Serialization;
using TestTgBot.Application.Models.Rest.V1.Weather.Models;

namespace TestTgBot.Application.Models.Rest.V1.Weather;

public sealed record WeatherResponseV1
{
    [JsonPropertyName("current_weather")]
    public required CurrentWeatherV1 CurrentWeather { get; set; }
}