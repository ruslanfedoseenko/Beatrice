using System.Threading.Tasks;

namespace LostArk.Discord.Bot.Infrastructure
{
    public interface IInitializable
    {
        Task InitializeAsync();
    }
}