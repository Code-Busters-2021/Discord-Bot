using Discord.WebSocket;

namespace DiscordBot.TriggerMapper;

public delegate Task Trigger(SocketMessageComponent component);

// Handles the logic for receiving events and triggering the appropriate action
public class TriggerMapper
{
    private readonly DiscordSocketClient _client;

    // Retrieve client and CommandService instance via ctor
    public TriggerMapper(DiscordSocketClient client)
    {
        _client = client;
        MapEvents();
    }

    private readonly Dictionary<string, Trigger> _triggers = new();
    public void AddTrigger(string triggerId, Trigger trigger)
    {
        _triggers.TryAdd(triggerId, trigger);
    }

    private void MapEvents()
    {
        // Hook the events into our triggers
        _client.ButtonExecuted += HandleTriggerAsync;
        _client.SelectMenuExecuted += HandleTriggerAsync;
    }


    private async Task HandleTriggerAsync(SocketMessageComponent component)
    {
        var customId = component.Data.CustomId ?? throw new Exception("ComponentId is null");
        var triggerId = CustomId.GetTriggerId(customId);

        if (_triggers.TryGetValue(triggerId, out var trigger))
            await trigger.Invoke(component);
        else
            throw new Exception($"TriggerId not recognized: {triggerId}");
    }

}