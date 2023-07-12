namespace DiscordBot.TriggerMapper;

// Handles the logic for receiving events and triggering the appropriate action
public interface ITriggerMapper
{
    void AddTrigger(string triggerId, Delegate trigger);
}