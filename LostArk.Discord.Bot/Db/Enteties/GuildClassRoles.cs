using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LostArk.Discord.Bot.Db.Enteties
{
    [Index(nameof(GuildId), nameof(RoleId), nameof(ClassId), IsUnique = true, Name = "ix_guild_class_roles_guild_id_role_id")]
    [Table("guild_class_roles", Schema = "beatrice.discord.bot")]
    public class GuildClassRoles
    {
        [Key]
        public long Id { get; set; }
        public long ClassId { get; set; }
        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }
    }
}