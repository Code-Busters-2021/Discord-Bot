using Discord;
using Discord.WebSocket;

public class GuildData
{

    private readonly DiscordSocketClient _client;
    private readonly SocketGuild _guild;

    public GuildData(DiscordSocketClient client)
    {
        _client = client;
        _guild = _client.Guilds.Where(guild => guild.Name == GlobalConfiguration.Instance.GuildName).First();
        ExtractRoles();
    }
    public ulong GuildId => _guild.Id;


    public IRole GoldRole;
    public IRole SilverRole;
    public IRole BronzeRole;
    public IRole DiamondRole;
    private void ExtractRoles()
    {
        foreach (IRole role in _guild.Roles)
        {
            switch (role.Name)
            {
                case "Gold":
                    GoldRole = role;
                    break;
                case "Silver":
                    SilverRole = role;
                    break;
                case "Bronze":
                    BronzeRole = role;
                    break;
                case "Diamond":
                    DiamondRole = role;
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