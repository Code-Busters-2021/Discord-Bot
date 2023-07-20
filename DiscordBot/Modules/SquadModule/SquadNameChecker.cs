namespace DiscordBot.Modules.SquadModule;

public class SquadNameChecker
{
    private readonly List<string> _squadAllowedWords;

    [ActivatorUtilitiesConstructor]
    public SquadNameChecker(IConfiguration configuration)
        : this(configuration.GetSection("SquadAllowedWords").Get<List<string>>())
    {
    }

    public SquadNameChecker(List<string> squadAllowedWords)
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