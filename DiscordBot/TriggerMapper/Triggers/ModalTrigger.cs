using Discord.WebSocket;

namespace DiscordBot.TriggerMapper.Triggers;

public delegate Task ModalTriggerDelegate(SocketMessageComponent component);

public class ModalTrigger : ITriggerWithId
{
    public ModalTrigger(ModalTriggerDelegate @delegate, string id)
    {
        Delegate = @delegate;
        Id = id;
    }

    public string Id { get; }

    public ModalTriggerDelegate Delegate { get; }
    Delegate ITrigger.Delegate => Delegate;
}