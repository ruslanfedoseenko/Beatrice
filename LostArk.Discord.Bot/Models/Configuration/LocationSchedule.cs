using System.Text.Json.Serialization;

namespace LostArk.Discord.Bot.Models.Configuration
{
    public partial class LocationSchedule
    {
        [JsonPropertyName("ru")]
        public Schedule Ru { get; set; }
    }
}