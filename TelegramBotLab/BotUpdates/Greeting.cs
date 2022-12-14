using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBotLab.BotUpdates;

public  class Greeting: IUpdate
{
    private  readonly List<string> GreetingsMessages = new List<string>()
    {
        "Hello, {0}!\nMy name is {1}. Reporting for duty!",
        "Glad to see you {0}\n{1} to your services",
        "Target {0} has been detected.",
        "User {2} has create connection to bot {1}",
        "https://www.newmynamepix.com/upload/post/sample/1625490990_Good%20Morning%20Wishes%20Messages%20Quotes%20Card%20On%20Name%20Write.jpg",
    };

    async Task<Message> IUpdate.Update(
        ITelegramBotClient botClient,
        Message message,
        CancellationToken cts)
    {
        var bot = await botClient.GetMeAsync(cancellationToken: cts);
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

        if (await Utilits.IsUserExist(message.Chat))
        {
            throw new DataException("User has been already added to database!");
        }

        await using var conn = new SqliteConnection(BotInfo.connectionString);
        await conn.ExecuteAsync($"INSERT INTO Users (UserId, Name)" +
                                $"VALUES (@Id, @Username);", message.Chat);
        return sendMessage;
    }
}