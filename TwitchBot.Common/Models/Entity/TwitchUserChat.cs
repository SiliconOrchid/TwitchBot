namespace TwitchBot.Common.Models.Entity
{
    public class TwitchUserChat : EntityBase
    {
        public string TwitchUserId { get; set; }

        public string TwitchUserType { get; set; }

        public string TwitchUserDisplayName { get; set; }

        public string ChatMessage { get; set; }

        public string ExtractedUrl { get; set; }

        public string LuisIntent { get; set; }

        public decimal? LuisCertainty { get; set; }

    }
}
