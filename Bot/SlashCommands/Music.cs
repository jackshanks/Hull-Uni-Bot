

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
            else
            {
                await ReplyAsync("You are not connected to a voice channel.");
            }
        }

        [SlashCommand("play", "Play your music!", runMode: Discord.Interactions.RunMode.Async)]
        public async Task Play()
        {
            var user = Context.User as IGuildUser;
            var audioUrl = "https://codeskulptor-demos.commondatastorage.googleapis.com/GalaxyInvaders/theme_01.mp3";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(audioUrl))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (var ms = new MemoryStream(await response.Content.ReadAsByteArrayAsync()))
                            {
                                // Initialize PortAudio
                                int deviceId = Pa_GetDefaultOutputDevice(); // Get default output device
                                int sampleRate = 48000; // Adjust as needed (common rate)
                                int channels = 2; // Adjust for stereo/mono
                                int framesPerBuffer = 1024; // Adjust buffer size based on your system

                                var stream = Pa_OpenStream(deviceId, channels, PaStreamFlags.paNoInput,
                                    sampleRate, framesPerBuffer, null, null);

                                if (stream == IntPtr.Zero)
                                {
                                    Console.WriteLine("Failed to open PortAudio stream!");
                                    await ReplyAsync("An error occurred while initializing audio playback.");
                                    return;
                                }

                                // Play audio data in a loop

                                int bytesRead;
                                byte[] buffer =
                                    new byte[framesPerBuffer * channels * (sizeof(short))]; // Assuming 16-bit PCM

                                while ((bytesRead = await ms.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    int result = Pa_StreamWrite(stream, buffer, bytesRead);
                                    if (result != 0)
                                    {
                                        Console.WriteLine($"PortAudio error: {Pa_GetErrorText(result)}");
                                        break;
                                    }
                                }

                                // Clean up
                                Pa_CloseStream(stream);
                                await ReplyAsync("Finished playing audio.");
                            }
                        }
                        else
                        {
                            await ReplyAsync($"Failed to download audio: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing audio: {ex.Message}");
                await ReplyAsync($"An error occurred while playing audio. {ex.Message}");
            }
        }
    }
}
