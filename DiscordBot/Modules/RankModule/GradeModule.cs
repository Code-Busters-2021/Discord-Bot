using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Modules.GradeModule;
using DiscordBot.Modules.ModuleBase;

namespace DiscordBot.Modules.RankModule;

public class GradeModule : OverlayInteractionModuleBase<SocketInteractionContext>
{
    private readonly HashSet<ulong> _canBeUsedOn;

    public GradeModule(GuildData guildData, GradeModuleConfiguration configuration) : base(guildData)
    {
        AllowedRoles = configuration.AllowedRoles?
            .Select(roleStr => guildData.GradeRoles.First(role => role.Name == roleStr).Id)
            .ToHashSet();
        _canBeUsedOn = configuration.CanBeUsedOn!
            .Select(roleStr => GuildData.GradeRoles.First(role => role.Name == roleStr).Id).ToHashSet();
    }

    [SlashCommand("rank", "Set rank for a user")]
    public async Task RankAsync([Summary("User")] SocketUser user,
        [Summary("Rank")] [Autocomplete(typeof(GradeAutocompleteHandler))]
        string rankStr)
    {
        await RespondAndThrowIfUserDenied();

        var targetUser = user as SocketGuildUser ??
                         GuildData.Guild.GetUser(user.Id);

        if (targetUser.GuildPermissions.Administrator)
        {
            await RespondAsync("You cannot assign a rank to an admin user");
            return;
        }

        if (!targetUser.Roles.Any(role => _canBeUsedOn.Contains(role.Id)))
        {
            await RespondAsync("You cannot use this command on this user");
            return;
        }

        await targetUser.RemoveRolesAsync(_canBeUsedOn);

        var rankRole = GuildData.GradeRoles.First(rank => rank.Name == rankStr);
        await targetUser.AddRoleAsync(rankRole);

        await targetUser.SendMessageAsync($"Vous êtes à présent {rankRole.Name}");

        await RespondAsync($"{targetUser.Mention} est à présent {rankRole.Name}", ephemeral: true);
    }
}