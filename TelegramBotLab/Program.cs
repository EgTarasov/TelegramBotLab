using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotLab.BotUpdates;
using Update = Telegram.Bot.Types.Update;

namespace TelegramBotLab;

internal static class Program
{
    private static readonly ITelegramBotClient Bot = new TelegramBotClient(BotInfo.token);
    static async Task Main(string[] args)
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        
        Bot.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            cancellationToken: cancellationToken);

        Console.ReadLine();
    }

    public static async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is not { } message)
            {
                return;
            }

            if ((message.From ?? await botClient.GetMeAsync(cancellationToken)).IsBot is true)
            {
                return;
            }
            
            var operation = await ParseUpdate.GetCommand(botClient, message, cancellationToken);
            var reply = await (operation
                         ?? throw new ArgumentException("Incorrect command"))
                .Update(
                    botClient,
                    message,
                    cancellationToken);
            await Utilits.AddMessageInfo(message.Chat, message, reply);
        }
        catch (ArgumentException ex)
        {
            var errorMessage = await botClient.SendTextMessageAsync(
                chatId: update.Message!.Chat.Id,
                text: ex.Message,
                cancellationToken: cancellationToken);
            await Utilits.AddMessageInfo(
                update.Message!.Chat,
                update.Message!,
                errorMessage);
        }
        catch(DataException){}
        catch(Exception ex)
        {
            await using var sw = new StreamWriter("Logs.txt");
            await sw.WriteAsync(ex.Message);
            Console.WriteLine(ex.Message);
        }
    }

    public static async Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(JsonSerializer.Serialize<Exception>(exception));
    }
    
}