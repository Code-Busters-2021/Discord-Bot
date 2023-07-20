using Discord;
using Discord.Interactions;
using DiscordBot.Core;

namespace DiscordBot.Modules.RankModule;

public class RankAutocompleteHandler : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var guildData = services.GetRequiredService<GuildData>();
        var ranks = new[] { guildData.BronzeRole, guildData.SilverRole, guildData.GoldRole, guildData.DiamondRole };
        // Create a collection with suggestions for autocomplete
        var results = ranks
            .Select(rank => new AutocompleteResult(rank.Name, rank.Id.ToString()));

        // max - 25 suggestions at a time (API limit)
        return Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
    }
}