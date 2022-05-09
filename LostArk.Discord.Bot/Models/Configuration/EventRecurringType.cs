using System.ComponentModel;

namespace LostArk.Discord.Bot.Models.Configuration
{
    [TypeConverter(typeof(EventRecurringTypeConverter))]
    public enum EventRecurringType { Recurring, RecurringRange };
}