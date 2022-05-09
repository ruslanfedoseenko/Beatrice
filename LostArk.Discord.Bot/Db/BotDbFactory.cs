using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LostArk.Discord.Bot.Db
{
    public class BotDbFactory : IDesignTimeDbContextFactory<BotDbContext>
    {
       

        public BotDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration =  new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DC_")
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .Build();
            
            var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"));

            return new BotDbContext(optionsBuilder.Options);
        }
    }
}