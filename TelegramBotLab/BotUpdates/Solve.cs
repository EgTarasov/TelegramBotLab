using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotLab.BotUpdates;

public class Solve
{
    public static async Task<Message> Solver(ITelegramBotClient botClient, Message request, CancellationToken cts)
    {
        var reply = new Message();
        var input = request.Text![6..];

        var values = Regex
            .Split(input, @"[+\-*\/]")
            .Select(decimal.Parse)
            .ToList();
        if (values.Count != 2)
        {
            throw new ArgumentException("Too many values");
        }

        try
        {
            var operation = Regex.Match(input, @"[+\-\/*]").Value;
            var replyText = operation switch
            {
                "+" => Solver(values[0], values[1], (a, b) => a + b).ToString(CultureInfo.InvariantCulture),
                "-" => Solver(values[0], values[1], (a, b) => a - b).ToString(CultureInfo.InvariantCulture),
                "*" => Solver(values[0], values[1], (a, b) => a * b).ToString(CultureInfo.InvariantCulture),
                "/" => Solver(values[0], values[1], (a, b) => a / b).ToString(CultureInfo.InvariantCulture),
            };
            Console.WriteLine(replyText);

            reply = await botClient.SendTextMessageAsync(
                request.Chat.Id,
                text: replyText,
                cancellationToken: cts);
        }
        catch
        {
            throw new ArgumentException("Can't solve this example");
        }

        return reply;
    }

    private static decimal Solver(decimal a, decimal b, Func<decimal, decimal, decimal> f) => f(a, b);
}