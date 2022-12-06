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

    public static async Task<Message> GetHelpMessage(
        ITelegramBotClient botClient,
        Message request,
        CancellationToken cts)
    {
        var helpMessage = new List<string>();
        foreach (var command in _commands.Keys)
        {
            helpMessage.Add(command);
        }

        var reply = await botClient.SendTextMessageAsync(
            chatId: request.Chat.Id,
            text: string.Join(Environment.NewLine, helpMessage),
            cancellationToken: cts);
        return reply;
    }
    public static async Task<bool> IsUserExist(long userId)
    {
        await using var conn = new SqliteConnection(BotInfo.connectionString);
        var lines = await conn.ExecuteAsync($"Select * from Users where UserId = {userId}");
        return lines != 1;
    }
    public static async Task AddMessageInfo(
        Chat user,
        Message request,
        Message reply)
    {
        try
        {
            await using var conn = new SqliteConnection(BotInfo.connectionString);
            await conn.ExecuteAsync($"INSERT INTO UsersMessages " +
                                    $"(UserId, RequestInfo, ReplyInfo)" +
                                    $"VALUES ({user.Id}, '{request.Text.Replace("\'", "")}', '{reply.Text}');");
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}