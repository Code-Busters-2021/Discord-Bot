using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.TriggerMapper;

namespace DiscordBot.Journeys;

public abstract class JourneyBase : ModuleBase<SocketCommandContext>
{
    protected JourneyBase(TriggerMapper.TriggerMapper mapper)
    {
        // Get TriggerAttributes, and hook the method to the TriggerMapper 
        foreach (var method in GetType().GetMethods())
        {
            var trigger = method.GetCustomAttribute<TriggerAttribute>();
            if (trigger == null) continue;


            switch (trigger.Type)
            {
                case TriggerType.Button:
                case TriggerType.SelectMenu:

                    async Task InvokeTrigger(SocketMessageComponent component)
                    {
                        await (Task)method.Invoke(this, new object?[] { component })!;
                    }

                    mapper.AddTrigger(trigger.TriggerId, InvokeTrigger);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"TriggerType is not handled: {trigger.Type}");
            }
        }
    }
}