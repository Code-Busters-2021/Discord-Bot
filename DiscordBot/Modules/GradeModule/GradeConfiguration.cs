using System.Text.Json.Serialization;

namespace DiscordBot.Modules.GradeModule;

[JsonSerializable(typeof(GradeModuleConfiguration))]
public class GradeModuleConfiguration
{
    public string[]? AllowedRoles { get; init; }
    public string[]? CanBeUsedOn { get; init; }

    public static GradeModuleConfiguration Get(IConfiguration configuration)
    {
        return configuration.GetSection("GradeModule").Get<GradeModuleConfiguration>();
    }
}