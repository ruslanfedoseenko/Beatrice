using System.Collections.Generic;

namespace LostArk.Discord.Bot.Models.Configuration
{
    public class GameSettings
    {
        public List<ClassInfo> Classes { get; set; }
        public Events Events { get; set; }
    }
}