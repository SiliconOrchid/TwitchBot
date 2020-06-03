namespace TwitchBot.Common.Models.Chat
{
    public class IntentResponse
    {
        public string Intent {get;set;}
        public decimal Certainty {get;set;}

        public string EmbeddedUrl { get; set; } // if a user's message contains a URL, this will be extracted and included in the response (if one is available, otherwise this is left blank)
    }

}