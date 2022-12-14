using System.Net.NetworkInformation;
using Dapper;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotLab.BotUpdates;

namespace TelegramBotLab;

public class Utilits: IUpdate
{
    private static Dictionary<string, string> _commands = new Dictionary<string, string>()
    {
        { "/start", "Show greeting message" },
        { "/get_id", "Show user Id" },
        { "/solve", "get two numbers and symbol and solve equation(32 + 12)" },
        { "/currency", "Show current info about popular currency(/currency or /currency Доллар США)" },
    };
    
    

    public async Task<Message> Update(
        ITelegramBotClient botClient,
        Message request,
        CancellationToken cts)
    {
        var helpMessage = new List<string>();
        foreach (var command in _commands.Keys)
        {
            helpMessage.Add(command + "\t" +_commands[command]);
        }

        var reply = await botClient.SendTextMessageAsync(
            chatId: request.Chat.Id,
            text: string.Join(Environment.NewLine, helpMessage),
            cancellationToken: cts);
        return reply;
    }
    public static async Task<bool> IsUserExist(Chat chat)
    {
        await using var conn = new SqliteConnection(BotInfo.connectionString);
        var lines = (await conn
                    .QueryAsync<Models.User>("Select * from Users where UserId = @Id", chat))
                    .ToList();
        return lines.Count == 1;
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
            await conn.ExecuteAsync($"INSERT INTO UsersMessages " +
                                    $"(UserId, RequestInfo, ReplyInfo)" +
                                    $"VALUES (@Id, '{request.Text.Replace("\'", "")}', '{reply.Text}');", user);

        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}