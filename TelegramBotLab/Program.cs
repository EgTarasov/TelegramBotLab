
using Telegram.Bot;

var botClient = new TelegramBotClient(token:"sd2");

var me = await botClient.GetMeAsync();
Console.WriteLine(me.Id);