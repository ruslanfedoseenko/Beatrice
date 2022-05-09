using System.Text.Json.Serialization;

namespace LostArk.Discord.Bot.Models.Configuration
{
    public partial class Schedule
    {
        [JsonPropertyName("utcOffset")]
        public string UtcOffset { get; set; }

        [JsonPropertyName("recurrence")]
        public PurpleRecurrence Recurrence { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("start")]
        public End Start { get; set; }

        [JsonPropertyName("end")]
        public End End { get; set; }
    }
}