using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class CommandsModule : ModuleBase<SocketCommandContext>
{
    public const string SetRank = "setrank";
    public const string SetSquad = "setsquad";

    private IServiceProvider _services;

    public CommandsModule(IServiceProvider services)
    {
        _services = services;
    }

    [Command("rank")]
    [Summary("Set rank for a user")]
    [Alias("set-rank")]
    public async Task RankAsync(
        [Summary("The user to get info from")] SocketGuildUser user)
    {
        if (user == null) throw new Exception("User was null");
        var guildData = _services.GetRequiredService<GuildData>();
        await user.RemoveRolesAsync(new[] { guildData.BronzeRole, guildData.SilverRole, guildData.GoldRole });

        var component = new ComponentBuilder()
            .WithButton("Bronze", customId: $"{SetRank}-{user.Id}-bronze", style: ButtonStyle.Primary, emote: Emoji.Parse(":key:"))
            .WithButton("Silver", customId: $"{SetRank}-{user.Id}-silver", style: ButtonStyle.Primary, emote: Emoji.Parse(":crossed_swords:"))
            .WithButton("Gold", customId: $"{SetRank}-{user.Id}-gold", style: ButtonStyle.Primary, emote: Emoji.Parse(":coin:"))
            .WithButton("Diamond", customId: $"{SetRank}-{user.Id}-diamond", style: ButtonStyle.Primary, emote: Emoji.Parse(":gem:"))
            .Build();
        await ReplyAsync($"Quel rang pour {user.Mention}?", components: component);
    }

    [Command("squad")]
    [Summary("Set squad for a user")]
    public async Task SquadAsync(
        [Summary("The user to get info from")] SocketGuildUser user)
    {
        if (user == null) throw new Exception("not working");
        var squads = _services.GetRequiredService<GuildData>().GetSquads();
        var component = new ComponentBuilder()
            .WithSelectMenu($"{SetSquad}-{user.Id}", squads.Select(s => new SelectMenuOptionBuilder(s.Name, $"{s.Id}")).ToList())
            .Build();
        await ReplyAsync($"Quel squad pour {user.Mention}?", components: component);
    }



    [Command("error")]
    [Summary("Generate an error")]
    public Task ErrorAsync()
    {
        throw new Exception("Successful error");
    }
}