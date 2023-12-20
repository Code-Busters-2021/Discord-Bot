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
        var guildData = services.GetRequiredService<GuildData>();

        // Seul un moderateur peut poster dans le channel du bot
        var isModerateur = guildData.ToGuildUser(context.User).Roles.Select(role => role.Name).Contains("Moderateur");
        var channels = isModerateur ? guildData.PostChannels.Append(guildData.BotChannel) : guildData.PostChannels;

        // Create a collection with suggestions for autocomplete
        var results = channels.Select(channel => new AutocompleteResult(channel.Name, channel.Id.ToString()));

        // max - 25 suggestions at a time (API limit)
        return Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
    }
}