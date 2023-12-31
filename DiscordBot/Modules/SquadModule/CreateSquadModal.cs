using Discord.Interactions;

namespace DiscordBot.Modules.SquadModule;

public class CreateSquadModal : IModal
{
    // Strings with the ModalTextInput attribute will automatically become components.
    [InputLabel("Nom de la nouvelle squad")]
    [ModalTextInput("squad_name", placeholder: "La Squad de Jean Dupond", maxLength: 30)]
    public string Name { get; set; } = null!;

    public string Title => "Creation de squad";
}