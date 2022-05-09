using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace LostArk.Discord.Bot.Models.Configuration
{
    public partial class Events
    {
        [JsonPropertyName("ids")]
        public List<string> Ids { get; set; }

        [JsonPropertyName("entities")]
        public Dictionary<string, EventSpecial> Entities { get; set; }
    }
}