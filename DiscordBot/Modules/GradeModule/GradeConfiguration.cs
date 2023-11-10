using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Modules.GradeModule;

[JsonSerializable(typeof(GradeModuleConfiguration))]
public class GradeModuleConfiguration
{
    // What roles can use this command
    public string[] AllowedRoles { get; init; } = null!;

    // On what roles can this command be used
    public string[] CanBeUsedOn { get; init; } = null!;

    public static GradeModuleConfiguration Get(IConfiguration configuration)
    {
        return configuration.GetSection("GradeModule").Get<GradeModuleConfiguration>()
               ?? throw new Exception("GradeModule not found in config");
    }
}