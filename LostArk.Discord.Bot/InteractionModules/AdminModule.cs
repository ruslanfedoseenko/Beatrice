using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArk.Discord.Bot.Constants;
using LostArk.Discord.Bot.DomainServices;
using Microsoft.Extensions.Logging;

namespace LostArk.Discord.Bot.InteractionModules
{
    public
        class AdminModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILogger<AdminModule> _logger;
        private readonly IGuildSettingsService _guildSettingsService;
        private readonly IUserSettingsService _userSettingsService;

        public AdminModule(ILogger<AdminModule> logger, IGuildSettingsService guildSettingsService, IUserSettingsService userSettingsService)
        {
            _logger = logger;
            _guildSettingsService = guildSettingsService;
            _userSettingsService = userSettingsService;
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
            var resMan = Resources.Localization.ResourceManager;
            var guild = Context.Guild;
            var roles = guild.Roles.Where(x => roleIds.Contains(x.Id));
            var rolesStr = string.Join(", ", roles.Select(x => x.Name));


            var guildAdminRoles = await _guildSettingsService.GetAdminRoles(guild.Id);

            EmbedBuilder? embedBuilder;
            var userSettings = await _userSettingsService.GetUserSettings(Context.User.Id);
            if (Context.User is SocketGuildUser user)
            {
                var isUserInCurrentRoles = guildAdminRoles.Length == 0 || user.Roles.Any(x => guildAdminRoles.Contains(x.Id));

                var isUserInNewRoles = user.Roles.Any(x => roleIds.Contains(x.Id));
            
                if (!isUserInCurrentRoles || !isUserInNewRoles)
                {
                    
                    embedBuilder = new EmbedBuilder()
                        .WithTitle(resMan.GetString(nameof(Resources.Localization.SetupWrongUserRoleTitle), userSettings.Locale))
                        .WithDescription(string.Format(resMan.GetString(nameof(Resources.Localization.SetupUserDoesNotHaveAnyRole), userSettings.Locale), rolesStr));
                    await Context.Interaction.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
                    return;
                }
            } 
            
            _logger.LogInformation("Chosen admin roles: {Roles} for guild: {Guild}", roleIds, guild);
            await _guildSettingsService.UpdateAdminRoles(guild.Id, roleIds);

            embedBuilder = new EmbedBuilder()
                .WithTitle(resMan.GetString(nameof(Resources.Localization.SetupClassRolesTitle), userSettings.Locale))
                .WithDescription(resMan.GetString(nameof(Resources.Localization.SetupClassRolesDescription), userSettings.Locale));

            var componentBuilder = new ComponentBuilder()
                .WithButton(new ButtonBuilder()
                    .WithCustomId(KnownButtons.ConfirmClassRolesCreation)
                    .WithStyle(ButtonStyle.Success)
                    .WithLabel(resMan.GetString(nameof(Resources.Localization.SetupClassRolesCreateButton), userSettings.Locale))
                )
                .WithButton(new ButtonBuilder()
                    .WithCustomId(KnownButtons.ContinueToRaidBossGroups)
                    .WithStyle(ButtonStyle.Secondary)
                    .WithLabel(resMan.GetString(nameof(Resources.Localization.SetupConinueButton), userSettings.Locale))
                );
            
            await Context.Interaction.RespondAsync(embed:embedBuilder.Build(), components: componentBuilder.Build(), ephemeral: true);

        }

        [RequireContext(ContextType.Guild)]
        [ComponentInteraction(KnownButtons.ConfirmClassRolesCreation)]
        public async Task CreateClassRoles()
        {
            var resMan = Resources.Localization.ResourceManager;
            var guild = Context.Guild;
            
            var guildAdminRoles = await _guildSettingsService.GetAdminRoles(guild.Id);

            var roles = guild.Roles.Where(x => guildAdminRoles.Contains(x.Id));
            var rolesStr = string.Join(", ", roles.Select(x => x.Name));
            EmbedBuilder? embedBuilder;
            var userSettings = await _userSettingsService.GetUserSettings(Context.User.Id);
            if (Context.User is SocketGuildUser user)
            {
                var isUserInCurrentRoles = user.Roles.Any(x => guildAdminRoles.Contains(x.Id));
                
            
                if (!isUserInCurrentRoles)
                {
                    
                    embedBuilder = new EmbedBuilder()
                        .WithTitle(resMan.GetString(nameof(Resources.Localization.SetupWrongUserRoleTitle), userSettings.Locale))
                        .WithDescription(string.Format(resMan.GetString(nameof(Resources.Localization.SetupUserDoesNotHaveAnyRole), userSettings.Locale), rolesStr));
                    await Context.Interaction.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
                    return;
                }
            } 
            _guildSettingsService.CreateClassRoles(Context.Guild, userSettings.Locale);
            
            embedBuilder = new EmbedBuilder()
                .WithTitle(resMan.GetString(nameof(Resources.Localization.SetupBossRaidChannelsTitle), userSettings.Locale))
                .WithDescription(resMan.GetString(nameof(Resources.Localization.SetupBossRaidChannelsDescription), userSettings.Locale));

            var componentBuilder = new ComponentBuilder()
                .WithButton(new ButtonBuilder()
                    .WithCustomId(KnownButtons.ConfirmBossRoomsCreation)
                    .WithStyle(ButtonStyle.Success)
                    .WithLabel(resMan.GetString(nameof(Resources.Localization.SetupBossRaidChannelsConfirm), userSettings.Locale))
                )
                .WithButton(new ButtonBuilder()
                    .WithCustomId(KnownButtons.ContinueToRaidBossGroups)
                    .WithStyle(ButtonStyle.Secondary)
                    .WithLabel(resMan.GetString(nameof(Resources.Localization.SetupConinueButton), userSettings.Locale))
                );
            
            await Context.Interaction.RespondAsync(embed:embedBuilder.Build(), components: componentBuilder.Build(), ephemeral: true);
        }
        
    }
}