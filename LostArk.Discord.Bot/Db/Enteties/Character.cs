using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LostArk.Discord.Bot.Db.Enteties
{
    [Index(nameof(UserId), Name = "ix_character_user_id")]
    [Table("character", Schema = "beatrice.discord.bot")]
    public class Character
    {
        [Key]
        public long Id { get; set; }
        public ulong UserId { get; set; }
        public int ClassId { get; set; }
        public string Name { get; set; }
        public Decimal GearScore { get; set; }
    }
}