using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBotLab.BotUpdates;

public static class Greeting
{
    private static readonly List<string> GreetingsMessages = new List<string>()
    {
        "Hello, {0}!\nMy name is {1}. Reporting for duty!",
        "Glad to see you {0}\n{1} to your services",
        "Target {0} has been detected.",
        "User {2} has create connection to bot {1}",
        "https://www.newmynamepix.com/upload/post/sample/1625490990_Good%20Morning%20Wishes%20Messages%20Quotes%20Card%20On%20Name%20Write.jpg",
    };

    public static async Task<Message> GreetMessage(
        ITelegramBotClient botClient,
        User bot,
        Message message,
        CancellationToken cts)
    {
        var random = new Random();
        var ind = random.Next(0, 5);
        var greetMessage = string.Format(
            GreetingsMessages[ind],
            message.Chat.Username,
            bot.FirstName,
            message.Chat.Id);

        Message sendMessage;
        if (ind < 4)
        {
            sendMessage = await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: greetMessage,
                cancellationToken: cts);
        }
        else
        {
            sendMessage = await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: new InputOnlineFile(greetMessage),
                cancellationToken: cts);
        }

        return sendMessage;
    }
}