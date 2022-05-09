﻿// <auto-generated />
using System;
using LostArk.Discord.Bot.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LostArk.Discord.Bot.Db.Migrations
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20220508194647_v1")]
    partial class v1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LostArk.Discord.Bot.Db.Enteties.Character", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("ClassId")
                        .HasColumnType("integer");

                    b.Property<decimal>("GearScore")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "UserId" }, "ix_character_user_id");

                    b.ToTable("character", "beatrice.discord.bot");
                });

            modelBuilder.Entity("LostArk.Discord.Bot.Db.Enteties.GuildMemberWelcomeSent", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "GuildId", "UserId" }, "ix_guild_welcome_history_user_id")
                        .IsUnique();

                    b.ToTable("guild_welcome_history", "beatrice.discord.bot");
                });

            modelBuilder.Entity("LostArk.Discord.Bot.Db.Enteties.UserSettings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<TimeSpan>("Offset")
                        .HasColumnType("interval");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "UserId" }, "ix_user_settings_user_id")
                        .IsUnique();

                    b.ToTable("user_settings", "beatrice.discord.bot");
                });
#pragma warning restore 612, 618
        }
    }
}
