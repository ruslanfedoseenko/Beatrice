using System;
using Discord.Interactions;

namespace LostArk.Discord.Bot.Modals
{
    public class CharacterModal : IModal
    {
        public class KnownInputs
        {
            public const string Class = "Character.Class";
            public const string Name = "Character.Name";
            public const string Gearscore = "Character.Gearscore";
        }
        public string Title { get; } = "Создание персонажа";

        [ModalTextInput(KnownInputs.Class, placeholder: "Класс")] 
        public string Class { get; set; }
        
        [ModalTextInput(KnownInputs.Name, placeholder: "Имя")] 
        public string Name { get; set; }

        [ModalTextInput(KnownInputs.Gearscore, placeholder: "ГС")] 
        public Decimal Gearscore { get; set; }
    }
}