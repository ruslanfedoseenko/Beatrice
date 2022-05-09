


using System;
using System.ComponentModel;
using System.Globalization;

namespace LostArk.Discord.Bot.Models.Configuration
{
    internal class EventRecurringTypeConverter : TypeConverter
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
                case "recurring":
                    return EventRecurringType.Recurring;
                case "recurringRange":
                    return EventRecurringType.RecurringRange;
            }
            throw new Exception("Cannot unmarshal type EventAbodeOfTheDamnedCategory");
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var casted = value is EventRecurringType category ? category : EventRecurringType.Recurring;
            switch (casted)
            {
                case EventRecurringType.Recurring:
                    return "recurring";
                case EventRecurringType.RecurringRange:
                    return "recurringRange";
            }
            throw new Exception("Cannot marshal type EventAbodeOfTheDamnedCategory");
        }
        /*public override bool CanConvert(Type t) => t == typeof(EventRecurringType) || t == typeof(EventRecurringType?);
        public override EventRecurringType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == "recurring")
            {
                return EventRecurringType.Recurring;
            }

            if (value == "recurringRange")
            {
                return EventRecurringType.RecurringRange;
            }
            throw new Exception("Cannot unmarshal type EventAbodeOfTheDamnedType");
        }

        public override void Write(Utf8JsonWriter writer, EventRecurringType value, JsonSerializerOptions options)
        {
            
            if (value == EventRecurringType.Recurring)
            {
                writer.WriteStringValue("recurringRange");
                return;
            }

            if (value == EventRecurringType.RecurringRange)
            {
                writer.WriteStringValue("recurring");
                return;
            }
            throw new Exception("Cannot marshal type EventAbodeOfTheDamnedType");
        }
        */

        public static readonly EventRecurringTypeConverter Singleton = new EventRecurringTypeConverter();
    }
}