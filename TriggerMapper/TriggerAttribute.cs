public class TriggerAttribute : Attribute
{
    public readonly TriggerType Type;
    public readonly string TriggerId;
    public readonly string[] Arguments;

    public string ComponentCustomId => CustomIdParser.BuildCustomId(TriggerId, Arguments);

    public TriggerAttribute(TriggerType type, string triggerId, params string[] arguments)
    {
        Type = type;
        TriggerId = triggerId;
        Arguments = arguments;
    }
}