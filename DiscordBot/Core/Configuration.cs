using Discord;
using Microsoft.Extensions.Configuration.Json;

namespace DiscordBot.Core;

public static class Configuration
{
    private const string ConfigurationPath = "Configuration";

    public static IConfiguration BuildConfiguration()
    {
        var config = new ConfigurationBuilder();

        config.Sources.Add(new JsonConfigurationSource { Path = GetConfigurationSource() });
#if DEBUG
        var env = "Debug";
#else
        var env = "Release";
#endif
        Program.Log(new LogMessage(LogSeverity.Info, nameof(Configuration), $"Loading environment: {env}"));

        config.Sources.Add(new JsonConfigurationSource { Path = GetConfigurationSource(env) });
        return config.Build();
    }

    private static string GetConfigurationSource(string? environment = null)
    {
        var insertStr = string.IsNullOrEmpty(environment) ? "" : "." + environment;
        return Path.Combine(ConfigurationPath, $"configuration{insertStr}.json");
    }
}