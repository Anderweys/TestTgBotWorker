namespace TestTgBot.Application.Configuration.DeepSeek;

public class DeepSeekOptions
{
    public required string ApiKey { get; set; }
    public required string BaseUrl { get; set; }
    public required string Model { get; set; }
    public required string SiteUrl { get; set; }
    public required string SiteName { get; set; }
    public required DeepSeekSettingOptions Settings { get; set; }
}