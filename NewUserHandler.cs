// using Discord.WebSocket;
// using Discord.Commands;
// public class NewUserHandler
// {
//     private readonly DiscordSocketClient _client;
//     private readonly CommandService _commands;

//     // Retrieve client and CommandService instance via ctor
//     public NewUserHandler(DiscordSocketClient client, CommandService commands)
//     {
//         _commands = commands;
//         _client = client;
//     }

//     public async Task InstallNewUserHandler()
//     {
//         // Hook the MessageReceived event into our command handler
//         _client.UserJoined += HandleUsersAsync;
        
//     }
//     public async Task HandleUsersAsync(SocketGuildUser socketGuildUser){
//         socketGuildUser.Guild.GetUser(socketGuildUser.Id)
//         socketGuildUser.AddRoleAsync() = "HediIsBG";
//     }
// }