using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class CommandsModule : ModuleBase<SocketCommandContext>
{
    public const string SetSquadId = "setsquad";


    private readonly DiscordSocketClient _client;
    private GuildData _guildData;

    public CommandsModule(DiscordSocketClient client,
            GuildData guildData,
            TriggerMapper triggerMapper)
    {
        _client = client;
        _guildData = guildData;
        triggerMapper.AddButtonTrigger(SetSquadId, SetSquad);
    }

    [Command("squad")]
    [Summary("Set squad for a user")]
    public async Task SquadAsync(
        [Summary("The user to get info from")] SocketGuildUser user)
    {
        if (user == null) throw new Exception("not working");
        var squads = _guildData.GetSquads();
        await user.RemoveRolesAsync(squads);

        var component = new ComponentBuilder()
            .WithSelectMenu($"{SetSquadId}-{user.Id}", squads.Select(s => new SelectMenuOptionBuilder(s.Name, $"{s.Id}")).ToList())
            .Build();
        await ReplyAsync($"Quel squad pour {user.Mention}?", components: component);
    }

    
    public async Task SetSquad(SocketMessageComponent component)
    {
        if (!component.GuildId.HasValue)
        {
            await component.Channel.SendMessageAsync("You can only do this in a channel");
            return;
        }
        var arguments = component.Data.CustomId.Split('-');
        var guild = _client.GetGuild(component.GuildId.Value);
        var guildUser = guild.GetUser(ulong.Parse(arguments[1]));

        var squadId = component.Data.Values.First();
        var squad = guild.GetRole(ulong.Parse(squadId));

        await guildUser.AddRoleAsync(squad);

        await component.RespondAsync($"{guildUser.Mention} a été ajouté.e à {squad.Name}");
        await component.Channel.DeleteMessageAsync(component.Message.Id);
    }

    [Command("error")]
    [Summary("Generate an error")]
    public Task ErrorAsync()
    {
        throw new Exception("Successful error");
    }
}