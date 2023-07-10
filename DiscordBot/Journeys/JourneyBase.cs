using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.TriggerMapper;

namespace DiscordBot.Journeys;

public abstract class JourneyBase : ModuleBase<SocketCommandContext>
{
    protected JourneyBase(TriggerMapper.TriggerMapper mapper)
    {
        foreach (var method in GetType().GetMethods())
        {
            var trigger = method.GetCustomAttribute<TriggerAttribute>();
            if (trigger == null) continue;

            async Task InvokeTrigger(SocketMessageComponent component) => await (Task)method.Invoke(this, new[] { component })!;
            switch (trigger.Type)
            {
                case TriggerType.Button:
                    mapper.AddButtonTrigger(trigger.TriggerId, InvokeTrigger);
                    break;
                case TriggerType.SelectMenu:
                    mapper.AddSelectMenuTrigger(trigger.TriggerId, InvokeTrigger);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"TriggerType is not handled: {trigger.Type}");
            }
        }
    }
}