using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;

namespace DiscordBot.Modules.PostModule;

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
    public async Task InputMessage(
        [Summary("Channel")] [Autocomplete(typeof(ChannelAutocompleteHandler))] string channelId)
    {
        await RespondWithModalAsync<PostMessageModal>($"{PostMessageId}-{channelId}");
    }

    [ModalInteraction($"{PostMessageId}-*")]
    public async Task PostMessage(string channelId, PostMessageModal modal)
    {
        var textChannel = _guildData.Guild.GetTextChannel(ulong.Parse(channelId));
        await textChannel.SendMessageAsync(modal.Contenu);
        await RespondAsync($"Le message a été posté sur le channel {textChannel.Name}");
    }
}