using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;

namespace DiscordBot.Modules;

public class RankModule : InteractionModuleBase<SocketInteractionContext>
{
    private const string SetRankId = "setrank";

    private readonly GuildData _guildData;

    public RankModule(GuildData guildData)
    {
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

    [SlashCommand("rank", "Set rank for a user")]
    public async Task RankAsync(SocketUser user)
    {
        if (user is SocketGuildUser guildUser)
        {
            if (guildUser.GuildPermissions.Administrator)
                await RespondAsync("You cannot assign a rank to this user");
            else
                await RespondAsync($"Quel rang pour {user.Mention}?", components: GetRankChoiceComponent(guildUser));
        }
        else
        {
            await RespondAsync("You need to be in the server to use this command");
        }
    }


    [ComponentInteraction($"{SetRankId}-*-*")]
    public async Task SetRank(string userId, string rank)
    {
        await Context.Channel.DeleteMessageAsync(((IComponentInteraction)Context.Interaction).Message);

        var guildUser = Context.Guild.GetUser(ulong.Parse(userId));

        await guildUser.RemoveRolesAsync(new[]
            { _guildData.BronzeRole, _guildData.SilverRole, _guildData.GoldRole, _guildData.DiamondRole });

        var newRole = rank switch
        {
            "bronze" => _guildData.BronzeRole,
            "silver" => _guildData.SilverRole,
            "gold" => _guildData.GoldRole,
            "diamond" => _guildData.DiamondRole,
            _ => throw new Exception("Role not found")
        };
        await guildUser.AddRoleAsync(newRole);

        await ReplyAsync($"Le rang {newRole.Name} a été assigné à {guildUser.Mention}");
    }
}