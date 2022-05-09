using System.Globalization;
using System.Threading.Tasks;
using Discord;
using LostArk.Discord.Bot.Constants;
using LostArk.Discord.Bot.DomainServices.Models;

namespace LostArk.Discord.Bot.DomainServices
{
    public class GuildCommonComponentsProvider : IGuildCommonComponentsProvider
    {
       
        private readonly IUserSettingsService _userSettingsService;

        public GuildCommonComponentsProvider(IUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService;
        }

        public async Task<WelcomeComponent> GetWelcomeComponent(string guildName, ulong userId)
        {
            var resMan = Resources.Localization.ResourceManager;

            var userSettings = await _userSettingsService.GetUserSettings(userId);
            var culture = userSettings.Locale;
            var componentsBuilder = new ComponentBuilder()
                .WithButton(resMan.GetString(nameof(Resources.Localization.AddCharacter), culture),
                    KnownButtons.AddCharacter)
                .WithButton(resMan.GetString(nameof(Resources.Localization.EditCharacter), culture),
                    KnownButtons.EditCharacter, ButtonStyle.Secondary)
                .WithButton(resMan.GetString(nameof(Resources.Localization.DeleteCharacter), culture),
                    KnownButtons.DeleteCharacter, ButtonStyle.Danger)
                .WithButton(resMan.GetString(nameof(Resources.Localization.Settings), culture), KnownButtons.Settings,
                    ButtonStyle.Success);


            var welcomeFormatMessage = resMan.GetString(
                nameof(Resources.Localization.RegisterMessage), culture);
            var welcomeMessage = string.Format(welcomeFormatMessage, guildName);
            var embedBuilder = new EmbedBuilder()
                .WithTitle("")
                .WithDescription(welcomeMessage)
                .WithThumbnailUrl(KnownBotParams.BeatriceFaceUrl);

            return new WelcomeComponent
            {
                Embed = embedBuilder.Build(),
                Components = componentsBuilder.Build()
            };
        }
    }

    public interface IGuildCommonComponentsProvider
    {
        Task<WelcomeComponent> GetWelcomeComponent(string guildName, ulong userId);
    }
}