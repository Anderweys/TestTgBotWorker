namespace TestTgBotWorker.Configuration;

public class WeatherOptions
{
    public required decimal Latitude { get; set; }
    public required decimal Longitude { get; set; }
    public required string Url { get; set; }
}