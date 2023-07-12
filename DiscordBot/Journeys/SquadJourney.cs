using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.TriggerMapper;

namespace DiscordBot.Journeys;

public class CommandsJourney : JourneyBase<CommandContext>
{
    public const string SetSquadId = "setsquad";


    private readonly DiscordSocketClient _client;
    private readonly GuildData _guildData;

    public CommandsJourney(DiscordSocketClient client,
        GuildData guildData,
        TriggerMapper.TriggerMapper mapper) : base(mapper)
    {
        _client = client;
        _guildData = guildData;
    }

    [Command("squad")]
    [Summary("Set squad for a user")]
    public async Task SquadAsync(
        [Summary("The user to get info from")] SocketGuildUser user)
    {
        if (user == null) throw new Exception("not working");
        var squads = _guildData.GetSquads().ToList();
        await user.RemoveRolesAsync(squads);

        var component = new ComponentBuilder().WithSelectMenu(
                CustomId.Build(SetSquadId, user.Id.ToString()),
                squads.Select(s => new SelectMenuOptionBuilder(s.Name, $"{s.Id}")).ToList())
            .Build();
        await ReplyAsync($"Quel squad pour {user.Mention}?", components: component);
    }

    [Trigger(SetSquadId)]
    public async Task SetSquad(SocketMessageComponent component)
    {
        if (!component.GuildId.HasValue)
        {
            await component.Channel.SendMessageAsync("You can only do this in a channel");
            return;
        }

        var (_, arguments) = CustomId.Parse(component.Data.CustomId);
        var guild = _client.GetGuild(component.GuildId.Value);
        var guildUser = guild.GetUser(ulong.Parse(arguments[0]));

        var squadId = component.Data.Values.First();
        var squad = guild.GetRole(ulong.Parse(squadId));

        await guildUser.AddRoleAsync(squad);

        await component.Channel.SendMessageAsync($"{guildUser.Mention} a été ajouté.e à {squad.Name}");
        await component.Channel.DeleteMessageAsync(component.Message.Id);
    }
}