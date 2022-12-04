using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotLab;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Update = Telegram.Bot.Types.Update;
using Telegram.Bot.Types.Enums;
using TelegramBotLab.BotUpdates;

class Program
{
    static ITelegramBotClient bot = new TelegramBotClient(BotInfo.token);
    static User me;
    
    static async Task Main(string[] args)
    {
        me = await bot.GetMeAsync();
        Console.WriteLine($"Bot {me.Id}, called {me.FirstName} starts working!");
        
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        
        bot.StartReceiving(
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
        if (update.Message is not { } message)
        {
            return;
        }

        if (message.Text == "/start")
        {
            var msg = await Greeting.GreetMessage(botClient, me, message, cancellationToken);
        }

        if (message.Text == "/get_id")
        {
            await Greeting.GetUserId(botClient, message, cancellationToken);
        }

        if (Regex.IsMatch(message.Text ?? "", @"^/solve\s+\d+\s*[+-/*]\s*\d+\s*$"))
        {
            await Solve.Solver(botClient, message, cancellationToken);
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "I cant understand you",
                cancellationToken: cancellationToken);
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