using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.VisualBasic;

// Handles the logic for receiving message and executing the appropriate command
namespace DiscordBot.Core;

public class InteractionMapper
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;

    private InteractionService _interactionService;

    public InteractionMapper(DiscordSocketClient client, InteractionService interactionService, IServiceProvider services)
    {
        _client = client;
        _services = services;
        _interactionService = interactionService;
    }

    public Task MapCommands()
    {

        //_client.MessageReceived += HandleCommandAsync;
        _client.InteractionCreated += HandleInteraction;

        // Map all commands contained within the assembly
        _client.Ready += async () =>
        {
            await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);
            await _interactionService.RegisterCommandsGloballyAsync();
        };

        return Task.CompletedTask;
    }

    // private async Task HandleUserJoinedAsync(SocketGuildUser user)
    // {
    //     var message =  await user.SendMessageAsync($"Bienvenue sur le Discord Codebusters!");
    //     await _commands.ExecuteAsync(new CommandContext(_client, message!), UserJoinedJourneyBase.UserJoinedCommand, _services);
    // }

    private async Task HandleInteraction (SocketInteraction interaction)
    {
        try
        {
            // create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // if a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if(interaction.Type == InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }

    // private async Task HandleCommandAsync(SocketMessage messageParam)
    // {
    //     // Don't process the command if it was a system message
    //     var message = messageParam as SocketUserMessage;
    //     if (message == null) return;
    //
    //     // We don't want the bot to respond to itself or other bots.
    //     if (message.Author.IsBot) return;
    //
    //     // Create a number to track where the prefix ends and the command begins
    //     var argPos = 0;
    //
    //     // Determine if the message is a command based on the prefix and make sure no bots trigger commands
    //     if (message.HasStringPrefix("!", ref argPos) ||
    //         message.HasMentionPrefix(_client.CurrentUser, ref argPos))
    //     {
    //         // Create a WebSocket-based command context based on the message
    //         var context = new CommandContext(_client, message);
    //
    //         // Execute the command with the command context we just
    //         // created, along with the service provider.
    //         var result = await _commands.ExecuteAsync(
    //             context,
    //             argPos,
    //             _services);
    //
    //         // This does not catch errors from commands with 'RunMode.Async',
    //         // subscribe a handler for '_commands.CommandExecuted' to see those.
    //         if (!result.IsSuccess)
    //             await message.Channel.SendMessageAsync(result.ErrorReason);
    //     }
    // }
}