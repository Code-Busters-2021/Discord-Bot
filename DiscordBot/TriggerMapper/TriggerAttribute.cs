using System;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
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

    public static void ExtractTriggers(TriggerMapper mapper)
    {
        foreach (var method in Assembly.GetCallingAssembly().GetTypes()
                    .SelectMany(type => type.GetMethods()))
        {
            var trigger = method.GetCustomAttribute<TriggerAttribute>();
            if (trigger == null) continue;
            switch (trigger.Type){
                case TriggerType.Button:
                    mapper.AddButtonTrigger(trigger.TriggerId, null);
                    break;
                case TriggerType.SelectMenu:
                    mapper.AddSelectMenuTrigger(trigger.TriggerId, null);
                    break;
            }
        }
    }
}