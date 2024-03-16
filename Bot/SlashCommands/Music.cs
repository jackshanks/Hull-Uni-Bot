

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
using System.Net.Http.Headers;
using Discord.Net;
using YoutubeDLSharp.Options;
using Microsoft.Extensions.Logging;
using YoutubeDLSharp;
using RunMode = Discord.Commands.RunMode;

namespace Bot.SlashCommands
{
    public class Music : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DiscordSocketClient _discord;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;
        private YoutubeDL ydl;
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
            ydl = new YoutubeDL();
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

            var mp3FilePath = $"{Directory.GetCurrentDirectory()}/mp3.mp3";

            ydl.OverwriteFiles = true;
            ydl.OutputFileTemplate = "mp3.mp3";
            ydl.YoutubeDLPath = "/usr/bin/yt-dlp";
            ydl.OutputFolder = "Hull-Uni-Bot/Bot";
            var result = await ydl.RunAudioDownload("https://www.youtube.com/watch?v=0iN-hemcKc8", AudioConversionFormat.Mp3);
            
            var pcmFilePath = $"{Directory.GetCurrentDirectory()}/pcm.pcm";
            
            //if (File.Exists(pcmFilePath))
            //{
            //    File.Delete(pcmFilePath);
            //}

            if (user?.VoiceChannel != null)
            {
                try
                {
                    await ConvertMp3ToPcm(mp3FilePath, pcmFilePath);

                    using var audioClient = await user.VoiceChannel.ConnectAsync();
                    var audioOutStream = audioClient.CreatePCMStream(AudioApplication.Mixed);

                    using (var pcmFileStream = File.OpenRead(pcmFilePath))
                    {
                        await pcmFileStream.CopyToAsync(audioOutStream);
                        await audioOutStream.FlushAsync();
                    }

                    await ReplyAsync("Finished playing audio.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error playing audio: {ex.Message}");
                    await ReplyAsync($"An error occurred while playing audio. {ex.Message}");
                }
            }
            else
            {
                await ReplyAsync("You are not connected to a voice channel.");
            }
        }

        private async Task ConvertMp3ToPcm(string mp3FilePath, string pcmFilePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i \"{mp3FilePath}\" -f s16le -acodec pcm_s16le -ar 44100 \"{pcmFilePath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();
        }

        public static class ProcessExtensions
        {
            public static Task WaitForExitAsync(Process process)
            {
                var tcs = new TaskCompletionSource<object>();
                process.EnableRaisingEvents = true;
                process.Exited += (sender, args) => tcs.SetResult(null);
                return tcs.Task;
            }
        }
    }
}

