using System.Text.Json.Serialization;

namespace TestTgBotWorker.Models.Weather.V1.Models;

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