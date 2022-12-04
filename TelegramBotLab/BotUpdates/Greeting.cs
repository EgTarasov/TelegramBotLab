using Dapper;
using Microsoft.Data.Sqlite;
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
        //Create greeting message from possible options
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
        
        await Utilits.AddMessageInfo(message.Chat, message, sendMessage);
        //Add user to database
        try
        {
            if (await Utilits.IsUserExist(message.Chat.Id))
            {
                throw new ArgumentException("User has been already added to database!");
            }
            await using var conn = new SqliteConnection(BotInfo.connectionString);
            await conn.ExecuteAsync($"INSERT INTO Users (UserId, Name)" +
                                    $"VALUES ({message.Chat.Id}, '{message.Chat.Username}');");
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return sendMessage;
    }

    public static async Task<Message> GetUserId(
        ITelegramBotClient botClient,
        Message response,
        CancellationToken cts)
    {
        var reply = await botClient.SendTextMessageAsync(
            chatId: response.Chat.Id,
            text: $"Your ID is {response.Chat.Id}",
            cancellationToken: cts
        );
        await Utilits.AddMessageInfo(response.Chat, response, reply);
        return reply;
    }
}