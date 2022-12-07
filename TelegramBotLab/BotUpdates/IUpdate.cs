using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotLab.BotUpdates;

public interface IUpdate
{
    Task<Message> Update(
        ITelegramBotClient botClient,
        Message request,
        CancellationToken cts);
}