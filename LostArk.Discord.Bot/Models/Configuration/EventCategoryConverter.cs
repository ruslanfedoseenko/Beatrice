using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;


namespace LostArk.Discord.Bot.Models.Configuration
{
    internal class EventCategoryConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var casted = value as string;
            switch (casted)
            {
                case "GvG & PvP":
                    return EventCategory.GvGPvP;
                case "island":
                    return EventCategory.Island;
                case "special":
                    return EventCategory.Special;
                case "voyage":
                    return EventCategory.Voyage;
                case "chaos gate":
                    return EventCategory.ChaosGate;
                case "field boss":
                    return EventCategory.FieldBoss;
                case "ghost ship":
                    return EventCategory.GhostShip;
            }
            throw new Exception("Cannot unmarshal type EventAbodeOfTheDamnedCategory");
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var casted = value is EventCategory category ? category : EventCategory.GvGPvP;
            switch (casted)
            {
                case EventCategory.GvGPvP:
                    return "GvG & PvP";
                case EventCategory.Island:
                    return "island";
                case EventCategory.Special:
                    return "special";
                case EventCategory.Voyage:
                    return "voyage";
                case EventCategory.ChaosGate:
                    return "chaos gate";
                case EventCategory.FieldBoss:
                    return "field boss";
                case EventCategory.GhostShip:
                    return "ghost ship";
            }
            throw new Exception("Cannot marshal type EventAbodeOfTheDamnedCategory");
        }
    }
}