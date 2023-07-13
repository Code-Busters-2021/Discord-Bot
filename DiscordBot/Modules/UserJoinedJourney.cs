// using Discord.Commands;
// using Discord.WebSocket;
// using DiscordBot.Core;
// using DiscordBot.TriggerMapper;
//
// namespace DiscordBot.Journeys;
//
// public class UserJoinedJourney : JourneyBase<CommandContext>
// {
//     private readonly DiscordSocketClient _client;
//     private readonly GuildData _guildData;
//
//     public UserJoinedJourney(DiscordSocketClient client,
//         GuildData guildData,
//         TriggerMapper.TriggerMapper mapper) : base(mapper)
//     {
//         _client = client;
//         _guildData = guildData;
//     }
//
//     [Trigger("")]
//     public Task UserJoined(SocketGuildUser user)
//     {
//         Console.WriteLine("triggerWorked");
//         return Task.CompletedTask;
//     }
// }