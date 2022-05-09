using System.Threading.Tasks;
using System.Transactions;
using Discord;
using Discord.WebSocket;
using LostArk.Discord.Bot.Db;
using LostArk.Discord.Bot.Db.Enteties;
using LostArk.Discord.Bot.DomainServices;
using LostArk.Discord.Bot.Infrastructure;
using LostArk.Discord.Bot.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LostArk.Discord.Bot.IntractionServices
{
    public class GuildInteractionService : IInitializable
    {
        private readonly ILogger<GuildInteractionService> _logger;
        private DiscordSocketClient _client;
        private IConfiguration _configuration;
        private readonly IDbContextFactory<BotDbContext> _dbContextFactory;
        private readonly GameSettings _gameSettings;
        private readonly IGuildCommonComponentsProvider _guildCommonComponentsProvider;

        public GuildInteractionService(
            ILogger<GuildInteractionService> logger,
            DiscordSocketClient client,
            IConfiguration configuration,
            IDbContextFactory<BotDbContext> dbContextFactory,
            GameSettings gameSettings,
            IGuildCommonComponentsProvider guildCommonComponentsProvider)
        {
            _logger = logger;
            _client = client;
            _configuration = configuration;
            _dbContextFactory = dbContextFactory;
            _gameSettings = gameSettings;
            _guildCommonComponentsProvider = guildCommonComponentsProvider;
        }

        public async Task InitializeAsync()
        {
            _client.GuildMembersDownloaded+= GuildMembersDownloaded;
            _client.UserJoined += NewGuildMemberJoined;
            _client.InviteCreated += InviteCreated;
        }

        private Task InviteCreated(SocketInvite invite)
        {
            _logger.LogInformation("Invite Created: {Invite}", invite);
            return Task.CompletedTask;
        }

        private async Task NewGuildMemberJoined(SocketGuildUser socketGuildUser)
        {
            _logger.LogInformation("New guild user joined: {User}", socketGuildUser.ToString());
            await using var ctx = await _dbContextFactory.CreateDbContextAsync();
            using var tx = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            await SendWelcomeMessage(socketGuildUser, ctx);
            await ctx.SaveChangesAsync();
            tx.Complete();
        }

        private async Task GuildMembersDownloaded(SocketGuild guild)
        {
            await using var ctx = await _dbContextFactory.CreateDbContextAsync();
            using var tx = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            foreach (var user in guild.Users)
            {
                await SendWelcomeMessage(user, ctx);
            }
            await ctx.SaveChangesAsync();
            tx.Complete();

        }

        private async Task SendWelcomeMessage(SocketGuildUser guildUser, BotDbContext db)
        {
            if (guildUser.IsBot)
                return;
            var guild = guildUser.Guild;
            
            if (await db.GuildMemberWelcomeSentHistory.AnyAsync(x => x.GuildId == guild.Id && x.UserId == guildUser.Id))
                return;

            var welcomeMessage = await _guildCommonComponentsProvider.GetWelcomeComponent(guild.Name, guildUser.Id); 
            await guildUser.SendMessageAsync(
                embed: welcomeMessage.Embed,
                components: welcomeMessage.Components);
            await db.GuildMemberWelcomeSentHistory.AddAsync(new GuildMemberWelcomeSent()
            {
                GuildId = guild.Id,
                UserId = guildUser.Id
            });
        }
        
        
    }
}