namespace DiscordBot.TriggerMapper.Triggers;

public interface ITriggerWithId : ITrigger
{
    public string Id { get; }
}