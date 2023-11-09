using Discord;
using Discord.Interactions;
using DiscordBot.Core;
using DiscordBot.Modules.GradeModule;

namespace DiscordBot.Modules.RankModule;

public class GradeAutocompleteHandler : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var configuration = services.GetRequiredService<GradeModuleConfiguration>();
        // Create a collection with suggestions for autocomplete
        var results = configuration.CanBeUsedOn!
            .Select(roleStr => new AutocompleteResult(roleStr, roleStr));

        // max - 25 suggestions at a time (API limit)
        return Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
    }
}