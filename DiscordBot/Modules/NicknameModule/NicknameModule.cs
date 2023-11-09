// using Discord.Interactions;
// using Discord.WebSocket;
// using DiscordBot.Core;
// using DiscordBot.Modules.ModuleBase;
//
// namespace DiscordBot.Modules.NicknameModule;
//
// public class NicknameModule : OverlayInteractionModuleBase<SocketInteractionContext>
// {
//     private const string NicknameId = "nickname";
//
//     public NicknameModule(GuildData guildData) : base(guildData)
//     {
//     }
//
//     [SlashCommand("nickname", "Set a nickname for yourself")]
//     public async Task InputNicknameAsync([Summary("New-Nickname")] string newNickname = "")
//     {
//         if (newNickname == "")
//         {
//             await RespondWithModalAsync<NicknameModal>($"{NicknameId}-{Context.User.Id}");
//             return;
//         }
//
//         var user = Context.User as SocketGuildUser ??
//                    GuildData.Guild.GetUser(Context.User.Id);
//
//         await HandleNickname(user, newNickname);
//     }
//
//     [ModalInteraction($"{NicknameId}-*")]
//     public async Task SetNickname(string userId, NicknameModal modal)
//     {
//         var user = GuildData.Guild.GetUser(ulong.Parse(userId));
//
//         await HandleNickname(user, modal.Name);
//     }
//
//     private async Task HandleNickname(SocketGuildUser user, string newNickname)
//     {
//         // This command doesn't work for now, the bot is missing permissions
//         await RespondAsync("I don't have permission to change your nickname, please do it yourself",
//             ephemeral: true);
//         return;
//
//         await user.ModifyAsync(properties => properties.Nickname = newNickname);
//
//         await RespondAsync($"Votre nom sur le serveur codebusters est à present '{newNickname}'", ephemeral: true);
//     }
// }