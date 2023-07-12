using Discord.WebSocket;

namespace DiscordBot.TriggerMapper.Triggers;

public delegate Task ComponentTriggerDelegate(SocketMessageComponent component);

public class ComponentTrigger : ITriggerWithId
{
    public ComponentTrigger(ComponentTriggerDelegate @delegate, string id)
    {
        Id = id;
        Delegate = @delegate;
    }

    public string Id { get; }

    public ComponentTriggerDelegate Delegate { get; }
    Delegate ITrigger.Delegate => Delegate;
}