using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Modules.ModuleBase;

namespace DiscordBot.Modules.GradeModule;

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

    [SlashCommand("grade", "Set grade for a user")]
    public async Task GradeAsync([Summary("User")] SocketUser user,
        [Summary("Grade")] [Autocomplete(typeof(GradeAutocompleteHandler))]
        string gradeStr)
    {
        await RespondAndThrowIfUserDenied();

        var targetUser = GuildData.ToGuildUser(user);

        if (targetUser.GuildPermissions.Administrator)
        {
            await RespondAsync("You cannot assign a grade to an admin user", ephemeral: true);
            return;
        }

        // Can be used on designed roles, or on users who don't have any grade
        if (!targetUser.Roles.Any(role => _canBeUsedOn.Contains(role.Id))
            && targetUser.Roles.Intersect(GuildData.GradeRoles).Any())
        {
            await RespondAsync("You cannot use this command on this user", ephemeral: true);
            return;
        }

        await targetUser.RemoveRolesAsync(_canBeUsedOn);

        var grade = GuildData.GradeRoles.First(grade => grade.Name == gradeStr);
        await targetUser.AddRoleAsync(grade);

        await targetUser.SendMessageAsync($"Vous êtes à présent {grade.Name}");

        await RespondAsync($"{targetUser.Mention} est à présent {grade.Name}", ephemeral: true);
    }
}