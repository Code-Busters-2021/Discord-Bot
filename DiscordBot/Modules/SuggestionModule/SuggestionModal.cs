// Defines the modal that will be sent.

using Discord;
using Discord.Interactions;

namespace DiscordBot.Modules.PostModule;

public class SuggestionModal : IModal
{
    // Strings with the ModalTextInput attribute will automatically become components.
    [InputLabel("Suggestion")]
    [ModalTextInput("message_content", TextInputStyle.Paragraph,
        "Bonjour,\nComme vous le savez tous ...\nC'est pour cela que ...\nAinsi ...")]
    public string Content { get; set; } = null!;

    public string Title => "Suggestion";
}