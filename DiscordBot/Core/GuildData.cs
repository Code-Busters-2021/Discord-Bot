using Discord;
using Discord.WebSocket;
using DiscordBot.Modules.SquadModule;
using Microsoft.Extensions.Configuration;

#pragma warning disable CS8618

namespace DiscordBot.Core;

// Handles data extracted from the codebusters guild
public class GuildData
{
    public IEnumerable<ITextChannel> PostChannels => _postChannels;
    public IEnumerable<IRole> GradeRoles => _gradeRoles;
    public IEnumerable<IRole> SquadRoles => _squadRoles;

    private readonly SocketSelfUser _clientCurrentUser;
    private readonly SquadNameChecker _squadNameChecker;
    public readonly SocketGuild Guild;
    private List<IRole> _gradeRoles;
    private List<IRole> _squadRoles;
    private List<ITextChannel> _postChannels;

    public GuildData(DiscordSocketClient client, IConfiguration configuration, SquadNameChecker squadNameChecker)
    {
        _squadNameChecker = squadNameChecker;
        _clientCurrentUser = client.CurrentUser;
        Guild = client.Guilds.First(guild =>
            guild.Id == ulong.Parse(configuration["GuildId"]));
        ExtractRoles(configuration);
        ExtractPostChannels(configuration);
        ExtractSquads();
    }

    private void ExtractPostChannels(IConfiguration configuration)
    {
        var postChannels = configuration.GetSection("PostChannels").Get<HashSet<ulong>>();
        if (postChannels == null || postChannels.Count == 0)
            throw new Exception("Could not load any post channels");
        _postChannels = Guild.Channels
            .OfType<ITextChannel>()
            .Where(channel => postChannels.Contains(channel.Id))
            .ToList();
    }

    private void ExtractRoles(IConfiguration configuration)
    {
        var gradesRolesNames = configuration.GetSection("Grades").Get<string[]>()
                               ?? throw new Exception("Roles not found in config");
        _gradeRoles = gradesRolesNames
            .Select(section => Guild.Roles.FirstOrDefault(role => role.Name == section) as IRole
                               ?? throw new Exception($"Role not found in the guild: {section}")).ToList();
    }

    public void ExtractSquads()
    {
        _squadRoles = new List<IRole>();
        foreach (IRole role in Guild.Roles)
            if (_squadNameChecker.CheckName(role.Name))
                _squadRoles.Add(role);
    }
}

#pragma warning restore CS8618