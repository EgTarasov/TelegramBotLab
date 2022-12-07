using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotLab.BotUpdates;

public class UserInfo: IUpdate
{
    private string GetUserId(Message request) => $"Your ID is {request.Chat.Id}";
    
    async Task<Message> IUpdate.Update(
        ITelegramBotClient botClient,
        Message request,
        CancellationToken cts)
    {
        
        var userInfo = new List<string>();
        //Info about user
        userInfo.Add(GetUserId(request));

        var reply = await botClient.SendTextMessageAsync(
            chatId: request.Chat.Id,
            text: string.Join(Environment.NewLine, userInfo),
            cancellationToken: cts
        );
        return reply;
    }
}