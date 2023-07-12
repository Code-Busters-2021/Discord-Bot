using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core;

namespace DiscordBot.Journeys.UserJoined;

public abstract class UserJoinedJourneyBase : JourneyBase<CommandContext>
{
    public const string UserJoinedCommand = "user-joined";

    private readonly DiscordSocketClient _client;
    private readonly GuildData _guildData;

    public UserJoinedJourneyBase(DiscordSocketClient client,
        GuildData guildData,
        TriggerMapper.TriggerMapper mapper) : base(mapper)
    {
        _client = client;
        _guildData = guildData;
    }

    // public abstract Task UserJoined();
}