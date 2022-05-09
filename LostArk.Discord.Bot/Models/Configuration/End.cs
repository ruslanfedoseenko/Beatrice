using System.Text.Json.Serialization;

namespace LostArk.Discord.Bot.Models.Configuration
{
    public partial class End
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }
    }
}