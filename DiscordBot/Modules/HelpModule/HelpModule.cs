using Discord.Interactions;
using DiscordBot.Core;
using DiscordBot.Modules.ModuleBase;

namespace DiscordBot.Modules.HelpModule;

public class HelpModule : OverlayInteractionModuleBase<SocketInteractionContext>
{
    private readonly string _helpMessage;
    private readonly string _helpModerateurMessage;

    public HelpModule(GuildData guildData) : base(guildData)
    {
        _helpMessage = File.ReadAllText("Configuration/help.txt");
        _helpModerateurMessage = File.ReadAllText("Configuration/help-moderateur.txt");
    }

    [SlashCommand("help", "Liste des commandes disponibles")]
    public async Task GetHelpAsync()
    {
        await RespondAsync(_helpMessage, ephemeral: true);
    }

    [SlashCommand("help-moderateur", "Liste des commandes modérateur disponibles")]
    public async Task GetHelpModerateurAsync()
    {
        await RespondAsync(_helpModerateurMessage, ephemeral: true);
    }
}