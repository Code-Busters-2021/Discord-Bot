using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        TriggerAttribute.ExtractTriggers(this);
    }

    private Dictionary<string, Trigger> _buttonTriggers = new();
    public void AddButtonTrigger(string triggerId, Trigger trigger)
    {
        _buttonTriggers.TryAdd(triggerId, trigger);
    }

    private Dictionary<string, Trigger> _selectMenuTriggers = new();
    public void AddSelectMenuTrigger(string triggerId, Trigger trigger)
    {
        _selectMenuTriggers.TryAdd(triggerId, trigger);
    }

    private Task MapEvents()
    {
        // Hook the events into our triggers
        _client.ButtonExecuted += HandleButtonAsync;
        _client.SelectMenuExecuted += HandleSelectMenuAsync;
        return Task.CompletedTask;
    }


    private async Task HandleButtonAsync(SocketMessageComponent component)
    {
        var componentId = component.Data.CustomId ?? throw new Exception("ComponentId is null");
        var triggerId = GetTriggerId(componentId);

        if (_buttonTriggers.TryGetValue(triggerId, out Trigger? trigger))
            await trigger.Invoke(component);
        else
            throw new Exception($"TriggerId not recognized: {triggerId}");
    }

    private async Task HandleSelectMenuAsync(SocketMessageComponent component)
    {
        var componentId = component.Data.CustomId ?? throw new Exception("ComponentId is null");
        var triggerId = GetTriggerId(componentId);

        if (_buttonTriggers.TryGetValue(triggerId, out Trigger? trigger))
            await trigger.Invoke(component);
        else
            throw new Exception($"TriggerId not recognized: {triggerId}");
    }

    private string GetTriggerId(string componentId) => componentId. Split('-').First();
}