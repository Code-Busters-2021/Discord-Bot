using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Modules.ModuleBase;

namespace DiscordBot.Modules.PostModule;

public class PostMessageModule : OverlayInteractionModuleBase<SocketInteractionContext>
{
    private const string PostMessageId = "postmessage";
    private readonly DiscordSocketClient _client;

    public PostMessageModule(GuildData guildData, DiscordSocketClient client) : base(guildData)
    {
        _client = client;
    }

    [SlashCommand("post", "Ask the bot to post a message in a channel")]
    public async Task InputMessage(
        [Summary("Channel")] [Autocomplete(typeof(ChannelAutocompleteHandler))]
        string channelId)
    {
        await RespondWithModalAsync<PostMessageModal>($"{PostMessageId}-{channelId}");
    }

    [ModalInteraction($"{PostMessageId}-*")]
    public async Task PostMessage(string channelId, PostMessageModal modal)
    {
        var textChannel = GuildData.Guild.GetTextChannel(ulong.Parse(channelId));
        await textChannel.SendMessageAsync(modal.Content);
        await RespondAsync($"Le message a été posté sur le channel {textChannel.Name}", ephemeral: true);
    }
}