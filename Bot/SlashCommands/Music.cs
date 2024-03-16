

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
            else { await ReplyAsync("You are not connected to a voice channel."); }
        }

        [SlashCommand("play", "Play your music!")]
        public async Task Play()
        {
            var user = Context.User as IGuildUser;
            var audioUrl = "https://www.youtube.com/watch?v=otwRNo9b4Ao&pp=ygUWcG9ybmh1YiBub2lzZSAyNCBob3Vycw%3D%3D";

            await using (var pcmStream = await GetAudioStreamAsync(audioUrl))
            {
                using (var waveOut = new WaveOutEvent())
                {
                    var audioClient = await user.VoiceChannel.ConnectAsync();
                    await using (var audioOutStream = audioClient.CreatePCMStream(AudioApplication.Music))
                    {
                        await pcmStream.CopyToAsync(audioOutStream);
                    }
                }
            }
        }
        
        
        public static async Task<Stream> GetAudioStreamAsync(string url)
        {
            var tempFilePath = Path.Combine(Directory.GetCurrentDirectory(), "audio.pcm");
            var ffmpegCommand = $"-y -i {url} -vn -ar 48000 -ac 2 -acodec pcm_s16le {tempFilePath}";


            try
            {
                var process = Process.Start("ffmpeg", ffmpegCommand);

                process.WaitForExit();
                
                if (process.ExitCode != 0)
                {
                    throw new Exception($"FFmpeg decoding failed.{Directory.GetCurrentDirectory()}");
                }
                
                using (var fileStream = File.OpenRead(tempFilePath))
                {
                    return fileStream;
                }
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
    }
}