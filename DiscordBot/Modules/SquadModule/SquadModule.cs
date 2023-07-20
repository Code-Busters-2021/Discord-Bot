using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;

namespace DiscordBot.Modules.SquadModule;

public class SquadModule : InteractionModuleBase<SocketInteractionContext>
{
    public const string InputSquadId = "inputsquad";
    public const string CreateSquadId = "createsquad";

    private readonly GuildData _guildData;
    private readonly SquadNameChecker _nameChecker;

    public SquadModule(GuildData guildData, SquadNameChecker nameChecker)
    {
        _guildData = guildData;
        _nameChecker = nameChecker;
    }

    [SlashCommand("squad", "Set squad for a user")]
    public async Task SquadAsync([Summary("User")] SocketUser user,
        [Summary("Squad")] [Autocomplete(typeof(SquadAutocompleteHandler))]
        string squadId)
    {
        var guildUser = user as SocketGuildUser ??
                        _guildData.Guild.GetUser(user.Id);

        if (guildUser.GuildPermissions.Administrator)
        {
            await RespondAsync("You cannot assign a rank to an admin user");
            return;
        }

        if (guildUser.Roles.Any(role => role.Id == _guildData.ManagerRole.Id))
        {
            await RespondAsync("You cannot assign a rank to a Manager");
            return;
        }

        if (squadId == "")
        {
            await RespondWithModalAsync<AddSquadModal>($"{CreateSquadId}-{user.Id}");
            return;
        }

        await DeferAsync(true);

        var squad = Context.Guild.GetRole(ulong.Parse(squadId));

        await AddUserToSquad(guildUser, squad);
    }


    [ComponentInteraction($"{InputSquadId}-*")]
    public async Task InputSquad(string userId)
    {
        await Context.Channel.DeleteMessageAsync(((IComponentInteraction)Context.Interaction).Message);
    }

    [ModalInteraction($"{CreateSquadId}-*")]
    public async Task CreateSquad(string userId, AddSquadModal squadModal)
    {
        var newName = squadModal.Name.Trim();
        if (!_nameChecker.CheckName(newName))
        {
            await RespondAsync($"{newName} n'est pas un nom valide. Le nom doit:\n"
                               + _nameChecker.Explanation);
            return;
        }

        if (_guildData.Squads.Any(squad=>squad.Name == newName))
        {
            await RespondAsync($"{newName} existe déjà");
            return;
        }

        await DeferAsync(true);

        var guild = Context.Guild;

        var squad = await guild.CreateRoleAsync(squadModal.Name, _guildData.Squads.FirstOrDefault()?.Permissions);
        _guildData.UpdateSquads();

        await AddUserToSquad(guild.GetUser(ulong.Parse(userId)), squad);
    }

    private async Task AddUserToSquad(SocketGuildUser user, IRole squad)
    {
        await user.RemoveRolesAsync(_guildData.Squads);

        await user.AddRoleAsync(squad);

        await user.SendMessageAsync($"Vous avez été ajouté.e à {squad.Name}");
        await FollowupAsync($"{user.Mention} a été ajouté.e à {squad.Name}");
    }
}