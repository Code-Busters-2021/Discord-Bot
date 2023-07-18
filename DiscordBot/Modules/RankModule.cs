using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Modules.AutocompleteHandlers;

namespace DiscordBot.Modules;

public class RankModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly GuildData _guildData;

    public RankModule(GuildData guildData)
    {
        _guildData = guildData;
    }

    [SlashCommand("rank", "Set rank for a user")]
    public async Task RankAsync(SocketUser user,
        [Autocomplete(typeof(RankAutocompleteHandler))]
        string rankId)
    {
        if (user is not SocketGuildUser guildUser)
        {
            await RespondAsync("You need to be in the server to use this command");
            return;
        }

        if (guildUser.GuildPermissions.Administrator)
        {
            await RespondAsync("You cannot assign a rank to an admin user");
            return;
        }

        if (guildUser.Roles.Any(role=>role.Id == _guildData.ManagerRole?.Id))
        {
            await RespondAsync("You cannot assign a rank to a Manager");
            return;
        }
        
        await guildUser.RemoveRolesAsync(new[]
            { _guildData.BronzeRole, _guildData.SilverRole, _guildData.GoldRole, _guildData.DiamondRole });


        var rankRole = Context.Guild.GetRole(ulong.Parse(rankId));
        await guildUser.AddRoleAsync(rankRole);

        await RespondAsync($"{guildUser.Mention} est à présent {rankRole.Name}");
    }
}