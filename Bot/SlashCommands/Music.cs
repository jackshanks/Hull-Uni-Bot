

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
using NAudio;
using NAudio.Wave;
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
                        using (var audioFileReader = new WaveFileReader(ms))
                        {
                            // Check if format is compatible (may need manual conversion on Linux)
                            if (audioFileReader.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
                            {
                                var audioClient = await user.VoiceChannel.ConnectAsync();
                                var pcmStream = audioClient.CreatePCMStream(AudioApplication.Music);

                                // You might need to adjust buffer size based on your system
                                var buffer = new byte[audioFileReader.WaveFormat.AverageBytesPerSecond / 10];
                                int bytesRead;

                                while ((bytesRead = await audioFileReader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await pcmStream.WriteAsync(buffer, 0, bytesRead);
                                }

                                await pcmStream.FlushAsync();
                                await audioClient.StopAsync();
                            }
                            else
                            {
                                Console.WriteLine("Unsupported audio format. Consider manual conversion on Linux.");
                                await ReplyAsync("Unsupported audio format. Please try a different file.");
                            }
                        }
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