

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Bot.HostingServices;
using Bot.LogHandle;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using RunMode = Discord.Commands.RunMode;

namespace Bot.SlashCommands
{
    public class Music : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DiscordSocketClient _discord;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;
        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        public Music(
            DiscordSocketClient discord,
            InteractionService interactions,
            IServiceProvider services,
            ILogger<InteractionService> logger)
        {
            _discord = discord;
            _interactions = interactions;
            _services = services;
            _interactions.Log += Msg => LogHelper.OnLogAsync(logger, Msg);
        }

        [SlashCommand("Join", "Join your voice channel.")]
        public async Task Join()
        {
            var User = Context.User as IGuildUser;
            if (User?.VoiceChannel != null)
            {
                await User.VoiceChannel.ConnectAsync();
                Console.WriteLine($"Joined voice channel: {User.VoiceChannel.Name}");
            }
            else { await ReplyAsync("You are not connected to a voice channel."); }
        }
    }
}