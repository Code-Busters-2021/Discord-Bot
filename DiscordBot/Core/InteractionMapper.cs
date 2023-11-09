using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

// Handles the logic for receiving message and executing the appropriate command
namespace DiscordBot.Core;

public class InteractionMapper
{
    private readonly DiscordSocketClient _client;

    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _services;

    public InteractionMapper(DiscordSocketClient client, InteractionService interactionService,
        IServiceProvider services)
    {
        _client = client;
        _services = services;
        _interactionService = interactionService;
    }

    public Task MapCommands()
    {
        _client.InteractionCreated += HandleInteraction;


        // Map all commands contained within the assembly
        _client.Ready += async () =>
        {
            await _client.BulkOverwriteGlobalApplicationCommandsAsync(Array.Empty<ApplicationCommandProperties>());

            await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);
            await _interactionService.RegisterCommandsGloballyAsync();
        };

        return Task.CompletedTask;
    }

    private async Task HandleInteraction(SocketInteraction interaction)
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
            if (interaction.Type == InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
        }
    }
}