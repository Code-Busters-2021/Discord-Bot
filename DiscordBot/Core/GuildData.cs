using Discord;
using Discord.WebSocket;
using DiscordBot.Modules.SquadModule;

#pragma warning disable CS8618

namespace DiscordBot.Core;

// Handles data extracted from the codebusters guild
public class GuildData
{
    private readonly SocketSelfUser _clientCurrentUser;

    private readonly SquadNameChecker _squadNameChecker;

    public readonly SocketGuild Guild;


    private Dictionary<string, IRole> _importantRoles;

    public GuildData(DiscordSocketClient client, IConfiguration configuration, SquadNameChecker squadNameChecker)
    {
        _squadNameChecker = squadNameChecker;
        _clientCurrentUser = client.CurrentUser;
        Guild = client.Guilds.First(guild =>
            guild.Name == configuration["GuildName"]);
        ExtractRoles(configuration);
        ExtractAnonymousPostChannels();
    }

    public List<ITextChannel> PostMessageChannels { get; private set; }

    public IReadOnlyDictionary<string, IRole> ImportantRoles => _importantRoles;

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

    private void ExtractRoles(IConfiguration configuration)
    {
        UpdateSquads();
        _importantRoles = configuration.GetSection("Roles").GetChildren()
            .ToDictionary(section => section.Key,
                section => Guild.Roles.FirstOrDefault(role => role.Name == section.Value) as IRole
                           ?? throw new Exception($"Role not found in the guild: {section.Value}"));
    }

    public void UpdateSquads()
    {
        Squads = new List<IRole>();
        foreach (IRole role in Guild.Roles)
            if (_squadNameChecker.CheckName(role.Name))
                Squads.Add(role);
    }
}

#pragma warning restore CS8618