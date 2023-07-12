namespace DiscordBot.TriggerMapper;

// CustomId is string that is passed into components such as Buttons or SelectMenus.
// We build the CustomId by concatenating all the arguments needed to understand what needs
// to be done when that component is triggered
// This class is a Helper that handles that

public static class CustomId
{
    public static string Build(string triggerId, params string[] arguments)
    {
        return string.Join('-', arguments.Prepend(triggerId));
    }

    public static (string, string[]) Parse(string customId)
    {
        var strings = customId.Split('-');
        return (strings.First(), strings.Skip(1).ToArray());
    }

    public static string GetTriggerId(string customId)
    {
        return customId[..customId.IndexOf('-')];
    }
}