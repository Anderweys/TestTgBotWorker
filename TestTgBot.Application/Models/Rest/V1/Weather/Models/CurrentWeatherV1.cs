using System.Text.Json.Serialization;

namespace TestTgBot.Application.Models.Rest.V1.Weather.Models;

public sealed record CurrentWeatherV1
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("windspeed")]
    public double Windspeed { get; set; }

    [JsonPropertyName("winddirection")]
    public double Winddirection { get; set; }

    [JsonPropertyName("weathercode")]
    public int Weathercode { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
}