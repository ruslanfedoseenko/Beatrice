using System.ComponentModel;
using System.Text.Json.Serialization;

namespace LostArk.Discord.Bot.Models.Configuration
{
    public partial class EventSpecial
    {
        [JsonPropertyName("category")]
        public EventCategory Category { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonConverter(typeof(EventRecurringTypeConverter))]
        [JsonPropertyName("type")]
        public EventRecurringType Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("ilvl")]
        public string Ilvl { get; set; }

        [JsonPropertyName("schedule")]
        public LocationSchedule Schedule { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonConverter(typeof(SubCategoryConverter))]
        [JsonPropertyName("subCategory")]
        public SubCategory? SubCategory { get; set; }
    }
}