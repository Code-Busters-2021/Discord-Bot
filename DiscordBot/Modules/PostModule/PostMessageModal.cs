// Defines the modal that will be sent.

using Discord;
using Discord.Interactions;

namespace DiscordBot.Modules.PostModule;

public class PostMessageModal : IModal
{
    // Strings with the ModalTextInput attribute will automatically become components.
    [InputLabel("Contenu du message")]
    [ModalTextInput("message_content", TextInputStyle.Paragraph,
        "Bonjour,\nComme vous le savez tous ...\nC'est pour cela que ...\nAinsi ...")]
    public string Contenu { get; set; }

    public string Title => "Poster un message";
}