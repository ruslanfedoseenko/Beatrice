using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LostArk.Discord.Bot.Db.Enteties
{
    public enum RoleType
    {
        BotAdmin,
        BotManager
    }
    [Index(nameof(GuildId),nameof(RoleId), IsUnique = true, Name = "ix_guild_roles_guild_id_role_id")]
    [Table("guild_roles", Schema = "beatrice.discord.bot")]
    public class GuildRoles
    {
        [Key]
        public long Id { get; set; }

        public RoleType RoleType { get; set; }

        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }
    }
}