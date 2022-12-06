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
    static User _me = null!;
    
    static async Task Main(string[] args)
    {
        _me = await Bot.GetMeAsync();
        Console.WriteLine($"Bot {_me.Id}, called {_me.FirstName} starts working!");
        
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

            if ((message.From ?? _me).IsBot is true)
            {
                return;
            }

            var reply = new Message();
            if (message.Text == "/start")
            {
                reply = await Greeting.GreetMessage(botClient, _me, message, cancellationToken);
            }

            else if (message.Text == "/get_id")
            {
                await Greeting.GetUserId(botClient, message, cancellationToken);
            }

            else if (Regex.IsMatch(message.Text ?? "", @"^/solve\s+(?:\d+\.?\d*)\s*[+\-\/*]\s*(?:\d+\.?\d*)\s*$"))
            {
                reply = await Solve.Solver(botClient, message, cancellationToken);
            }
            else if (message.Text!.StartsWith("/currency"))
            {
                reply = await Currency.GetCurrencyInfo(botClient, message, cancellationToken);
            }
            else if (Regex.IsMatch(message.Text, $".*@{_me.Username}.*"))
            {
                reply = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "What do you need?",
                    cancellationToken: cancellationToken);
            }

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
        catch(Exception ex)
        {
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