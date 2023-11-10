using Discord;
using Discord.Interactions;
using DiscordBot.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Modules.PostModule;

public class ChannelAutocompleteHandler : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        // Create a collection with suggestions for autocomplete
        var results = services.GetRequiredService<GuildData>().PostChannels
            .Select(channel => new AutocompleteResult(channel.Name, channel.Id.ToString()));

        // max - 25 suggestions at a time (API limit)
        return Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
    }
}