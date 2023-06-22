// using Discord.WebSocket;

// public class RankAssignerService
// {
//     private readonly DiscordSocketClient _client;
//     private readonly GuildData _guildData;

//     public RankAssignerService(DiscordSocketClient client, GuildData guildData)
//     {
//         _client = client;
//         _guildData = guildData;
//     }

//     public async Task HandleButtonAsync(SocketMessageComponent component)
//     {
//         var componentId = component.Data.CustomId;
//         if (componentId == null) return;
//         if (componentId.StartsWith(CommandsModule.SetRank))
//         {
//             if (!component.GuildId.HasValue)
//             {
//                 await component.Channel.SendMessageAsync("You can only do this in a channel");
//                 return;
//             }
//             var parameters = componentId.Split('-');
//             var guildUser = _client.GetGuild(component.GuildId.Value)
//                         .GetUser(ulong.Parse(parameters[1]));

//             var newRole = parameters[2] switch
//             {
//                 "bronze" => _guildData.BronzeRole,
//                 "silver" => _guildData.SilverRole,
//                 "gold" => _guildData.GoldRole,
//                 "gem" => _guildData.DiamondRole,
//                 _ => throw new Exception("Role not found")
//             };
//             await guildUser.AddRoleAsync(newRole);

//             await component.RespondAsync($"Le rang {newRole.Name} a été assigné à {guildUser.Mention}");
//             await component.Channel.DeleteMessageAsync(component.Message.Id);
//         }

//     }

//     public static KeyValuePair<string, Func<>>()
//     {
//         services.AddScoped<RankAssignerService>();
//         buttonPressedEvent+= 
//     }

// }