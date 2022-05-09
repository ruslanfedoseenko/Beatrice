using LostArk.Discord.Bot.Db.Enteties;
using Microsoft.EntityFrameworkCore;

namespace LostArk.Discord.Bot.Db
{
    public class BotDbContext : DbContext
    {
        public BotDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<GuildMemberWelcomeSent> GuildMemberWelcomeSentHistory { get; set; }
    }
}