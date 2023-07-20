using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;

namespace DiscordBot.Modules.RankModule;

public class RankModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly GuildData _guildData;

    public RankModule(GuildData guildData)
    {
        _guildData = guildData;
    }

    [SlashCommand("rank", "Set rank for a user")]
    public async Task RankAsync([Summary("User")] SocketUser user,
        [Summary("Rank")] [Autocomplete(typeof(RankAutocompleteHandler))]
        string rankId)
    {
        var guildUser = user as SocketGuildUser ??
                        _guildData.Guild.GetUser(user.Id);

        if (guildUser.GuildPermissions.Administrator)
        {
            await RespondAsync("You cannot assign a rank to an admin user");
            return;
        }

        if (guildUser.Roles.Any(role => role.Id == _guildData.ManagerRole?.Id))
        {
            await RespondAsync("You cannot assign a rank to a Manager");
            return;
        }

        await guildUser.RemoveRolesAsync(new[]
            { _guildData.BronzeRole, _guildData.SilverRole, _guildData.GoldRole, _guildData.DiamondRole });


        var rankRole = Context.Guild.GetRole(ulong.Parse(rankId));
        await guildUser.AddRoleAsync(rankRole);

        await guildUser.SendMessageAsync($"Vous êtes à présent {rankRole.Name}");

        await RespondAsync($"{guildUser.Mention} est à présent {rankRole.Name}");
    }
}