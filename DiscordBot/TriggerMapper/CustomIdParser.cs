using System.Linq;

// CustomId is string that is passed into components such as Buttons or SelectMenus.
// We build the CustomId by concatenating all the arguments needed to understand what needs
// to be done when that component is triggered
// This class is a Helper that handles that
public static class CustomIdParser
{
    public static string BuildCustomId(string triggerId, params string[] arguments)
        => string.Concat(arguments.Prepend(triggerId), '-');

    public static (string, string[]) BuilParseCutomId(string customId)
    {
        var strings = customId.Split('-');
        return (strings.First(), strings.Skip(1).ToArray());
    }

}