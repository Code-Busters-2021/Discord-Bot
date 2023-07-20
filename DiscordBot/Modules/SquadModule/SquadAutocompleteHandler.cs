using Discord;
using Discord.Interactions;
using DiscordBot.Core;

namespace DiscordBot.Modules.SquadModule;

public class SquadAutocompleteHandler : AutocompleteHandler
{
    private const int MaxOptionCount = 4;

    private readonly GuildData _guildData;

    public SquadAutocompleteHandler(GuildData guildData)
    {
        _guildData = guildData;
    }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var userInput = autocompleteInteraction.Data.Current.Value.ToString()!;

        var showSquads = userInput == ""
            ? _guildData.Squads.Take(MaxOptionCount)
            : _guildData.Squads
                .Where(squad => squad.Name.StartsWith(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(MaxOptionCount);

        // Create a collection with suggestions for autocomplete
        var results = showSquads
            .Select(squad => new AutocompleteResult(squad.Name, squad.Id.ToString()))
            .Prepend(new AutocompleteResult("Create a new Squad", ""));

        return Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
    }
}