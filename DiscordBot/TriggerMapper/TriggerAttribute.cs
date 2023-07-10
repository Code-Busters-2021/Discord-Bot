namespace DiscordBot.TriggerMapper;

[AttributeUsage(AttributeTargets.Method)]
public class TriggerAttribute : Attribute
{
    public readonly TriggerType Type;
    public readonly string TriggerId;

    public TriggerAttribute(TriggerType type, string triggerId)
    {
        Type = type;
        TriggerId = triggerId;
    }
}