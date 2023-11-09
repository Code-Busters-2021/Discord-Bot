using Discord.Interactions;
using DiscordBot.Modules.GradeModule;
using DiscordBot.Modules.SquadModule;

namespace DiscordBot.Core;

public static class ServicesBuilder
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddSingleton<GuildData>()
            .AddSingleton<SquadNameChecker>()
            .AddSingleton<InteractionService>()
            .AddSingleton<InteractionMapper>()
            .AddSingleton(_ => configuration)
            .AddSingleton(SquadModuleConfiguration.Get(configuration))
            .AddSingleton(GradeModuleConfiguration.Get(configuration));
    }
}