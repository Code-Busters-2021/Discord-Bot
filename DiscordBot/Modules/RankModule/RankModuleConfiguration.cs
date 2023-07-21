using System.Text.Json.Serialization;
using DiscordBot.Modules.SquadModule;

namespace DiscordBot.Modules.RankModule;

[JsonSerializable(typeof(RankModuleConfiguration))]
public class RankModuleConfiguration
{
    public string[]? AllowedRoles { get; init; }
    public string[]? CanBeUsedOn { get; init; }

    public static RankModuleConfiguration Get(IConfiguration configuration)
    {
        return configuration.GetSection("RankModule").Get<RankModuleConfiguration>();
    }
}