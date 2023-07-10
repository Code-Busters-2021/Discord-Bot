using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.TriggerMapper;

// Handles the logic for receiving message and executing the appropriate command
namespace DiscordBot.Core;

public class CommandMapper
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;

    public CommandMapper(DiscordSocketClient client, CommandService commands, IServiceProvider services)
    {
        _commands = commands;
        _client = client;
        _services = services;
    }

    private Dictionary<string, Trigger> _triggers = new();
    public void AddTrigger(string eventName, Trigger trigger)
    {
        _triggers.Add(eventName, trigger);
    }

    public Task MapCommands()
    {
        _client.MessageReceived += HandleCommandAsync;

        // Map all commands contained within the assembly
        _client.Ready += async () => await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
            services: _services);

        return Task.CompletedTask;
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // We don't want the bot to respond to itself or other bots.
        if (message.Author.IsBot) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (message.HasStringPrefix("!", ref argPos) ||
            message.HasMentionPrefix(_client.CurrentUser, ref argPos))
        {
            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);

            // This does not catch errors from commands with 'RunMode.Async',
            // subscribe a handler for '_commands.CommandExecuted' to see those.
            if (!result.IsSuccess)
                await message.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}