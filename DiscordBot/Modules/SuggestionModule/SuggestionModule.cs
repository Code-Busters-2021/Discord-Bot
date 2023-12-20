using Discord;
using Discord.Interactions;
using DiscordBot.Core;
using DiscordBot.Modules.ModuleBase;
using DiscordBot.Modules.PostModule;

namespace DiscordBot.Modules.HelpModule;

public class SuggestionModule : OverlayInteractionModuleBase<SocketInteractionContext>
{
    private const string SuggestionId = "suggestion";

    public SuggestionModule(GuildData guildData) : base(guildData)
    {
    }

    [SlashCommand("suggestion", "Suggérer une nouvelle feature")]
    public async Task WriteSuggestionAsync()
    {
        await RespondWithModalAsync<SuggestionModal>($"{SuggestionId}");
    }

    [ModalInteraction($"{SuggestionId}")]
    public async Task PostSuggestion(SuggestionModal modal)
    {
        await DeferAsync();
        await GuildData.SuggestionChannel.SendMessageAsync(MakeSuggestionMessage(Context.User, modal.Content));
        await RespondAsync($"Le suggestion a bien étét recue, merci!", ephemeral: true);
    }

    private static string MakeSuggestionMessage(IMentionable user, string content)
        => $"Suggestion de {user.Mention} :" + Environment.NewLine + content;
}