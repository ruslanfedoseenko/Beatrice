using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LostArk.Discord.Bot.Db;
using LostArk.Discord.Bot.DomainServices;
using LostArk.Discord.Bot.Infrastructure;
using LostArk.Discord.Bot.IntractionServices;
using LostArk.Discord.Bot.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LostArk.Discord.Bot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private readonly DiscordSocketConfig _socketConfig = new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = false,
        };
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var gameSettings = Configuration.GetSection("GameSettings").Get<GameSettings>();

            services.AddLogging(o =>
                {
                    o.AddSerilog(logger: Log.Logger, dispose: true);
                })
                .AddLocalization(o =>
                {
                    o.ResourcesPath = "Resources";
                })
                .Configure<RequestLocalizationOptions>(options =>
                {
                    options.SetDefaultCulture("ru");
                    options.AddSupportedUICultures("en-US", "ru-RU");
                    options.FallBackToParentUICultures = true;

                })
                .AddSingleton<Bootstrap>()
                .AddTransient<ICharacterService, CharacterService>()
                .AddTransient<IUserSettingsService, UserSettingsService>()
                .AddTransient<IGuildCommonComponentsProvider, GuildCommonComponentsProvider>()
                .AddSingleton(_socketConfig)
                .AddSingleton(gameSettings)
                .AddPooledDbContextFactory<BotDbContext>(
                    (sp, o) => o.UseNpgsql(Configuration.GetConnectionString("Database"))
                        .UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>()),
                    Configuration.GetValue<int>("DbPoolSize"))
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<GuildInteractionService>()
                .AddSingleton<CommandsInteractionService>()
                .AddSingleton<IInitializable, CommandsInteractionService>(sp =>
                    sp.GetRequiredService<CommandsInteractionService>())
                .AddSingleton<IInitializable, GuildInteractionService>(sp =>
                sp.GetRequiredService<GuildInteractionService>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Bootstrap bootstrap)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
           
            bootstrap.InitializeAsync();
        }
    }
}