using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;

namespace DiscordBot.Modules.ModuleBase;

// Over
public abstract class OverlayInteractionModuleBase<T> : InteractionModuleBase<T>
    where T : class, IInteractionContext
{
    protected readonly GuildData GuildData;
    protected HashSet<ulong>? AllowedRoles = null; // Keep null to allow everyone

    protected HashSet<ulong>? DeniedRoles = null; // Keep null to deny no one

    protected OverlayInteractionModuleBase(GuildData guildData)
    {
        GuildData = guildData;
    }

    private bool IsUserAllowed(SocketGuildUser user)
    {
        if (DeniedRoles != null)
            if (user.Roles.Any(role => DeniedRoles.Contains(role.Id)))
                return false;
        if (AllowedRoles != null) return user.Roles.Any(role => AllowedRoles.Contains(role.Id));

        return true;
    }

    protected async Task RespondAndThrowIfUserDenied()
    {
        var user = GuildData.ToGuildUser(Context.User);
        if (IsUserAllowed(user)) return;
        await RespondAsync("You are not allowed to use this command", ephemeral: true);
        throw new Exception("User was denied");
    }
}