using TwitchBot.Common.Models.Chat;

namespace TwitchBot.Agent.Services.Interfaces
{
    public interface IChatMessageService
    {
        string HandleBotCommands(string chatMessage);
        string MapLuisIntentToResponse(IntentResponse intentResponse);

    }
}
