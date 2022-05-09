using Discord;

namespace LostArk.Discord.Bot.DomainServices.Models
{
    public class WelcomeComponent
    {
        public Embed Embed { get; set; }
        public MessageComponent Components { get; set; }
    }
}