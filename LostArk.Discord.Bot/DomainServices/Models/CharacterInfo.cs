using System;
using LostArk.Discord.Bot.Models.Configuration;

namespace LostArk.Discord.Bot.DomainServices.Models
{
    public class CharacterInfo
    {
        public long Id { get; set; }
        public ClassInfo? Class { get; set; }
        public string Name { get; set; }
        public Decimal GearScore { get; set; }
    }
}