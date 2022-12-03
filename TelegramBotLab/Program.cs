using System.Text.Json;
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

class Program
{
    static ITelegramBotClient bot = new TelegramBotClient(BotInfo.token);

    static async Task Main(string[] args)
    {
        var me = await bot.GetMeAsync();
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