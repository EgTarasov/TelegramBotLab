using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotLab.BotUpdates;

public class Solve: IUpdate
{
    async Task<Message> IUpdate.Update(
        ITelegramBotClient botClient,
        Message request,
        CancellationToken cts)
    {
        Message reply;
        var input = request.Text![6..];
        
        try
        {
            var values = ParseString(input);
            var replyText = Compute(values, input);

            reply = await botClient.SendTextMessageAsync(
                request.Chat.Id,
                text: replyText.ToString(CultureInfo.InvariantCulture),
                cancellationToken: cts);
        }
        catch (DivideByZeroException)
        {
            throw new ArgumentException("Division by zero");
        }
        catch (ArithmeticException)
        {
            throw new ArgumentException("Values are too big");
        }
        catch
        {
            throw new ArgumentException("Can't solve this example");
        }

        return reply;
    }

    public decimal Compute(List<decimal> values, string input)
    {
        var operation = Regex.Match(input, @"[+\-\/*]").Value;
        return operation switch
        {
            "+" => Solver(values[0], values[1], (a, b) => a + b),
            "-" => Solver(values[0], values[1], (a, b) => a - b),
            "*" => Solver(values[0], values[1], (a, b) => a * b),
            "/" => Solver(values[0], values[1], (a, b) => a / b),
            _ => throw new ArgumentException("Invalid operation")
        };
    }

    public List<decimal> ParseString(string input)
    {
        var values = Regex
            .Split(input, @"[+\-*\/]")
            .Select(decimal.Parse)
            .ToList();
        // if (values.Count != 2)
        // {
        //     throw new ArgumentException("Too many values");
        // }

        return values;
    }
    public static decimal Solver(decimal a, decimal b, Func<decimal, decimal, decimal> f) => f(a, b);
}