using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Modules.SquadModule;

[JsonSerializable(typeof(SquadModuleConfiguration))]
public class SquadModuleConfiguration
{
    public string[]? AllowedRoles { get; init; }
    public string[]? CanBeUsedOn { get; init; }
    public string[]? SquadAllowedWords { get; init; }

    public static SquadModuleConfiguration Get(IConfiguration configuration)
    {
        return configuration.GetSection("SquadModule").Get<SquadModuleConfiguration>()
               ?? throw new Exception("SquadModule not found in config");
    }
}