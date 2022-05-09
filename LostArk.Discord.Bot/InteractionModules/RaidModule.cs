using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using LostArk.Discord.Bot.Constants;
using LostArk.Discord.Bot.IntractionServices;
using LostArk.Discord.Bot.Modals;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace LostArk.Discord.Bot.InteractionModules
{
    // Interation modules must be public and inherit from an IInterationModuleBase
    public class RaidModule : InteractionModuleBase<SocketInteractionContext>
    {
        // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
        public InteractionService Commands { get; set; }

        private CommandsInteractionService _service;
        private readonly IStringLocalizer<RaidModule> _stringLocalizer;
        private readonly ILogger<RaidModule> _logger;

        // Constructor injection is also a valid way to access the dependencies
        public RaidModule(CommandsInteractionService service, IStringLocalizer<RaidModule> stringLocalizer, ILogger<RaidModule> logger)
        {
            _service = service;
            _stringLocalizer = stringLocalizer;
            _logger = logger;
            _logger.LogInformation("Test");
        }

        

        // [Group] will create a command group. [SlashCommand]s and [ComponentInteraction]s will be registered with the group prefix
        [Group("raid", "This is a raid command group")]
        public class RaidGroup : InteractionModuleBase<SocketInteractionContext>
        {
            [RequireContext(ContextType.Guild)]
            [SlashCommand("create", "Create raid", runMode: RunMode.Async)]
            public async Task CrateRaid()
            {
                await RespondWithModalAsync<CreateRaidModal>(KnownModals.Raid);
            }
        }

        [RequireContext(ContextType.Guild)]
        [ModalInteraction(KnownModals.Raid)]
        public async Task Test(CreateRaidModal modal)
        {
            var user = (IGuildUser)Context.User;
            var builder = new EmbedBuilder()
                .WithTitle(modal.Title)
                .WithAuthor(user.ToString(), user.GetDisplayAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithDescription(_stringLocalizer.GetString(Resources.Localization.RaidCreatedMessageTemplate));
            var components = new ComponentBuilder()
                .WithButton("Join", "join_raid_button")
                .WithButton("Exit", "leave_raid_button");
            
                
            await Context.Interaction.RespondAsync(embed: builder.Build(), components: components.Build());
        }

        [RequireContext(ContextType.Guild)]
        [ComponentInteraction("join_raid_button")]
        public async Task ButtonPress()
        {
            // ...
            await RespondAsync($"join_raid_button :rabbit:");
        }

        [RequireContext(ContextType.Guild)]
        [ComponentInteraction("leave_raid_button")]
        public async Task RoleSelect()
        {
            await RespondAsync($"leave_raid_button <:avatar:972779816577277993>");
        }
    }
}
