using TwitchBot.Common.Models.Chat;

namespace TwitchBot.Agent.Services.Interfaces
{
    public interface IChatMessageService
    {
        string HandleBotCommands(TwitchLib.Client.Events.OnMessageReceivedArgs e);
        string MapLuisIntentToResponse(IntentResponse intentResponse);

    }
}
