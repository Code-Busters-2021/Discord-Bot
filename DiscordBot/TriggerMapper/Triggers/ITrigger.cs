namespace DiscordBot.TriggerMapper.Triggers;

public interface ITrigger
{
    public Delegate Delegate { get; }
}

public static class TriggerExtensions
{
    public static Delegate GetDelegateType(this ITrigger trigger)
    {
        return trigger.Delegate;
    }
}