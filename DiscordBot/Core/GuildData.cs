using Discord;
using Discord.WebSocket;

#pragma warning disable CS8618

namespace DiscordBot.Core;

// Handles data extracted from the codebusters guild
public class GuildData
{
    private readonly SocketGuild _guild;

    public GuildData(DiscordSocketClient client)
    {
        _guild = client.Guilds.First(guild => guild.Name == GlobalConfiguration.Instance.GuildName);
        ExtractRoles();
    }
    public ulong GuildId => _guild.Id;


    public IRole DiamondRole;
    public IRole GoldRole;
    public IRole SilverRole;
    public IRole BronzeRole;
    private void ExtractRoles()
    {
        foreach (IRole role in _guild.Roles)
        {
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
            }
        }
    }

    public IEnumerable<IRole> GetSquads()
    {
        foreach (IRole role in _guild.Roles)
        {
            if (role.Name.ToLower().Contains("squad"))
                yield return role;
        }
    }
}

#pragma warning restore CS8618