using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;

namespace DiscordBot.Modules.NicknameModule;

public class NicknameModule : InteractionModuleBase<SocketInteractionContext>
{
    private const string NicknameId = "nickname";
    private readonly GuildData _guildData;

    public NicknameModule(GuildData guildData)
    {
        _guildData = guildData;
    }

    [SlashCommand("nickname", "Set a nickname for yourself")]
    public async Task NicknameAsync([Summary("New-Nickname")] string newNickname = "")
    {
        if (newNickname == "")
        {
            await RespondWithModalAsync<NicknameModal>($"{NicknameId}-{Context.User.Id}");
            return;
        }

        var user = Context.User as SocketGuildUser ??
                   _guildData.Guild.GetUser(Context.User.Id);

        await HandleNickname(user, newNickname);
    }

    [ModalInteraction($"{NicknameId}-*")]
    public async Task PostMessage(string userId, NicknameModal modal)
    {
        var user = _guildData.Guild.GetUser(ulong.Parse(userId));

        await HandleNickname(user, modal.Name);
    }

    private async Task HandleNickname(SocketGuildUser user, string newNickname)
    {
        await user.ModifyAsync(properties => properties.Nickname = newNickname);

        await Context.User.SendMessageAsync($"Votre nom sur le serveur codebusters est à present '{newNickname}'");
        await RespondAsync("");
    }
}