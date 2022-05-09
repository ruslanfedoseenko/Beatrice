using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LostArk.Discord.Bot.Db.Enteties
{
    [Index(nameof(UserId), IsUnique = true, Name = "ix_user_settings_user_id")]
    [Table("user_settings", Schema = "beatrice.discord.bot")]
    public class UserSettings
    {
        [Key]
        public long Id { get; set; }

        
        public ulong UserId { get; set; }
        public string Locale { get; set; }
        public TimeSpan Offset { get; set; }
    }
}