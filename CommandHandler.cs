using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;

    // Retrieve client and CommandService instance via ctor
    public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
    {
        _commands = commands;
        _client = client;
        _services = services;
    }

    public async Task InstallCommandsAsync()
    {
        // Hook the MessageReceived event into our command handler
        _client.MessageReceived += HandleCommandAsync;
        _client.ButtonExecuted += HandleButtonAsync;
        _client.SelectMenuExecuted += HandleSelectMenuAsync;

        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                        services: _services);
    }

    public async Task HandleSelectMenuAsync(SocketMessageComponent component){

        var componentId = component.Data.CustomId;
        if (componentId == null) return;
        if (componentId.StartsWith(CommandsModule.SetSquad)){

            if (!component.GuildId.HasValue)
            {
                await component.Channel.SendMessageAsync("You can only do this in a channel");
                return;
            }
            var parameters = componentId.Split('-');
            var guild = _client.GetGuild(component.GuildId.Value);
            var guildUser = guild.GetUser(ulong.Parse(parameters[1]));

            var guildData = _services.GetRequiredService<GuildData>();
            var squadId = component.Data.Values.First();
            var squad = guild.GetRole(ulong.Parse(squadId));

            await guildUser.AddRoleAsync(squad);

            await component.RespondAsync($"{guildUser.Mention} a été ajouté.e à {squad.Name}");
            await component.Channel.DeleteMessageAsync(component.Message.Id);
        }
    }

    public async Task HandleButtonAsync(SocketMessageComponent component)
    {
        var componentId = component.Data.CustomId;
        if (componentId == null) return;
        if (componentId.StartsWith(CommandsModule.SetRank))
        {
            if (!component.GuildId.HasValue)
            {
                await component.Channel.SendMessageAsync("You can only do this in a channel");
                return;
            }
            var parameters = componentId.Split('-');
            var guildUser = _client.GetGuild(component.GuildId.Value)
                        .GetUser(ulong.Parse(parameters[1]));

            var guildData = _services.GetRequiredService<GuildData>();
            var newRole = parameters[2] switch
            {
                "bronze" => guildData.BronzeRole,
                "silver" => guildData.SilverRole,
                "gold" => guildData.GoldRole,
                "gem" => guildData.DiamondRole,
                _ => throw new Exception("Role not found")
            };
            await guildUser.AddRoleAsync(newRole);

            await component.RespondAsync($"Le rang {newRole.Name} a été assigné à {guildUser.Mention}");
            await component.Channel.DeleteMessageAsync(component.Message.Id);
        }

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