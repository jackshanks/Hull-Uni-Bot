

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
using Victoria;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Victoria.Node;
using Victoria.Player;
using Victoria.Responses.Search;
using RunMode = Discord.Commands.RunMode;

namespace Bot.SlashCommands
{
    public class Music : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DiscordSocketClient _discord;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;
        private readonly AudioService _audioService;
        private readonly LavaNode _lavaNode;
        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        public Music(DiscordSocketClient discord,
            InteractionService interactions,
            IServiceProvider services,
            ILogger<InteractionService> logger,
            LavaNode lavaNode,
            AudioService audioService)
        {
            _discord = discord;
            _interactions = interactions;
            _services = services;
            _interactions.Log += Msg => LogHelper.OnLogAsync(logger, Msg);
            _audioService = audioService;
            _lavaNode = lavaNode;
        }

        [SlashCommand("join", "Join the voice channel", runMode: Discord.Interactions.RunMode.Async)]
        public async Task JoinAsync()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            if (_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await _lavaNode.LeaveAsync(player.VoiceChannel);
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
            }
            else
            {
                try
                {
                    await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                    await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
                }
                catch (Exception exception)
                {
                    await ReplyAsync(exception.Message);
                }
            }
        }
        
        [SlashCommand("leave","Leave the voice channel")]
        public async Task LeaveAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                await ReplyAsync("I'm not connected to any voice channels!");
                return;
            }

            var voiceChannel = (Context.User as IVoiceState).VoiceChannel ?? player.VoiceChannel;
            if (voiceChannel == null) {
                await ReplyAsync("Not sure which voice channel to disconnect from.");
                return;
            }

            try
            {
                await _lavaNode.LeaveAsync(voiceChannel);
                await ReplyAsync($"I've left {voiceChannel.Name}!");
            }
            catch (Exception exception) {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("play", "Play your music!", runMode: Discord.Interactions.RunMode.Async)]
        public async Task PlayAsync([Remainder] string searchQuery) {
            if (string.IsNullOrWhiteSpace(searchQuery)) {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await JoinAsync();
            }
            
            var searchResponse = await _lavaNode.SearchAsync(default, searchQuery);
            if (searchResponse.Status == SearchStatus.LoadFailed ||
                searchResponse.Status == SearchStatus.NoMatches) 
            { 
                await ReplyAsync($"I wasn't able to find anything for `{searchQuery}`.");
                return;
            }

            var yes = _lavaNode.TryGetPlayer(Context.Guild, out var player);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused) {
                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name)) {
                    foreach (var track in searchResponse.Tracks) {
                        player.Vueue.Enqueue(track);
                    }

                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else {
                    var track = searchResponse.Tracks.First();
                    player.Vueue.Enqueue(track);
                    await ReplyAsync($"Enqueued: {track.Title}");
                }
            }
            else {
                var track = searchResponse.Tracks.First();

                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name)) {
                    for (var i = 0; i < searchResponse.Tracks.Count; i++) {
                        if (i == 0) {
                            await player.PlayAsync(track);
                            await ReplyAsync($"Now Playing: {track.Title}");
                        }
                        else {
                            player.Vueue.Enqueue(searchResponse.Tracks.ElementAtOrDefault(i));
                        }
                    }

                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else {
                    await player.PlayAsync(track);

                    var embed = new EmbedBuilder { }
                        .WithTitle(track.Title)
                        .WithDescription($"As requested by {Context.User.Mention}")
                        .WithFooter($"Length: {track.Duration.ToString()}");
                        

                    
                    await ReplyAsync(embed: embed.Build());
                }
            }
        }
    }
}

