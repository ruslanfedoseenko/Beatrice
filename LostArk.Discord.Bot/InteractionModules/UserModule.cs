using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using LostArk.Discord.Bot.Constants;
using LostArk.Discord.Bot.DomainServices;
using LostArk.Discord.Bot.Infrastructure;
using LostArk.Discord.Bot.Modals;
using LostArk.Discord.Bot.Models.Configuration;
using Microsoft.Extensions.Logging;
using ModalBuilder = Discord.ModalBuilder;

namespace LostArk.Discord.Bot.InteractionModules
{
    public class UserModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly GameSettings _gameSettings;
        private readonly ILogger<UserModule> _logger;
        private readonly IUserSettingsService _userSettingsService;
        private readonly ICharacterService _characterService;

        public UserModule(GameSettings gameSettings,
            ILogger<UserModule> logger,
            IUserSettingsService userSettingsService,
            ICharacterService characterService)
        {
            _gameSettings = gameSettings;
            _logger = logger;
            _userSettingsService = userSettingsService;
            _characterService = characterService;
        }

        [Group("user", "User registration processing")]
        public class UserGroup : InteractionModuleBase<SocketInteractionContext>
        {
            private readonly IGuildCommonComponentsProvider _guildComponentsProvider;

            public UserGroup(IGuildCommonComponentsProvider guildComponentsProvider)
            {
                _guildComponentsProvider = guildComponentsProvider;
            }

            [SlashCommand("register", "Initiate user registration", runMode: RunMode.Async)]
            public async Task Register()
            {
                var user = Context.User;
                var guild = Context.Guild;
                var welcomeMessage = await _guildComponentsProvider.GetWelcomeComponent(guild.Name, user.Id);

                await Context.Interaction.RespondAsync(embed: welcomeMessage.Embed,
                    components: welcomeMessage.Components, ephemeral: true);
            }
        }


        [ComponentInteraction(KnownButtons.DeleteCharacter)]
        public async Task ChoseCharacterToEdit()
        {
            await ResponseWithCharacterList(KnownSelects.Actions.Delete);
        }

        private async Task ResponseWithCharacterList(string action)
        {
            var user = Context.User;

            var chars = await _characterService.GetCharacters(user);

            var selectBuilder = new SelectMenuBuilder()
                .WithCustomId(KnownSelects.Character + action);

            foreach (var character in chars)
            {
                selectBuilder.AddOption(new SelectMenuOptionBuilder().WithEmote(Emote.Parse(character.Class.Emote))
                    .WithLabel($"{character.Name} {character.GearScore}").WithValue(character.Id.ToString()));
            }

            var componentsBuilder = new ComponentBuilder()
                .WithSelectMenu(selectBuilder);

            await Context.Interaction.RespondAsync(components: componentsBuilder.Build(), ephemeral: true);
        }

        [ComponentInteraction(KnownButtons.EditCharacter)]
        public async Task ChoseCharacterToDelete()
        {
            await ResponseWithCharacterList(KnownSelects.Actions.Edit);
        }


        [ComponentInteraction(KnownSelects.Character + KnownSelects.Actions.Edit)]
        public async Task EditCharacter(long id)
        {
            var resMan = Resources.Localization.ResourceManager;
            var user = Context.User;
            var userSettings = await _userSettingsService.GetUserSettings(user.Id);
            var culture = userSettings.Locale;
            var character = await _characterService.GetCharacter(id);
            await Context.Interaction.RespondWithModalAsync<CharacterModal>(KnownModals.Character + KnownModals.Actions.Edit + $":{id}",
                modifyModal: m => m
                    .WithTitle(resMan.GetString(nameof(Resources.Localization.EditCharacter), culture))
                    .UpdateTextInput(CharacterModal.KnownInputs.Class, x => x.Value = character.Class.Name)
                    .UpdateTextInput(CharacterModal.KnownInputs.Gearscore, x => x.Value = character.GearScore.ToString(CultureInfo.InvariantCulture))
                    .UpdateTextInput(CharacterModal.KnownInputs.Name, x => x.Value = character.Name)
            );
        }

        [ComponentInteraction(KnownSelects.Character + KnownSelects.Actions.Delete)]
        public async Task DeleteCharacter(long id)
        {
            _logger.LogInformation("Deleting character. ID: {Id}", id);
            await _characterService.DeleteCharacter(id);
            await Context.Interaction.RespondAsync("Character deleted", ephemeral: true);
        }



        [ComponentInteraction(KnownButtons.AddCharacter)]
        public async Task AddCharacter()
        {
            var selectMenuBuilder = new SelectMenuBuilder()
                .WithCustomId(KnownSelects.Class);
            var resMan = Resources.Localization.ResourceManager;
            var user = Context.User;
            var userSettings = await _userSettingsService.GetUserSettings(user.Id);
            var culture = userSettings.Locale;
            foreach (var gameClass in _gameSettings.Classes)
            {
                var label = resMan.GetString(gameClass.Name, culture) ?? gameClass.Name;
                selectMenuBuilder.AddOption(new SelectMenuOptionBuilder().WithValue(gameClass.Name).WithLabel(label)
                    .WithEmote(Emote.Parse(gameClass.Emote)));
            }

            var componentsBuilder = new ComponentBuilder()
                .WithSelectMenu(selectMenuBuilder);

            var text = resMan.GetString(nameof(Resources.Localization.ChoseClass), culture);

            await Context.Interaction.RespondAsync(text, components: componentsBuilder.Build(), ephemeral: true);
        }

        [ComponentInteraction(KnownSelects.Class)]
        public async Task OnClassSelected(string value)
        {
            await Context.Interaction.RespondWithModalAsync<CharacterModal>(
                KnownModals.Character + KnownModals.Actions.Create, modifyModal: m => m
                    .UpdateTextInput("Character.Class", x => x.Value = value));
        }



        [ModalInteraction(KnownModals.Character + KnownModals.Actions.Create)]
        public async Task CreateCharacter(CharacterModal modal)
        {
            var user = Context.User;
            _logger.LogInformation("Modal:{Modal} Data: GS: {GS} Name: {Name} Class: {Class}",
                nameof(CharacterModal), modal.Gearscore, modal.Name, modal.Class);

            if (_gameSettings.Classes.All(x => x.Name != modal.Class))
            {
                _logger.LogWarning("Invalid class:{Class}", modal.Class);
                await RespondAsync(ephemeral: true, text: "Invalid class");
                return;
            }

            await _characterService.AddCharacter(user, modal.Gearscore, modal.Name, modal.Class);
            await RespondAsync(ephemeral: true, text: "Character created");
        }
        
        [ModalInteraction(KnownModals.Character + KnownModals.Actions.Edit + ":*")]
        public async Task EditCharacter(long id, CharacterModal modal)
        {
            var user = Context.User;
            _logger.LogInformation("Modal:{Modal} Data: Id:{id} GS: {GS} Name: {Name} Class: {Class}",
                nameof(CharacterModal), id, modal.Gearscore, modal.Name, modal.Class);

            if (_gameSettings.Classes.All(x => x.Name != modal.Class))
            {
                _logger.LogWarning("Invalid class:{Class}", modal.Class);
                await RespondAsync(ephemeral: true, text: "Invalid class");
                return;
            }

            await _characterService.UpdateCharacter(id, user, modal.Gearscore, modal.Name, modal.Class);
            await RespondAsync(ephemeral: true, text: "Character created");
        }
    }
}