using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading.Tasks;
using LostArk.Discord.Bot.Db;
using LostArk.Discord.Bot.Db.Enteties;
using LostArk.Discord.Bot.DomainServices.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LostArk.Discord.Bot.DomainServices
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly ILogger<UserSettingsService> _logger;
        private readonly IDbContextFactory<BotDbContext> _dbContextFactory;
        private readonly RequestLocalizationOptions _localizationOptions;

        public UserSettingsService(ILogger<UserSettingsService> logger, IDbContextFactory<BotDbContext> dbContextFactory, IOptions<RequestLocalizationOptions> localizationOptions)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _localizationOptions = localizationOptions.Value;
        }

        public IEnumerable<CultureInfo> GetSupportedLocales()
        {
            if (_localizationOptions.SupportedUICultures != null) return _localizationOptions.SupportedUICultures;
            return ImmutableArray<CultureInfo>.Empty;
        }

        public async Task<UserSettingsInfo> GetUserSettings(ulong userId)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            var userSettings = await db.UserSettings.FirstOrDefaultAsync(x => x.UserId == userId);

            if (userSettings != null)
            {
                return new UserSettingsInfo()
                {
                    Locale = new CultureInfo(userSettings.Locale),
                    TimeZone = userSettings.Offset
                };
            }

            return new UserSettingsInfo()
            {
                Locale = new CultureInfo("ru-RU"),
                TimeZone = TimeSpan.FromHours(-3)
            };
        }

        public async Task UpdateSettings(ulong userId, UserSettingsInfo settings)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            var dbSettings = await db.UserSettings.FirstOrDefaultAsync(x => x.UserId == userId);

            if (dbSettings == null)
            {
                dbSettings = new UserSettings()
                {
                    UserId = userId
                };
                db.Attach(dbSettings);
            }
            
            dbSettings.Locale = settings.Locale.IetfLanguageTag;
            dbSettings.Offset = settings.TimeZone;
            await db.SaveChangesAsync();
        }
    }

    public interface IUserSettingsService
    {
        IEnumerable<CultureInfo> GetSupportedLocales();
        Task<UserSettingsInfo> GetUserSettings(ulong userId);
        Task UpdateSettings(ulong userId, UserSettingsInfo settings);
    }
}