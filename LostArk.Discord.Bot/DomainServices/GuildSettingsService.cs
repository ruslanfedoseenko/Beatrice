using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Discord;
using Discord.WebSocket;
using LostArk.Discord.Bot.Db;
using LostArk.Discord.Bot.Db.Enteties;
using LostArk.Discord.Bot.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LostArk.Discord.Bot.DomainServices
{
    public class GuildSettingsService : IGuildSettingsService
    {
        private readonly ILogger<GuildSettingsService> _logger;
        private readonly IDbContextFactory<BotDbContext> _dbContextFactory;
        private readonly GameSettings _gameSettings;

        public GuildSettingsService(
            ILogger<GuildSettingsService> logger,
            IDbContextFactory<BotDbContext> dbContextFactory,
            GameSettings gameSettings)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _gameSettings = gameSettings;
        }


        public async Task<ulong[]> GetAdminRoles(ulong guildId)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            var existingRoles = await db.GuildRoles.Where(x => x.GuildId == guildId && x.RoleType == RoleType.BotAdmin)
                .Select(x => x.RoleId)
                .ToArrayAsync();
            return existingRoles;
        }

        public async Task UpdateAdminRoles(ulong guildId, ulong[] roleIds)
        {
            await MergeGuildRoles(guildId, roleIds, RoleType.BotAdmin);
        }
        
        public async Task UpdateManagerRoles(ulong guildId, ulong[] roleIds)
        {
            await MergeGuildRoles(guildId, roleIds, RoleType.BotManager);
        }

        public async Task CreateClassRoles(SocketGuild guild, CultureInfo locale)
        {
            var resMan = Resources.Localization.ResourceManager;
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            using var tx = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            foreach (var classInfo in _gameSettings.Classes)
            {
                var roleName = resMan.GetString(classInfo.Name, locale);
                var existingRole = guild.Roles.FirstOrDefault(x => x.Name == roleName);
                if (existingRole == null)
                {
                    var role = await guild.CreateRoleAsync(roleName);
                    db.GuildClassRoles.Add(new GuildClassRoles()
                    {
                        GuildId = guild.Id,
                        RoleId = role.Id,
                        ClassId = classInfo.Id
                    });    
                }
                else
                {
                    db.GuildClassRoles.Add(new GuildClassRoles()
                    {
                        GuildId = guild.Id,
                        RoleId = existingRole.Id,
                        ClassId = classInfo.Id
                    });
                }
                
                
            }

            await db.SaveChangesAsync();
            tx.Complete();
        }

        private async Task MergeGuildRoles(ulong guildId, ulong[] roleIds, RoleType roleType)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            using var tx = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            var existingRoles = await db.GuildRoles.Where(x => x.GuildId == guildId && x.RoleType == roleType)
                .ToArrayAsync();

            var rolesToAdd = roleIds.Where(x => existingRoles.All(y => y.RoleId != x));
            var rolesToRemove = existingRoles.Where(x => roleIds.All(y => y != x.RoleId));
            var rolesToUpdate = existingRoles.Where(x => roleIds.Any(y => y == x.RoleId));

            foreach (var roleId in rolesToAdd)
            {
                db.GuildRoles.Add(new GuildRoles()
                {
                    RoleType = roleType,
                    GuildId = guildId,
                    RoleId = roleId
                });
            }

            db.GuildRoles.RemoveRange(rolesToRemove);

            foreach (var role in rolesToUpdate)
            {
                role.RoleType = roleType;
            }

            await db.SaveChangesAsync();
            tx.Complete();
        }
    }

    public interface IGuildSettingsService
    {
        Task<ulong[]> GetAdminRoles(ulong guildId);
        Task UpdateAdminRoles(ulong guildId, ulong[] roleIds);
        Task UpdateManagerRoles(ulong guildId, ulong[] roleIds);
        Task CreateClassRoles(SocketGuild guild, CultureInfo locale);
    }
}