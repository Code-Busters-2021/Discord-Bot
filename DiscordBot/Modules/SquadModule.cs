using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;

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

    public MessageComponent GetSquadChoiceComponent(SocketGuildUser user)
    {
        return new ComponentBuilder()
            .WithSelectMenu(
                CustomId.Build(SetSquadId, user.Id.ToString()),
                _guildData.Squads.Select(s => new SelectMenuOptionBuilder(s.Name, $"{s.Id}")).ToList())
            .WithButton(
                "Add a new squad",
                CustomId.Build(InputSquadId, user.Id.ToString()))
            .Build();
    }

    [SlashCommand("squad", "Set squad for a user")]
    public async Task SquadAsync(
        SocketUser user)
    {
        if (user is SocketGuildUser guildUser)
        {
            if (guildUser.GuildPermissions.Administrator)
                await RespondAsync("You cannot assign a squad to this user");
            else
                await RespondAsync($"Quel squad pour {user.Mention}?", components: GetSquadChoiceComponent(guildUser));
        }
        else
        {
            await RespondAsync("You need to be in the server to assign a squad");
        }
    }

    [ComponentInteraction($"{SetSquadId}-*")]
    public async Task SetSquad(string userId, string squadId)
    {
        await Context.Channel.DeleteMessageAsync(((IComponentInteraction)Context.Interaction).Message);

        var guild = Context.Guild;
        var guildUser = guild.GetUser(ulong.Parse(userId));

        var squad = guild.GetRole(ulong.Parse(squadId));

        await guildUser.RemoveRolesAsync(_guildData.Squads);

        await guildUser.AddRoleAsync(squad);

        await ReplyAsync($"{guildUser.Mention} a été ajouté.e à {squad.Name}");
    }

    [ComponentInteraction($"{InputSquadId}-*")]
    public async Task InputSquad(string userId)
    {
        await Context.Channel.DeleteMessageAsync(((IComponentInteraction)Context.Interaction).Message);
        await RespondWithModalAsync<AddSquadModal>($"{CreateSquadId}-{userId}");
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