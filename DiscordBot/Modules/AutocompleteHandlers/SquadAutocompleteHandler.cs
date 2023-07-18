using Discord;
using Discord.Interactions;
using DiscordBot.Core;

namespace DiscordBot.Modules.AutocompleteHandlers;

public class SquadAutocompleteHandler : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var userInput = autocompleteInteraction.Data.Current.Value.ToString()!;

        var squads = services.GetRequiredService<GuildData>().Squads;
        // Create a collection with suggestions for autocomplete
        var results = squads
            .Select(squad => new AutocompleteResult(squad.Name, squad.Id.ToString()))
            .Prepend(new AutocompleteResult("Create a new Squad", ""))
            .Where(x => x.Name.StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase));

        return Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
    }
}