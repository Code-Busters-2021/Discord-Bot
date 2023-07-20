using Discord.Interactions;
using DiscordBot.Modules;
using DiscordBot.Modules.SquadModule;

namespace DiscordBot.Core;

public static class ServicesBuilder
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<GuildData>()
            .AddSingleton<SquadNameChecker>()
            .AddSingleton<InteractionService>()
            .AddSingleton<InteractionMapper>();
    }
}