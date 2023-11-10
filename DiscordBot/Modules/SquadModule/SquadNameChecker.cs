using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Modules.SquadModule;

public class SquadNameChecker
{
    private readonly string[] _squadAllowedWords;

    [ActivatorUtilitiesConstructor]
    public SquadNameChecker(SquadModuleConfiguration config)
        : this(config.SquadAllowedWords
               ?? throw new Exception(nameof(config.SquadAllowedWords) + " configuration was not found"))
    {
    }

    public SquadNameChecker(string[] squadAllowedWords)
    {
        _squadAllowedWords = squadAllowedWords;
    }

    public string Explanation =>
        @"Contenir le mot 'squad' ou le mot 'team' (indépendament de la casse)";

    public bool CheckName(string name)
    {
        foreach (var word in _squadAllowedWords)
            if (name.Contains(word, StringComparison.OrdinalIgnoreCase))
                return true;

        return false;
    }
}