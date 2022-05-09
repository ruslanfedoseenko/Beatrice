using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;



namespace LostArk.Discord.Bot.Models.Configuration
{
    internal class SubCategoryConverter : TypeConverter
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
                case "gate":
                    return SubCategory.Gate;
                case "sailing coop":
                    return SubCategory.SailingCoop;
                case "ghost ship":
                    return SubCategory.GhostShip;
            }
            throw new Exception("Cannot unmarshal type EventAbodeOfTheDamnedCategory");
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var casted = value is SubCategory category ? category : SubCategory.Gate;
            switch (casted)
            {
              
                case SubCategory.Gate:
                    return "gate";
                case SubCategory.SailingCoop:
                    return "sailing coop";
                case SubCategory.GhostShip:
                    return "ghost ship";
            }
            throw new Exception("Cannot marshal type EventAbodeOfTheDamnedCategory");
        }
        /*
        public override bool CanConvert(Type t) => t == typeof(SubCategory) || t == typeof(SubCategory?);
        public override SubCategory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            
            var value = reader.GetString();
            switch (value)
            {
                case "gate":
                    return SubCategory.Gate;
                case "sailing coop":
                    return SubCategory.SailingCoop;
                case "ghost ship":
                    return SubCategory.GhostShip;
            }
            throw new Exception("Cannot unmarshal type SubCategory");
        }

        public override void Write(Utf8JsonWriter writer, SubCategory value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case SubCategory.Gate:
                    writer.WriteStringValue("gate");
                    return;
                case SubCategory.SailingCoop:
                    writer.WriteStringValue("sailing coop");
                    return;
                case SubCategory.GhostShip:
                    writer.WriteStringValue("ghost ship");
                    return;
            }
            throw new Exception("Cannot marshal type SubCategory");
        }*/

        public static readonly SubCategoryConverter Singleton = new SubCategoryConverter();
    }
}