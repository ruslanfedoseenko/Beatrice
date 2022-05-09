using Discord.Interactions;

namespace LostArk.Discord.Bot.Modals
{
    public class CreateRaidModal : IModal
    {
        public CreateRaidModal()
        {
            
        }

        public string Title { get; set; } = "Создание рейда";
        
        [ModalTextInput("raid_description")] 
        public string Description { get; set; }
        
        [ModalTextInput("raid_date", placeholder: "dd.mm.yy HH:MM")] 
        public string Date { get; set; }
        
        [ModalTextInput("gs_requirement", placeholder: "Minimal GS to enter")] 
        public string GearScoreRequirement { get; set; }
    }
}