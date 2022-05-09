using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LostArk.Discord.Bot.Db.Enteties
{
    [Index(nameof(GuildId),nameof(UserId), IsUnique = true, Name = "ix_guild_welcome_history_user_id")]
    [Table("guild_welcome_history", Schema = "beatrice.discord.bot")]
    public class GuildMemberWelcomeSent
    {
        public long Id { get; set; }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
    }
}