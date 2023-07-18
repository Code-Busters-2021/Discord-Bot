using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Modules.AutocompleteHandlers;

namespace DiscordBot.Modules;

public class SquadModule : InteractionModuleBase<SocketInteractionContext>
{
    public const string SetSquadId = "setsquad";
    public const string InputSquadId = "inputsquad";
    public const string CreateSquadId = "createsquad";

    private readonly GuildData _guildData;

    public SquadModule(GuildData guildData)
    {
        _guildData = guildData;
    }

    [SlashCommand("squad", "Set squad for a user")]
    public async Task SquadAsync(SocketUser user,
        [Autocomplete(typeof(SquadAutocompleteHandler))]
        string squadId)
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

        var squad = Context.Guild.GetRole(ulong.Parse(squadId));

        await guildUser.RemoveRolesAsync(_guildData.Squads);

        await guildUser.AddRoleAsync(squad);

        await RespondAsync($"{guildUser.Mention} est maintenant membre de {squad.Name}");
    }


    [ComponentInteraction($"{InputSquadId}-*")]
    public async Task InputSquad(string userId)
    {
        await Context.Channel.DeleteMessageAsync(((IComponentInteraction)Context.Interaction).Message);
    }

    [ModalInteraction($"{CreateSquadId}-*")]
    public async Task CreateSquad(string userId, AddSquadModal squadModal)
    {
        var guild = Context.Guild;
        var guildUser = guild.GetUser(ulong.Parse(userId));

        var squad = await guild.CreateRoleAsync(squadModal.Name, _guildData.Squads.FirstOrDefault()?.Permissions);
        _guildData.UpdateSquads();

        await guildUser.RemoveRolesAsync(_guildData.Squads);

        await guildUser.AddRoleAsync(squad);

        await RespondAsync($"{guildUser.Mention} a été ajouté.e à {squad.Name}");
    }

    // Defines the modal that will be sent.
    public class AddSquadModal : IModal
    {
        // Strings with the ModalTextInput attribute will automatically become components.
        [InputLabel("Nom de la nouvelle squad")]
        [ModalTextInput("squad_name", placeholder: "La Squad de Jean Dupond", maxLength: 30)]
        public string Name { get; set; }

        public string Title => "Creation de squad";
    }
}