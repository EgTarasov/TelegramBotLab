using Dapper;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotLab;

public static class Utilits
{
    private static Dictionary<string, string> _commands = new Dictionary<string, string>()
    {
        { "/start", "Show greeting message" },
        { "/get_id", "Show user Id" },
        { "/solve", "get two numbers and symbol and solve equation" },
        { "/currency", "Show current info about popular currency" },
    };

    public static async Task GetHelpMessage(ITelegramBotClient botClient)
    {
        var helpMessage = new List<string>();
        foreach (var command in _commands.Keys)
        {
            helpMessage.Add(command);
        }
        await botClient
    }
    public static async Task<bool> IsUserExist(long userId)
    {
        await using var conn = new SqliteConnection(BotInfo.connectionString);
        var lines = await conn.ExecuteAsync($"Select * from Users where UserId = {userId}");
        return lines != 1;
    }
    public static async Task AddMessageInfo(
        Chat user,
        Message response,
        Message reply)
    {
        try
        {
            await using var conn = new SqliteConnection(BotInfo.connectionString);
            await conn.ExecuteAsync($"INSERT INTO UsersMessages " +
                                    $"(UserId, ResponceInfo, ReplyInfo)" +
                                    $"VALUES ({user.Id}, '{response.Text.Replace("\'", "")}', '{reply.Text}');");
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}