using Discord;
using Discord.WebSocket;

#pragma warning disable CS8618

namespace DiscordBot.Core;

// Handles data extracted from the codebusters guild
public class GuildData
{
    private readonly SocketSelfUser _clientCurrentUser;
    public readonly SocketGuild Guild;

    public GuildData(DiscordSocketClient client, IConfiguration configuration)
    {
        _clientCurrentUser = client.CurrentUser;
        Guild = client.Guilds.First(guild =>
            guild.Name == configuration[$"GuildData:Name:{configuration["Environment"]}"]);
        ExtractRoles();
        ExtractAnonymousPostChannels();
    }

    public List<ITextChannel> PostMessageChannels { get; private set; }

    public IRole ManagerRole { get; private set; }
    public IRole DiamondRole { get; private set; }
    public IRole GoldRole { get; private set; }
    public IRole SilverRole { get; private set; }
    public IRole BronzeRole { get; private set; }

    public List<IRole> Squads { get; private set; }

    public void ExtractAnonymousPostChannels()
    {
        PostMessageChannels = Guild.Channels
            .OfType<ITextChannel>()
            .Where(channel => channel.PermissionOverwrites
                .FirstOrDefault(overwrite => overwrite.TargetId == _clientCurrentUser.Id).Permissions
                .SendMessages == PermValue.Allow)
            .ToList();
    }

    private void ExtractRoles()
    {
        UpdateSquads();
        foreach (IRole role in Guild.Roles)
            switch (role.Name)
            {
                case "Diamond":
                    DiamondRole = role;
                    break;
                case "Gold":
                    GoldRole = role;
                    break;
                case "Silver":
                    SilverRole = role;
                    break;
                case "Bronze":
                    BronzeRole = role;
                    break;
                case "Manager":
                    ManagerRole = role;
                    break;
            }
    }

    public void UpdateSquads()
    {
        Squads = new List<IRole>();
        foreach (IRole role in Guild.Roles)
            if (role.Name.ToLower().Contains("squad"))
                Squads.Add(role);
    }
}

#pragma warning restore CS8618