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
    public async Task InputNicknameAsync([Summary("New-Nickname")] string newNickname = "")
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
    public async Task SetNickname(string userId, NicknameModal modal)
    {
        var user = _guildData.Guild.GetUser(ulong.Parse(userId));

        // This command doesn't work for now, the bot is missing permissions
        await RespondAsync("I don't have permission to change your nickname, please do it yourself",
            ephemeral: true);
        return;

        await HandleNickname(user, modal.Name);
    }

    private async Task HandleNickname(SocketGuildUser user, string newNickname)
    {
        await user.ModifyAsync(properties => properties.Nickname = newNickname);

        await RespondAsync($"Votre nom sur le serveur codebusters est à present '{newNickname}'", ephemeral: true);
    }
}