

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
        private readonly IAudioClient _audioClient;
        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        public Music(
            DiscordSocketClient discord,
            InteractionService interactions,
            IServiceProvider services,
            ILogger<InteractionService> logger,
            IAudioClient audioClient)
        {
            _discord = discord;
            _audioClient = _discord.GetGuild(1153315295306465381).AudioClient;
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
                        var audioStream = await response.Content.ReadAsStreamAsync();

                        // Create a PCM stream from the downloaded audio
                        var pcmStream = _audioClient.CreatePCMStream(AudioApplication.Music);

                        // Start sending audio data asynchronously
                        Task musicTask = Task.Run(async () =>
                        {
                            try
                            {
                                await audioStream.CopyToAsync(pcmStream);
                            }
                            finally
                            {
                                // Cleanup after playback is complete
                                pcmStream.Dispose();
                                await _audioClient.StopAsync(); // Stop audio output if necessary
                            }
                        });

                        await RespondAsync($"Now playing: {audioUrl}");
                        await musicTask; // Wait for the music playback to finish
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