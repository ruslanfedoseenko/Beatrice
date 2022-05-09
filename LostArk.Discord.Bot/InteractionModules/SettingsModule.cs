using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using LostArk.Discord.Bot.Constants;
using LostArk.Discord.Bot.DomainServices;
using Microsoft.Extensions.Logging;

namespace LostArk.Discord.Bot.InteractionModules
{
    public class SettingsModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILogger<SettingsModule> _logger;
        private readonly IUserSettingsService _settingsService;

        public SettingsModule(
            ILogger<SettingsModule> logger,
            IUserSettingsService settingsService)
        {
            _logger = logger;
            _settingsService = settingsService;
        }


        [ComponentInteraction(KnownButtons.Settings)]
        public async Task ListSettings()
        {
            var user = Context.User;
            var settings = await _settingsService.GetUserSettings(user.Id);
            var resMan = Resources.Localization.ResourceManager;

            var embedBuilder = new EmbedBuilder();

            embedBuilder.AddField(
                new EmbedFieldBuilder().WithName(resMan.GetString(nameof(Resources.Localization.Language),
                    settings.Locale)).WithValue(settings.Locale.NativeName));

            embedBuilder.AddField(
                new EmbedFieldBuilder().WithName(resMan.GetString(nameof(Resources.Localization.Timezone),
                    settings.Locale)).WithValue((settings.TimeZone < TimeSpan.Zero ? "-" : "+") +
                                                settings.TimeZone.ToString(@"dd\:hh")));

            var componentBuilder = new ComponentBuilder()
                .WithButton(new ButtonBuilder()
                    .WithLabel(resMan.GetString(nameof(Resources.Localization.ChangeLanguage), settings.Locale))
                    .WithStyle(ButtonStyle.Primary)
                    .WithCustomId(KnownButtons.ShowLanguageList)
                )
                .WithButton(new ButtonBuilder()
                    .WithLabel(resMan.GetString(nameof(Resources.Localization.ChangeTimezone), settings.Locale))
                    .WithStyle(ButtonStyle.Primary)
                    .WithCustomId(KnownButtons.ShowTimezoneList)
                );

            await Context.Interaction.RespondAsync(embed: embedBuilder.Build(), components: componentBuilder.Build(), ephemeral: true);
        }

        [ComponentInteraction(KnownButtons.ShowLanguageList)]
        public async Task ShowLanguageList()
        {
            var user = Context.User;
            var settings = await _settingsService.GetUserSettings(user.Id);
            var locales = _settingsService.GetSupportedLocales();

            var componentBuilder = new ComponentBuilder()
                .WithSelectMenu(new SelectMenuBuilder()
                    .WithCustomId(KnownSelects.ChangeLanguage)
                    .WithOptions(locales.Select(x => new SelectMenuOptionBuilder()
                        .WithEmote(Emoji.Parse($":flag_{GetCountryCode(x.IetfLanguageTag)}:"))
                        .WithLabel(x.NativeName)
                        .WithValue(x.IetfLanguageTag)
                        .WithDefault(x.Name == settings.Locale.Name)
                    ).ToList())
                    
                );

            await Context.Interaction.RespondAsync(components: componentBuilder.Build(), ephemeral: true);
        }
        
        private string GetCountryCode(string ietfLanguageTag)
        {
            var parts = ietfLanguageTag.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var code = parts.Last();
            return code.ToLower();
        }

        [ComponentInteraction(KnownSelects.ChangeLanguage)]
        public async Task ChangeLanguage(string localeCode)
        {
            var user = Context.User;
            var settings = await _settingsService.GetUserSettings(user.Id);
            var resMan = Resources.Localization.ResourceManager;

            var locale = settings.Locale = new CultureInfo(localeCode);
            await _settingsService.UpdateSettings(user.Id, settings);
            
            await Context.Interaction.RespondAsync(resMan.GetString(nameof(Resources.Localization.LanguageChanged), locale) , ephemeral: true);
            
        }

       
    }
}