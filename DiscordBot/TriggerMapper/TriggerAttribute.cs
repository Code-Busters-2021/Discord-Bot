namespace DiscordBot.TriggerMapper;

[AttributeUsage(AttributeTargets.Method)]
public class TriggerAttribute : Attribute
{
    public string TriggerId { get; }
    public TriggerAttribute(string triggerId)
    {
        TriggerId = triggerId;
    }
}
