namespace TestTgBot.Application.Configuration.DeepSeek;

public class DeepSeekSettingOptions
{
    public required DeepSeekModelOptions Model { get; set; }
    public required DeepSeekRoleOptions Role { get; set; }
}