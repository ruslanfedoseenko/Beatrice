using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LostArk.Discord.Bot.Db;
using LostArk.Discord.Bot.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LostArk.Discord.Bot.IntractionServices
{
    public class Bootstrap : IInitializable
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Bootstrap> _logger;
        private readonly IEnumerable<IInitializable> _services;
        private readonly IDbContextFactory<BotDbContext> _dbContextFactory;

        public Bootstrap(
            DiscordSocketClient discordSocketClient,
            IConfiguration configuration, 
            ILogger<Bootstrap> logger, 
            IEnumerable<IInitializable> services, 
            IDbContextFactory<BotDbContext> dbContextFactory)
        {
            _discordSocketClient = discordSocketClient;
            _configuration = configuration;
            _logger = logger;
            _services = services;
            _dbContextFactory = dbContextFactory;
        }

        public async Task InitializeAsync()
        {
            try
            {
                {
                    await using var db = await _dbContextFactory.CreateDbContextAsync();
                    await db.Database.MigrateAsync();
                }
                var client = _discordSocketClient;

                client.Log += LogAsync;

                _logger.LogInformation("Initializing services....");
                // Here we can initialize the service that will register and execute our commands
                await Task.WhenAll(_services.Select(x => x.InitializeAsync()));
            
            
                // Bot token can be provided from the Configuration object we set up earlier
                var token = _configuration["token"];
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
                await client.SetActivityAsync(new Game("Lost Ark", flags: ActivityProperties.Join, details: "Скучает без тебя в Тризионе"));

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Boostrap InitializeAsync Error");
            }
            
            
        }
        private async Task LogAsync(LogMessage message)
            => Log.Logger.Write(message.ToLogEvent());
    }
}