using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using LostArk.Discord.Bot.Db;
using LostArk.Discord.Bot.Db.Enteties;
using LostArk.Discord.Bot.DomainServices.Models;
using LostArk.Discord.Bot.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LostArk.Discord.Bot.DomainServices
{
    public class CharacterService : ICharacterService
    {
        private readonly ILogger<CharacterService> _logger;
        private readonly IDbContextFactory<BotDbContext> _dbContextFactory;
        private readonly GameSettings _gameSettings;

        public CharacterService(ILogger<CharacterService> logger, IDbContextFactory<BotDbContext> dbContextFactory, GameSettings gameSettings)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _gameSettings = gameSettings;
        }

        public async Task AddCharacter(SocketUser user, decimal gearScore, string name, string @class)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            await db.Characters.AddAsync(new Character()
            {
                Name = name,
                GearScore = gearScore,
                UserId = user.Id,
                ClassId = _gameSettings.Classes.FirstOrDefault(x => x.Name == @class)!.Id
            });
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<CharacterInfo>> GetCharacters(SocketUser user)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            var characters = await db.Characters.AsNoTracking().Where(x => x.UserId == user.Id).ToArrayAsync();
            
            return characters.Select(x =>
            {
                return new CharacterInfo()
                {
                    Class = _gameSettings.Classes.FirstOrDefault(y => y.Id == x.ClassId),
                    Id = x.Id,
                    GearScore = x.GearScore,
                    Name = x.Name,
                };
            });
        }

        public async Task DeleteCharacter(long id)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            var c = await db.Characters.FirstOrDefaultAsync(x => x.Id == id);
            if (c != null)
            {
                db.Characters.Remove(c);
                await db.SaveChangesAsync();
            }
        }

        public async Task<CharacterInfo> GetCharacter(long id)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            var character = await db.Characters.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            return new CharacterInfo()
            {
                Class = _gameSettings.Classes.FirstOrDefault(y => y.Id == character.ClassId),
                Id = character.Id,
                GearScore = character.GearScore,
                Name = character.Name,
            };

        }

        public async Task UpdateCharacter(long id, SocketUser user, decimal gearscore, string name, string @class)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            var character = await db.Characters.FirstOrDefaultAsync(x => x.Id == id);

            if (character != null)
            {
                character.Name = name;
                character.GearScore = gearscore;
                character.ClassId = _gameSettings.Classes.FirstOrDefault(y => y.Name == @class)!.Id;
                await db.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Character with id: {Id} not found", id);
            }
        }
    }

    public interface ICharacterService
    {
        Task AddCharacter(SocketUser user, decimal gearScore, string name, string @class);
        Task<IEnumerable<CharacterInfo>> GetCharacters(SocketUser user);
        Task DeleteCharacter(long id);
        Task<CharacterInfo> GetCharacter(long id);
        Task UpdateCharacter(long id, SocketUser user, decimal gearscore, string name, string @class);
    }
}