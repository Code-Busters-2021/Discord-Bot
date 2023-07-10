using Discord.WebSocket;

namespace DiscordBot.TriggerMapper;

public delegate Task Trigger(SocketMessageComponent component);

// Handles the logic for receiving events and triggering the appropriate action
public class TriggerMapper
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;

    // Retrieve client and CommandService instance via ctor
    public TriggerMapper(DiscordSocketClient client, IServiceProvider services)
    {
        _client = client;
        _services = services;
        MapEvents();
    }

    private readonly Dictionary<string, Trigger> _buttonTriggers = new();
    public void AddButtonTrigger(string triggerId, Trigger trigger)
    {
        _buttonTriggers.TryAdd(triggerId, trigger);
    }

    private readonly Dictionary<string, Trigger> _selectMenuTriggers = new();
    public void AddSelectMenuTrigger(string triggerId, Trigger trigger)
    {
        _selectMenuTriggers.TryAdd(triggerId, trigger);
    }

    private void MapEvents()
    {
        // Hook the events into our triggers
        _client.ButtonExecuted += HandleButtonAsync;
        _client.SelectMenuExecuted += HandleSelectMenuAsync;
    }


    private async Task HandleButtonAsync(SocketMessageComponent component)
    {
        var customId = component.Data.CustomId ?? throw new Exception("ComponentId is null");
        var triggerId = CustomId.GetTriggerId(customId);

        if (_buttonTriggers.TryGetValue(triggerId, out var trigger))
            await trigger.Invoke(component);
        else
            throw new Exception($"TriggerId not recognized: {triggerId}");
    }

    private async Task HandleSelectMenuAsync(SocketMessageComponent component)
    {
        var customId = component.Data.CustomId ?? throw new Exception("ComponentId is null");
        var triggerId = CustomId.GetTriggerId(customId);

        if (_selectMenuTriggers.TryGetValue(triggerId, out var trigger))
            await trigger.Invoke(component);
        else
            throw new Exception($"TriggerId not recognized: {triggerId}");
    }
}