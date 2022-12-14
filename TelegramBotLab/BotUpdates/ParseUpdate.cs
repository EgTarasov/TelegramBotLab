using System.Data;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotLab.BotUpdates;

public static class ParseUpdate
{
    public static async Task<IUpdate?> GetCommand(
        ITelegramBotClient botClient,
        Message request,
        CancellationToken cancellationToken)
    {
        var me = await botClient.GetMeAsync(cancellationToken);
        if (request.Text is null)
        {
            return null;
        }

        if (request.Text == "/help")
        {
            return new Utilits();
        }
        if (request.Text == "/start")
        {

            return new Greeting();
        }

        if (request.Text == "/get_id")
        {
            return new UserInfo();
        }

        if (Regex.IsMatch(request.Text ?? "", @"^/solve\s*(?:\d+\.?\d*)\s*[+\-\/*]\s*(?:\d+\.?\d*)\s*$"))
        {
            return new Solve();
        }
        
        if (request.Text!.StartsWith("/currency"))
        {
            return new Currency();
        }
        
        if (Regex.IsMatch(request.Text, $".*@{me.Username}.*"))
        {
            await botClient.SendTextMessageAsync(
                chatId: request.Chat.Id,
                text: "What do you need?",
                cancellationToken: cancellationToken);
            throw new DataException();
        }

        return null;
    }
}