using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LostArk.Discord.Bot.Db.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ix_guild_welcome_history_user_id",
                schema: "beatrice.discord.bot",
                table: "guild_welcome_history",
                newName: "ix_guild_welcome_history_guild_id_user_id");

            migrationBuilder.CreateTable(
                name: "guild_class_roles",
                schema: "beatrice.discord.bot",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassId = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guild_class_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "guild_roles",
                schema: "beatrice.discord.bot",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleType = table.Column<int>(type: "integer", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    RoleId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guild_roles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_guild_class_roles_guild_id_role_id",
                schema: "beatrice.discord.bot",
                table: "guild_class_roles",
                columns: new[] { "GuildId", "RoleId", "ClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_guild_roles_guild_id_role_id",
                schema: "beatrice.discord.bot",
                table: "guild_roles",
                columns: new[] { "GuildId", "RoleId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guild_class_roles",
                schema: "beatrice.discord.bot");

            migrationBuilder.DropTable(
                name: "guild_roles",
                schema: "beatrice.discord.bot");

            migrationBuilder.RenameIndex(
                name: "ix_guild_welcome_history_guild_id_user_id",
                schema: "beatrice.discord.bot",
                table: "guild_welcome_history",
                newName: "ix_guild_welcome_history_user_id");
        }
    }
}
