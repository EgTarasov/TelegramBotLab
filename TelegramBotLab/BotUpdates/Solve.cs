using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotLab.BotUpdates;

public class Solve
{
    public static async Task<Message> Solver(ITelegramBotClient botClient, Message response, CancellationToken cts)
    {
        var reply = new Message();
        try
        {
            var input = response.Text![6..];
            if (input.Contains("+"))
            {
                var values = input.Split("+");
                if (values.Length != 2)
                {
                    throw new ArgumentException();
                }

                reply = await botClient.SendTextMessageAsync(
                    response.Chat.Id,
                    text: Solver(int.Parse(values[0]), int.Parse(values[1]), (a, b) => a + b).ToString(),
                    cancellationToken: cts);
            }

            else if (input.Contains("-"))
            {
                var values = input.Split("-");
                if (values.Length != 2)
                {
                    throw new ArgumentException();
                }

                reply = await botClient.SendTextMessageAsync(
                    response.Chat.Id,
                    text: Solver(int.Parse(values[0]), int.Parse(values[1]), (a, b) => a - b).ToString(),
                    cancellationToken: cts);
            }

            else if (input.Contains("*"))
            {
                var values = input.Split("*");

                if (values.Length != 2)
                {
                    throw new ArgumentException();
                }

                reply = await botClient.SendTextMessageAsync(
                    response.Chat.Id,
                    text: Solver(int.Parse(values[0]), int.Parse(values[1]), (a, b) => a * b).ToString(),
                    cancellationToken: cts);
            }

            else if (input.Contains("/"))
            {
                var values = input.Split("/");

                if (values.Length != 2)
                {
                    throw new ArgumentException();
                }

                reply = await botClient.SendTextMessageAsync(
                    response.Chat.Id,
                    text: Solver(int.Parse(values[0]), int.Parse(values[1]), (a, b) => a / b).ToString(),
                    cancellationToken: cts);
            }
        }
        catch (Exception)
        {
            throw new ArgumentException();
        }

        await Utilits.AddMessageInfo(response.Chat, response, reply);

        return reply;
    }

    private static int Solver(int a, int b, Func<int, int, int> f) => f(a, b);
}