using System.ComponentModel;

namespace LostArk.Discord.Bot.Models.Configuration
{
    [TypeConverter(typeof(EventCategoryConverter))]
    public enum EventCategory { GvGPvP, Island, Special, Voyage, ChaosGate, FieldBoss, GhostShip };
}