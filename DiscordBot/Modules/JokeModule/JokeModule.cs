using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Core;
using DiscordBot.Modules.ModuleBase;

namespace DiscordBot.Modules.JokeModule;

public class JokeModule : OverlayInteractionModuleBase<SocketInteractionContext>
{
    private readonly HttpClient _jokeClient = new()
    {
        BaseAddress = new Uri("https://backend-omega-seven.vercel.app/api/")
    };

    public JokeModule(GuildData guildData) : base(guildData)
    {
    }

    [SlashCommand("joke", "Poulpy will tell you a joke")]
    public async Task GetJokeAsync()
    {
        await DeferAsync();

        Joke joke;
        try
        {
            var response = await _jokeClient.GetAsync("getjoke");
            var jokes = await JsonSerializer.DeserializeAsync<List<Joke>>(await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            //the api always returns a list of jokes with one element
            joke = jokes?.First() ?? throw new Exception("Failed to deserialize the joke");
        }
        catch (Exception e)
        {
            await Program.Log(new LogMessage(LogSeverity.Error, nameof(JokeModule), e.Message, e));
            await RespondAsync("Sorry, I didn't manage do get a joke");
            return;
        }

        await FollowupAsync(joke.Question);

        // Send the punchline after 6 seconds
        new Timer(Callback, null, TimeSpan.FromSeconds(6), Timeout.InfiniteTimeSpan);

        async void Callback(object? _) => await FollowupAsync(joke.Punchline);
    }
}

public class Joke
{
    public Joke(string question, string punchline)
    {
        Question = question;
        Punchline = punchline;
    }

    [JsonPropertyName("question")] public string Question { get; init; }

    public string Punchline { get; init; }
}