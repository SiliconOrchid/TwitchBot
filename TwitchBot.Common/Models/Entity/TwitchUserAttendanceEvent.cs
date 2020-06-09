namespace TwitchBot.Common.Models.Entity
{
    public class TwitchUserAttendanceEvent : EntityBase
    {
        public string TwitchUserDisplayName { get; set; }

        public bool UserJoined { get; set; }

        public bool UserLeft { get; set; }
    }
}
