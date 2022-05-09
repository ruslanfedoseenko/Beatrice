using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using LostArk.Discord.Bot.Constants;
using LostArk.Discord.Bot.DomainServices;
using Microsoft.Extensions.Logging;

namespace LostArk.Discord.Bot.InteractionModules
{
    public
        class AdminModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILogger<AdminModule> _logger;

        public AdminModule(ILogger<AdminModule> logger)
        {
            _logger = logger;
        }

        [Group("setup", "Setup server via bot")]
        public class SetupGroup : InteractionModuleBase<SocketInteractionContext>
        {
            private readonly IUserSettingsService _userSettingsService;

            public SetupGroup(IUserSettingsService userSettingsService)
            {
                _userSettingsService = userSettingsService;
            }

            [RequireContext(ContextType.Guild)]
            [RequireBotPermission(GuildPermission.ManageEvents | GuildPermission.ManageRoles,
                NotAGuildErrorMessage = "This bot requires permission to manage events and roles")]
            [SlashCommand("start", "Start setup server via bot", runMode: RunMode.Async)]
            public async Task Start()
            {
                var resMan = Resources.Localization.ResourceManager;
                var user = Context.User;
                var userSettings = await _userSettingsService.GetUserSettings(user.Id);
                
                var roles = Context.Guild.Roles;
                var menuBuilder = new SelectMenuBuilder()
                    .WithCustomId(KnownSelects.ChangeAdminRoles)
                    .WithPlaceholder(resMan.GetString(nameof(Resources.Localization.SetupChoseRolePlaceholder), userSettings.Locale))
                    .WithMinValues(1)
                    .WithMaxValues(Math.Min(10, Context.Guild.Roles.Count));
                foreach (var role in roles)
                {
                    menuBuilder.AddOption(new SelectMenuOptionBuilder()
                        .WithLabel(role.Name)
                        .WithValue(role.Id.ToString())
                    );
                }

                var componentsBuilder = new ComponentBuilder()
                    .WithSelectMenu(menuBuilder);

                var embedBuilder = new EmbedBuilder()
                    .WithTitle(resMan.GetString(nameof(Resources.Localization.SetupTitle), userSettings.Locale))
                    .WithDescription(resMan.GetString(nameof(Resources.Localization.SetupDescription), userSettings.Locale))
                    .WithFooter(new EmbedFooterBuilder().WithText(resMan.GetString(nameof(Resources.Localization.SetupChoseRole), userSettings.Locale)))
                    .WithThumbnailUrl(KnownBotParams.BeatriceFaceUrl);

                await Context.Interaction.RespondAsync(embed: embedBuilder.Build(),
                    components: componentsBuilder.Build(), ephemeral: true);
            }
        }

        [RequireContext(ContextType.Guild)]
        [ComponentInteraction(KnownSelects.ChangeAdminRoles)]
        public async Task ChangeBotAdminRoles(ulong[] roleIds)
        {
            var guild = Context.Guild;
            var roles = guild.Roles.Where(x => roleIds.Contains(x.Id));
            var rolesStr = string.Join(", ", roles.Select(x => x.Name));
            _logger.LogInformation("Chosen roles: {Roles}", roleIds);

            await Context.Interaction.RespondAsync(rolesStr, ephemeral: true);

        }
        
    }
}