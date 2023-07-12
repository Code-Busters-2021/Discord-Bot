using Discord.WebSocket;

namespace DiscordBot.TriggerMapper.Triggers;

public delegate Task UserJoinTriggerDelegate(SocketMessageComponent component);

public class UserJoinTrigger : ITrigger
{
    public UserJoinTrigger(UserJoinTriggerDelegate @delegate)
    {
        Delegate = @delegate;
    }

    public UserJoinTriggerDelegate Delegate { get; }
    Delegate ITrigger.Delegate => Delegate;

}