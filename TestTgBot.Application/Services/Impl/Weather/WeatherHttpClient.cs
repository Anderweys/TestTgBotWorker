using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TestTgBot.Application.Configuration;
using TestTgBot.Application.Models.Rest.V1.Weather;
using TestTgBot.Application.Models.Rest.V1.Weather.Models;
using TestTgBot.Application.Services.Abst.Weather;

namespace TestTgBot.Application.Services.Impl.Weather;

public sealed class WeatherHttpClient : IWeatherHttpClient
{
    private readonly HttpClient _http;
    private readonly WeatherOptions _options;
    private readonly JsonSerializerOptions _deserialize = new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

    public WeatherHttpClient(HttpClient http, IOptions<WeatherOptions> options)
    {
        _http=http;
        _options=options.Value;
    }

    public async Task<WeatherResponseV1?> GetCurrentWeather(CancellationToken cancellationToken)
    {
        try
        {
            var url = string.Format(CultureInfo.InvariantCulture, _options.Url, _options.Latitude, _options.Longitude);
            var response = await _http.GetFromJsonAsync<WeatherResponseV1>(url, _deserialize, cancellationToken);

            return response;
        }
        catch
        {
            return null;
        }
    }

    public string ConvertToPrettyMessage(CurrentWeatherV1 weather)
    {
        var temp = $"{weather.Temperature:+0;-0}°C";
        var windDir = GetWindDirection(weather.Winddirection);
        var emoji = GetWeatherEmoji(weather.Weathercode);
        var description = GetWeatherDescription(weather.Weathercode);

        return $"Сегодня {description} {emoji}, {temp}\n" +
               $"Ветер: {weather.Windspeed} км/ч с {windDir}";
    }

    private static string GetWindDirection(double deg) => deg switch
    {
        >= 337.5 or < 22.5 => "севера",
        >= 22.5 and < 67.5 => "северо-востока",
        >= 67.5 and < 112.5 => "востока",
        >= 112.5 and < 157.5 => "юго-востока",
        >= 157.5 and < 202.5 => "юга",
        >= 202.5 and < 247.5 => "юго-запада",
        >= 247.5 and < 292.5 => "запада",
        _ => "северо-запада"
    };

    private static string GetWeatherEmoji(int code) => code switch
    {
        0 => "☀️",
        1 or 2 => "🌤",
        3 => "☁️",
        45 or 48 => "🌫",
        51 or 53 or 55 => "🌦",
        61 or 63 or 65 => "🌧",
        66 or 67 => "🌨",
        71 or 73 or 75 => "❄️",
        95 => "⛈",
        _ => "🤷"
    };

    private static string GetWeatherDescription(int code) => code switch
    {
        0 => "ясно",
        1 or 2 => "слегка облачно",
        3 => "пасмурно",
        45 or 48 => "туман",
        51 or 53 or 55 => "морось",
        61 or 63 or 65 => "дождь",
        66 or 67 => "ледяной дождь",
        71 or 73 or 75 => "снег",
        95 => "гроза",
        _ => "неизвестно"
    };
}