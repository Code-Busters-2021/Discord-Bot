using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.TriggerMapper;

namespace DiscordBot.Journeys;

public abstract class JourneyBase<T> : ModuleBase<T>
    where T : class, ICommandContext
{
    protected JourneyBase(TriggerMapper.TriggerMapper mapper)
    {
        // Get TriggerAttributes, and hook the method to the TriggerMapper 
        foreach (var method in GetType().GetMethods())
        {
            var trigger = method.GetCustomAttribute<TriggerAttribute>();
            if (trigger == null) continue;

            if (HaveSameSignature(method, typeof(ComponentTrigger)))
            {
                async Task InvokeTrigger(SocketMessageComponent component)
                {
                    await (Task)method.Invoke(this, new object?[] { component })!;
                }

                mapper.AddComponentTrigger(trigger.TriggerId, InvokeTrigger);
            }
            else if (HaveSameSignature(method, typeof(ModalTrigger)))
            {
                async Task InvokeTrigger(SocketModal modal)
                {
                    await (Task)method.Invoke(this, new object?[] { modal })!;
                }

                mapper.AddModalTrigger(trigger.TriggerId, InvokeTrigger);
            }
            else
            {
                throw new Exception($"Method marked as Trigger, but signature was not recognized: {method.Name}");
            }
        }
    }

    private static bool HaveSameSignature(MethodBase method, Type delegateType)
    {
        return method.GetParameters().Select(p => p.ParameterType)
            .SequenceEqual(delegateType.GetMethod("Invoke")!.GetParameters().Select(p => p.ParameterType));
    }
}