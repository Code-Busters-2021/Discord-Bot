using Discord;
using Discord.WebSocket;
using DiscordBot.Core;

namespace DiscordBot.Journeys.UserJoined;

public class NewUserAssignmentJourney : UserJoinedJourneyBase
{
    public NewUserAssignmentJourney(DiscordSocketClient client, GuildData guildData,
        TriggerMapper.TriggerMapper mapper)
        : base(client, guildData, mapper)
    {
        client.UserJoined += UserJoined;
    }

    public async Task UserJoined(SocketGuildUser user)
    {
        await user.SendMessageAsync("WElcome! GG");

        //    user.ModifyAsync(user => user.Nickname = "Hedi Sellami");
    }
}