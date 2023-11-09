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


    private List<IRole> _gradeRoles;
    public List<IRole> _squadRoles { get; private set; }

    public GuildData(DiscordSocketClient client, IConfiguration configuration, SquadNameChecker squadNameChecker)
    {
        _squadNameChecker = squadNameChecker;
        _clientCurrentUser = client.CurrentUser;
        Guild = client.Guilds.First(guild =>
            guild.Id == ulong.Parse(configuration["GuildId"]));
        ExtractRoles(configuration);
        ExtractAnonymousPostChannels();
    }

    public List<ITextChannel> PostMessageChannels { get; private set; }

    public IReadOnlyList<IRole> GradeRoles => _gradeRoles;
    public IReadOnlyList<IRole> SquadRoles => _squadRoles;


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
        _gradeRoles = configuration.GetSection("Grades").Get<string[]>()
            .Select(section => Guild.Roles.FirstOrDefault(role => role.Name == section) as IRole
                               ?? throw new Exception($"Role not found in the guild: {section}")).ToList();
    }

    public void UpdateSquads()
    {
        _squadRoles = new List<IRole>();
        foreach (IRole role in Guild.Roles)
            if (_squadNameChecker.CheckName(role.Name))
                _squadRoles.Add(role);
    }
}

#pragma warning restore CS8618