using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LostArk.Discord.Bot.Db.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "beatrice.discord.bot");

            migrationBuilder.CreateTable(
                name: "character",
                schema: "beatrice.discord.bot",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ClassId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    GearScore = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_character_user_id",
                schema: "beatrice.discord.bot",
                table: "character",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character",
                schema: "beatrice.discord.bot");
        }
    }
}
