using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotLab.BotUpdates;

public class Currency: IUpdate
{
    private const string DatePattern = 
        @"<input class=""datepicker-filter_input"" type=""hidden"" name=""UniDbQuery.To"" value=""(\d\d\.\d\d\.\d\d\d\d)"" />";

    private const string RequestWithCurrency = @"/currency ([\w\s]+)";

    private const string CurrencyNumberPattern = @"          <td>{0}</td>
          <td>\w+</td>
          <td>.+</td>
          <td>(.+)</td>
          <td>(.+)</td>";

    private const string CurrencyNamePattern = @"          <td>\d+</td>
          <td>\w+</td>
          <td>.+</td>
          <td>{0}</td>
          <td>(.+)</td>";

    private static IReadOnlySet<string> _currencyNumbers = new HashSet<string>()
    {
        "840", //Доллар США
        "978", //Евро
        "985", //Польский злотый
        "156", //Китайская Юань
    };

    async Task<Message> IUpdate.Update(
        ITelegramBotClient botClient,
        Message request,
        CancellationToken cts)
    {
        var httpClient = new HttpClient();
        var query = await httpClient.GetStringAsync("https://www.cbr.ru/currency_base/daily/", cts);
        
        var info = await GetInfo(request.Text ?? "", query);

        var date = Regex.Match(
                query,
                DatePattern)
            .Groups[1].Value;
        return await botClient.SendTextMessageAsync(
            chatId: request.Chat.Id,
            text: $"Today is {date}{Environment.NewLine}" +
                  $"{string.Join(Environment.NewLine, info)}",
            cancellationToken: cts);
    }

    public async Task<List<string>> GetInfo(string text, string query)
    {
        var info = new List<string>();
        if (Regex.IsMatch(text!, RequestWithCurrency))
        {
            var currencyName = Regex
                .Match(
                    text!,
                    RequestWithCurrency)
                .Groups[1]
                .Value
                .Trim();
            if (currencyName == "all currencies")
            {
                throw new ArgumentException("Че еще хочешь?");
            }
            var match = Regex.Match(query, string.Format(CurrencyNamePattern, currencyName));
            if (match.Success is false)
            {
                throw new ArgumentException("Such currency does not exist");
            }

            info.Add($"{currencyName}: {match.Groups[1].Value}");
        }
        else if (Regex.IsMatch(text, @"^/currency$"))
        {
            foreach (var number in _currencyNumbers)
            {
                var match = Regex.Match(query, string.Format(CurrencyNumberPattern, number));
                info.Add($"{match.Groups[1].Value}: {match.Groups[2].Value}");
            }
        }
        else
        {
            throw new ArgumentException("Incorrect input");
        }

        return info;
    }
}
/*
         var httpClient = new HttpClient();
        var query = await httpClient.GetStringAsync("https://www.cbr.ru/currency_base/daily/", cts);

        var info = new List<string>();
        if (Regex.IsMatch(request.Text!, RequestWithCurrency))
        {
            var currencyName = Regex
                .Match(
                    request.Text!,
                    RequestWithCurrency)
                .Groups[1]
                .Value
                .Trim();
            var match = Regex.Match(query, string.Format(CurrencyNamePattern, currencyName));
            if (match.Success is false)
            {
                throw new ArgumentException("Such currency does not exist");
            }

            info.Add($"{currencyName}: {match.Groups[1].Value}");
        }
        else if (Regex.IsMatch(request.Text!, @"^/currency$"))
        {
            foreach (var number in _currencyNumbers)
            {
                var match = Regex.Match(query, string.Format(CurrencyNumberPattern, number));
                info.Add($"{match.Groups[1].Value}: {match.Groups[2].Value}");
            }
        }
        else
        {
            throw new ArgumentException("Incorrect input");
        }*/
