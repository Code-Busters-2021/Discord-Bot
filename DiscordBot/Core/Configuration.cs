using Microsoft.Extensions.Configuration.Json;

namespace DiscordBot;

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
        string env = "Release";
#endif
        config.Sources.Add(new JsonConfigurationSource { Path = GetConfigurationSource(env) });
        return config.Build();
    }

    public static string GetConfigurationSource(string? environment = null)
    {
        var insertStr = string.IsNullOrEmpty(environment) ? "" : "." + environment;
        return Path.Combine(ConfigurationPath, $"configuration{insertStr}.json");
    }
}