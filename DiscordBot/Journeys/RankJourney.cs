using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.TriggerMapper;

namespace DiscordBot.Journeys;

public class RankJourney : JourneyBase<CommandContext>
{
    private const string SetRankId = "setrank";

    private readonly DiscordSocketClient _client;
    private readonly GuildData _guildData;

    public RankJourney(DiscordSocketClient client,
        GuildData guildData,
        TriggerMapper.TriggerMapper mapper) : base(mapper)
    {
        _client = client;
        _guildData = guildData;
    }

    public MessageComponent GetRankChoiceComponent(SocketGuildUser user)
    {
        return new ComponentBuilder()
            .WithButton("Bronze", CustomId.Build(SetRankId, user.Id.ToString(), "bronze"),
                ButtonStyle.Primary, Emoji.Parse(":key:"))
            .WithButton("Silver", CustomId.Build(SetRankId, user.Id.ToString(), "silver"),
                ButtonStyle.Primary, Emoji.Parse(":crossed_swords:"))
            .WithButton("Gold", CustomId.Build(SetRankId, user.Id.ToString(), "gold"),
                ButtonStyle.Primary, Emoji.Parse(":coin:"))
            .WithButton("Diamond", CustomId.Build(SetRankId, user.Id.ToString(), "diamond"),
                ButtonStyle.Primary, Emoji.Parse(":gem:"))
            .Build();
    }

    [Command("rank")]
    [Summary("Set rank for a user")]
    public async Task RankAsync(
        [Summary("The user to get info from")] SocketGuildUser user)
    {
        if (user == null) throw new Exception("User was null");
        await user.RemoveRolesAsync(new[]
            { _guildData.BronzeRole, _guildData.SilverRole, _guildData.GoldRole, _guildData.DiamondRole });
        
        await ReplyAsync($"Quel rang pour {user.Mention}?", components: GetRankChoiceComponent(user));
    }


    [Trigger(SetRankId)]
    public async Task SetRank(SocketMessageComponent component)
    {
        if (!component.GuildId.HasValue)
        {
            await component.Channel.SendMessageAsync("You can only do this in a channel");
            return;
        }

        var (_, arguments) = CustomId.Parse(component.Data.CustomId);
        var guildUser = _client.GetGuild(component.GuildId.Value)
            .GetUser(ulong.Parse(arguments[0]));

        var newRole = arguments[1] switch
        {
            "bronze" => _guildData.BronzeRole,
            "silver" => _guildData.SilverRole,
            "gold" => _guildData.GoldRole,
            "diamond" => _guildData.DiamondRole,
            _ => throw new Exception("Role not found")
        };
        await guildUser.AddRoleAsync(newRole);

        await component.Channel.SendMessageAsync($"Le rang {newRole.Name} a été assigné à {guildUser.Mention}");
        await component.Channel.DeleteMessageAsync(component.Message.Id);
    }
}