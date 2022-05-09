using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LostArk.Discord.Bot.Db.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "guild_welcome_history",
                schema: "beatrice.discord.bot",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guild_welcome_history", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_settings",
                schema: "beatrice.discord.bot",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Locale = table.Column<string>(type: "text", nullable: false),
                    Offset = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_settings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_guild_welcome_history_user_id",
                schema: "beatrice.discord.bot",
                table: "guild_welcome_history",
                columns: new[] { "GuildId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_settings_user_id",
                schema: "beatrice.discord.bot",
                table: "user_settings",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guild_welcome_history",
                schema: "beatrice.discord.bot");

            migrationBuilder.DropTable(
                name: "user_settings",
                schema: "beatrice.discord.bot");
        }
    }
}
