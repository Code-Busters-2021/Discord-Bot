using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;

namespace DiscordBot.Modules;

public class PostMessageModule : InteractionModuleBase<SocketInteractionContext>
{
    private const string PostMessageId = "postmessage";
    private readonly DiscordSocketClient _client;

    private readonly GuildData _guildData;

    public PostMessageModule(GuildData guildData, DiscordSocketClient client)
    {
        _guildData = guildData;
        _client = client;
    }

    [SlashCommand("post", "Ask the bot to post a message in a channel")]
    public async Task InputMessage([Autocomplete(typeof(ChannelAutocompleteHandler))] string channelId)
    {
        await RespondWithModalAsync<PostMessageModal>($"{PostMessageId}-{channelId}");
    }

    [ModalInteraction($"{PostMessageId}-*")]
    public async Task PostMessage(string channelId, PostMessageModal modal)
    {
        await _guildData.Guild.GetTextChannel(ulong.Parse(channelId))
            .SendMessageAsync(modal.Contenu);
        await RespondAsync();
    }

    public class ChannelAutocompleteHandler : AutocompleteHandler
    {
        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
            IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            // Create a collection with suggestions for autocomplete
            var results = services.GetRequiredService<GuildData>().PostMessageChannels
                .Select(channel => new AutocompleteResult(channel.Name, channel.Id.ToString()));

            // max - 25 suggestions at a time (API limit)
            return Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
        }
    }

    // Defines the modal that will be sent.
    public class PostMessageModal : IModal
    {
        // Strings with the ModalTextInput attribute will automatically become components.
        [InputLabel("Contenu du message")]
        [ModalTextInput("message_content", TextInputStyle.Paragraph,
            "Bonjour,\nComme vous le savez tous ...\nC'est pour cela que ...\nAinsi ...")]
        public string Contenu { get; set; }

        public string Title => "Poster un message";
    }
}