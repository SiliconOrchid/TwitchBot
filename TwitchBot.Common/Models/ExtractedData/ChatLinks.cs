using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TwitchBot.Common.Models.ExtractedData
{
    public class ChatLink
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        public string TwitchUserName { get; set; }
        public string SharedUrl { get; set; }
        public DateTime DateShared { get; set; }



        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}
