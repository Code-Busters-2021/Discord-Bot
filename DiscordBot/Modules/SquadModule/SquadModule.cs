using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Modules.ModuleBase;

namespace DiscordBot.Modules.SquadModule;

public class SquadModule : OverlayInteractionModuleBase<SocketInteractionContext>
{
    public const string CreateSquadId = "createsquad";

    private readonly HashSet<ulong> _canBeUsedOn;

    private readonly SquadNameChecker _nameChecker;

    public SquadModule(GuildData guildData, SquadNameChecker nameChecker, SquadModuleConfiguration config) :
        base(guildData)
    {
        _nameChecker = nameChecker;
        AllowedRoles = config.AllowedRoles?
            .Select(roleStr => guildData.GradeRoles.First(role => role.Name == roleStr).Id)
            .ToHashSet();
        _canBeUsedOn = config.CanBeUsedOn!
            .Select(roleStr => GuildData.GradeRoles.First(role => role.Name == roleStr).Id)
            .ToHashSet();
    }

    [SlashCommand("squad", "Set squad for a user")]
    public async Task SquadAsync([Summary("User")] SocketUser user,
        [Summary("Squad")] [Autocomplete(typeof(SquadAutocompleteHandler))]
        string squadId)
    {
        await RespondAndThrowIfUserDenied();

        var targetUser = user as SocketGuildUser ??
                         GuildData.Guild.GetUser(user.Id);

        if (targetUser.GuildPermissions.Administrator)
        {
            await RespondAsync("You cannot assign a grade to an admin user", ephemeral: true);
            return;
        }

        if (!targetUser.Roles.Any(role => _canBeUsedOn.Contains(role.Id)))
        {
            await RespondAsync("You can only assign a grade to a BBBuster", ephemeral: true);
            return;
        }

        if (squadId == "")
        {
            await RespondWithModalAsync<CreateSquadModal>($"{CreateSquadId}-{user.Id}");
            return;
        }

        await DeferAsync(true);

        var squad = Context.Guild.GetRole(ulong.Parse(squadId));
        await AddUserToSquad(targetUser, squad);
    }

    [ModalInteraction($"{CreateSquadId}-*")]
    public async Task CreateSquad(string userId, CreateSquadModal squadModal)
    {
        var newName = squadModal.Name.Trim();
        if (!_nameChecker.CheckName(newName))
        {
            await RespondAsync($"{newName} n'est pas un nom valide. Le nom doit:\n"
                               + _nameChecker.Explanation, ephemeral: true);
            return;
        }

        if (GuildData.SquadRoles.Any(squad => squad.Name == newName))
        {
            await RespondAsync($"{newName} existe déjà", ephemeral: true);
            return;
        }

        await DeferAsync(true);

        var guild = Context.Guild;

        var squad = await guild.CreateRoleAsync(squadModal.Name, GuildData.SquadRoles.FirstOrDefault()?.Permissions);
        GuildData.ExtractSquads();

        await AddUserToSquad(guild.GetUser(ulong.Parse(userId)), squad);
    }

    private async Task AddUserToSquad(SocketGuildUser user, IRole squad)
    {
        await user.RemoveRolesAsync(GuildData.SquadRoles);

        await user.AddRoleAsync(squad);

        await user.SendMessageAsync($"Vous avez été ajouté.e à {squad.Name}");

        await FollowupAsync($"{user.Mention} a été ajouté.e à {squad.Name}", ephemeral: true);
    }
}