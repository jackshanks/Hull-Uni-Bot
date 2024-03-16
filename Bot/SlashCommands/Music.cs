

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Bot.HostingServices;
using Bot.LogHandle;
using Discord.WebSocket;
using Discord.Audio;
using Discord.Net;
using Microsoft.Extensions.Logging;
using Alsa.Net;
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

        [SlashCommand("join", "Join your voice channel.", runMode: Discord.Interactions.RunMode.Async)]
        public async Task Join()
        {
            var user = Context.User as IGuildUser;
            if (user?.VoiceChannel != null)
            {
                await RespondAsync($"Connected to {user.VoiceChannel.Name}");
                await user.VoiceChannel.ConnectAsync();
                Console.WriteLine($"Joined voice channel: {user.VoiceChannel.Name}");
            }
            else { await ReplyAsync("You are not connected to a voice channel."); }
        }
        
        [SlashCommand("play", "Play your music!", runMode: Discord.Interactions.RunMode.Async)]
        public async Task Play()
        {
            var user = Context.User as IGuildUser;
            var audioUrl = "https://codeskulptor-demos.commondatastorage.googleapis.com/GalaxyInvaders/theme_01.mp3";

            // Download audio stream (avoid creating temporary file)
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(audioUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var process = Process.Start("usr/bin/ffmpeg", $"-i - -acodec pcm_s16le -f s16le -");
            process.StandardInput.BaseStream.CopyToAsync(await response.Content.ReadAsStreamAsync()).Wait();
            process.StandardOutput.BaseStream.CopyToAsync(_discord.GetGuild(1153315295306465381)
                .AudioClient.CreatePCMStream(AudioApplication.Music)).Wait();
            await process.WaitForExitAsync();

            await RespondAsync($"Now playing: {audioUrl}");
                    }
                    else
                    {
                        await ReplyAsync($"Failed to download audio: {response.StatusCode}");
                    }
                }
            }
        }
    }
}