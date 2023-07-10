using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class AddRankModule : ModuleBase<SocketCommandContext>
{
    public const string SetRankId = "setrank";

    private readonly DiscordSocketClient _client;
    private readonly GuildData _guildData;

    public AddRankModule(DiscordSocketClient client,
            GuildData guildData,
            TriggerMapper triggerMapper)
    {
        _client = client;
        _guildData = guildData;
        triggerMapper.AddButtonTrigger(SetRankId, SetRank);
    }

    [Command("rank")]
    [Summary("Set rank for a user")]
    public async Task RankAsync(
        [Summary("The user to get info from")] SocketGuildUser user)
    {
        if (user == null) throw new Exception("User was null");
        await user.RemoveRolesAsync(new[] { _guildData.BronzeRole, _guildData.SilverRole, _guildData.GoldRole, _guildData.DiamondRole });

        var component = new ComponentBuilder()
            .WithButton("Bronze", customId: $"{SetRankId}-{user.Id}-bronze", style: ButtonStyle.Primary, emote: Emoji.Parse(":key:"))
            .WithButton("Silver", customId: $"{SetRankId}-{user.Id}-silver", style: ButtonStyle.Primary, emote: Emoji.Parse(":crossed_swords:"))
            .WithButton("Gold", customId: $"{SetRankId}-{user.Id}-gold", style: ButtonStyle.Primary, emote: Emoji.Parse(":coin:"))
            .WithButton("Diamond", customId: $"{SetRankId}-{user.Id}-diamond", style: ButtonStyle.Primary, emote: Emoji.Parse(":gem:"))
            .Build();
        await ReplyAsync($"Quel rang pour {user.Mention}?", components: component);
    }

    public async Task SetRank(SocketMessageComponent component)
    {
        if (!component.GuildId.HasValue)
        {
            await component.Channel.SendMessageAsync("You can only do this in a channel");
            return;
        }
        var arguments = component.Data.CustomId.Split('-');
        var guildUser = _client.GetGuild(component.GuildId.Value)
                    .GetUser(ulong.Parse(arguments[1]));

        var newRole = arguments[2] switch
        {
            "bronze" => _guildData.BronzeRole,
            "silver" => _guildData.SilverRole,
            "gold" => _guildData.GoldRole,
            "diamond" => _guildData.DiamondRole,
            _ => throw new Exception("Role not found")
        };
        await guildUser.AddRoleAsync(newRole);

        await component.RespondAsync($"Le rang {newRole.Name} a été assigné à {guildUser.Mention}");
        await component.Channel.DeleteMessageAsync(component.Message.Id);
    }
}