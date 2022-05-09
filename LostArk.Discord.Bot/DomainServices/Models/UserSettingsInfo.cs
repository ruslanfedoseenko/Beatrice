using System;
using System.Globalization;

namespace LostArk.Discord.Bot.DomainServices.Models
{
    public class UserSettingsInfo
    {
        public CultureInfo Locale { get; set; }
        public TimeSpan TimeZone { get; set; }
    }
}