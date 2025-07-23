using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TestTgBotWorker.Configuration;
using TestTgBotWorker.Extensions;
using TestTgBotWorker.Models.Enum;
using TestTgBotWorker.Repositories.Abst;

namespace TestTgBotWorker.Services;

public class BotPollingWorker : BackgroundService
{
    private readonly IBotUserRepository _userRepository;
    private readonly TelegramBotClient _bot;
    private int _offset = 0;

    public BotPollingWorker(IBotUserRepository userRepository, IOptions<BotOptions> options)
    {
        _userRepository=userRepository;
        _bot = new TelegramBotClient(options.Value.Token);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var updates = await _bot.GetUpdates(offset: _offset, timeout: 30, cancellationToken: cancellationToken);

            foreach (var update in updates)
            {
                try
                {
                    if (update.Message is null)
                    {
                        await _bot.SendMessage(update.Message!.Chat.Id, "Message is null", cancellationToken: cancellationToken);
                        continue;
                    }

                    _offset = update.Id + 1;
                    var commandText = update.Message?.Text?.Trim()?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    var updateType = BotExtensions.GetCommandByText(commandText);
                    if (updateType == null)
                    {
                        await _bot.SendMessage(update.Message!.Chat.Id, "Not implemented command", cancellationToken: cancellationToken);
                        continue;
                    }
                    var command = updateType.Value.GetCommandType();

                    switch (command)
                    {
                        case BotUpdateType.Operation:
                            await HandleOperationCommand(update, updateType.Value, cancellationToken);
                            break;

                        case BotUpdateType.User:
                            await HandleUserCommand(update, updateType.Value, cancellationToken);
                            break;

                        default:
                            await _bot.SendMessage(update.Message!.Chat.Id, "Not implemented command", cancellationToken: cancellationToken);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (update.Message is not null)
                    {
                        await _bot.SendMessage(update.Message.Chat.Id, $"Error: {ex.Message}, command: {update.Message.Text ?? ""}", cancellationToken: cancellationToken);
                    }
                }
            }
        }
    }

    private async Task HandleOperationCommand(Update update, BotUpdateCommandType updateType, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;

        switch (updateType)
        {
            case BotUpdateCommandType.Ping:
                await _bot.SendMessage(chatId, "pong", cancellationToken: cancellationToken);
                break;

            case BotUpdateCommandType.Weather:
                await _bot.SendMessage(chatId, "Сегодня солнечно 🌞 +25°C", cancellationToken: cancellationToken);
                break;

            default:
                await _bot.SendMessage(update.Message!.Chat.Id, "Not implemented command", cancellationToken: cancellationToken);
                break;
        }
    }

    private async Task HandleUserCommand(Update update, BotUpdateCommandType updateType, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;

        switch (updateType)
        {
            case BotUpdateCommandType.AddRole:
                var text = update.Message.Text;
                var parts = text!.Split(' ');
                if (parts.Length == 3)
                {
                    var role = parts[1];
                    var username = parts[2].TrimStart('@');

                    if (!Enum.TryParse<BotRole>(role, out var roleType))
                    {
                        await _bot.SendMessage(chatId, $"Роль \"{role}\" неизвестна", cancellationToken: cancellationToken);
                        return;
                    }

                    var user = await _userRepository.GetUserByName(username, cancellationToken);
                    if (user is null)
                    {
                        await _bot.SendMessage(chatId, $"Такого пользователя \"{username}\" не существует", cancellationToken: cancellationToken);
                        return;
                    }

                    await _userRepository.AddRole(username, roleType, cancellationToken);
                    await _userRepository.SaveChangesAsync(cancellationToken);

                    await _bot.SendMessage(chatId, $"Роль {role} установлена для {username}", cancellationToken: cancellationToken);
                }
                break;

            default:
                await _bot.SendMessage(update.Message!.Chat.Id, "Not implemented command", cancellationToken: cancellationToken);
                break;
        }
    }
}