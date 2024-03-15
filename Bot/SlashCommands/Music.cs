

using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Bot.HostingServices;
using Bot.LogHandle;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ContextType = Discord.Commands.ContextType;
using RunMode = Discord.Commands.RunMode;

namespace Bot.SlashCommands
{
    public class Music : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DiscordSocketClient _Discord;
        private readonly InteractionService _Interactions;
        private readonly IServiceProvider _Services;
        
        public Music (
            DiscordSocketClient Discord,
            InteractionService Interactions,
            IServiceProvider Services,
            ILogger<InteractionService> Logger)
        {
            _Discord = Discord;
            _Interactions = Interactions;
            _Services = Services;
            _Interactions.Log += Msg => LogHelper.OnLogAsync(Logger, Msg);
        }
        
        
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await Context.Channel.SendMessageAsync(
                    "User must be in a voice channel, or a voice channel must be passed as an argument.");
                return;
            }

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            var audioClient = await channel.ConnectAsync();
        }
    }
}