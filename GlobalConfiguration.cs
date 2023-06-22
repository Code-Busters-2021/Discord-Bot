public class GlobalConfiguration
{

    public string GuildName { get; init; } = "Codebusters-Preprod";
    public string Environment { get; init; } = "Preprod";
    public string AuthenticationToken { get; init; } = "";

    private static GlobalConfiguration? _instance;
    public static GlobalConfiguration Instance
    {
        get
        {
            _instance ??= new GlobalConfiguration();
            return _instance;
        }
    }
}