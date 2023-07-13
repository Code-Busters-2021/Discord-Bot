using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;
using Microsoft.Extensions.Configuration.Json;

namespace DiscordBot;

public class Program
{
    // Program entry point
    static Task Main()
    {
        // Call the Program constructor, followed by the 
        // MainAsync method and wait until it finishes (which should be never).
        return new Program().MainAsync();
    }

    private readonly DiscordSocketClient _client;

    // Keep the CommandService and DI container around for use with commands.
    // These two types require you install the Discord.Net.Commands package.
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private IConfiguration _configuration;

    public Program()
    {
        _configuration = Configuration.BuildConfiguration();

        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            GatewayIntents = GatewayIntents.All,
        });

        _commands = new CommandService(new CommandServiceConfig
        {
            LogLevel = LogSeverity.Info,
            CaseSensitiveCommands = false,
        });

        // Subscribe the logging handler to both the client and the CommandService.
        _client.Log += Log;
        _commands.Log += Log;

        // Setup your DI container.
        _services = ConfigureServices();

    }

    // If any services require the client, or the CommandService, or something else you keep on hand,
    // pass them as parameters into this method as needed.
    // If this method is getting pretty long, you can seperate it out into another file using partials.
    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<DiscordSocketClient>(_ => _client);
        services.AddSingleton(_configuration);

        services.AddSingleton<GuildData>();

        services.AddSingleton<InteractionService>();
        services.AddSingleton<InteractionMapper>();

        return services.BuildServiceProvider();
    }

    public static Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();

        return Task.CompletedTask;
    }

    private async Task MainAsync()
    {
        // Login and connect.
        await _client.LoginAsync(TokenType.Bot, _configuration["BotToken"]);
        await _client.StartAsync();

        await _services.GetRequiredService<InteractionMapper>().MapCommands();

        // Wait infinitely so your bot actually stays connected.
        await Task.Delay(Timeout.Infinite);
    }

}