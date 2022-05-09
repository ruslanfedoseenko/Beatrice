using System.ComponentModel;

namespace LostArk.Discord.Bot.Models.Configuration
{
    [TypeConverter(typeof(SubCategoryConverter))]
    public enum SubCategory { Gate, SailingCoop, GhostShip };
}