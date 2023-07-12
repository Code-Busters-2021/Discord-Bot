using Discord.WebSocket;

namespace DiscordBot.TriggerMapper;

public delegate Task ComponentTrigger(SocketMessageComponent component);

public delegate Task ModalTrigger(SocketModal modal);

public delegate Task UserJoinTrigger(SocketGuildUser user);

// Handles the logic for receiving events and triggering the appropriate action
public class TriggerMapper
{
    private readonly DiscordSocketClient _client;

    private readonly Dictionary<string, ComponentTrigger> _componentTriggers = new();

    private readonly Dictionary<string, ModalTrigger> _modalTriggers = new();
    
    // Retrieve client and CommandService instance via ctor
    public TriggerMapper(DiscordSocketClient client)
    {
        _client = client;
        MapEvents();
    }

    public void AddComponentTrigger(string triggerId, ComponentTrigger componentTrigger)
    {
        _componentTriggers.TryAdd(triggerId, componentTrigger);
    }

    public void AddModalTrigger(string triggerId, ModalTrigger modalTrigger)
    {
        _modalTriggers.TryAdd(triggerId, modalTrigger);
    }

    private void MapEvents()
    {
        // Hook the events into our triggers
        _client.ButtonExecuted += HandleComponentTriggerAsync;
        _client.SelectMenuExecuted += HandleComponentTriggerAsync;
        _client.ModalSubmitted += HandleModalTriggerAsync;
    }

    private async Task HandleModalTriggerAsync(SocketModal modal)
    {
        var customId = modal.Data.CustomId ?? throw new Exception("ComponentId is null");
        var triggerId = CustomId.GetTriggerId(customId);

        if (_modalTriggers.TryGetValue(triggerId, out var trigger))
            await trigger.Invoke(modal);
        else
            throw new Exception($"TriggerId not recognized: {triggerId}");
    }


    private async Task HandleComponentTriggerAsync(SocketMessageComponent component)
    {
        var customId = component.Data.CustomId ?? throw new Exception("ComponentId is null");
        var triggerId = CustomId.GetTriggerId(customId);

        if (_componentTriggers.TryGetValue(triggerId, out var trigger))
            await trigger.Invoke(component);
        else
            throw new Exception($"TriggerId not recognized: {triggerId}");
    }

}