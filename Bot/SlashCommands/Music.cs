

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
                await RespondAsync("You must be connected to a voice channel!");
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
                    
                    var embed = await JoinLeave(true);
                    await RespondAsync(embed: embed.Build());
                }
                catch (Exception exception)
                {
                    await RespondAsync(exception.Message);
                }
            }
        }
        
        [SlashCommand("leave","Leave the voice channel")]
        public async Task LeaveAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                await RespondAsync("I'm not connected to any voice channels!");
                return;
            }

            var voiceChannel = (Context.User as IVoiceState).VoiceChannel ?? player.VoiceChannel;
            if (voiceChannel == null) {
                await RespondAsync("Not sure which voice channel to disconnect from.");
                return;
            }

            try
            {
                await _lavaNode.LeaveAsync(voiceChannel);
                
                var embed = await JoinLeave(false);
                await RespondAsync(embed: embed.Build());
            }
            catch (Exception exception) {
                await RespondAsync(exception.Message);
            }
        }
        
        private Task<EmbedBuilder> JoinLeave(bool join)
        {

            var embed = new EmbedBuilder { }
                .WithTitle(join ? "Connected!" : "Disconnected!")
                .WithDescription(join
                    ? $"Connected to {(Context.User as IVoiceState).VoiceChannel.Mention}."
                    : $"Left {(Context.User as IVoiceState).VoiceChannel.Mention}.")
                .WithColor(join ? Color.Green : Color.Red);
            

            return Task.FromResult(embed);
        }
        [SlashCommand("play", "Play your music!", runMode: Discord.Interactions.RunMode.Async)]
        public async Task PlayAsync([Remainder] string searchQuery) {
            if (string.IsNullOrWhiteSpace(searchQuery)) {
                await RespondAsync("Please provide search terms.");
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
                await RespondAsync($"I wasn't able to find anything for `{searchQuery}`.");
                return;
            }

            var yes = _lavaNode.TryGetPlayer(Context.Guild, out var player);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused) {
                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name)) {
                    foreach (var track in searchResponse.Tracks) {
                        player.Vueue.Enqueue(track);
                    }

                    await RespondAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else {
                    var track = searchResponse.Tracks.First();
                    player.Vueue.Enqueue(track);
                    var embed = await EmbedMaker(track, true, player.Vueue.Count);
                    await RespondAsync(embed: embed.Build());
                }
            }
            else {
                var track = searchResponse.Tracks.First();

                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name)) {
                    for (var i = 0; i < searchResponse.Tracks.Count; i++) {
                        if (i == 0) {
                            await player.PlayAsync(track);
                            var embed = await EmbedMaker(track, false);
                            await RespondAsync(embed: embed.Build());
                        }
                        else {
                            player.Vueue.Enqueue(searchResponse.Tracks.ElementAtOrDefault(i));
                        }
                    }

                    await RespondAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else {
                    await player.PlayAsync(track);


                    var embed = await EmbedMaker(track, false);
                    
                    await RespondAsync(embed: embed.Build());
                }
            }
        }

        private Task<EmbedBuilder> EmbedMaker(LavaTrack track, bool queue, int queuePosition = 0)
        {

            var embed = new EmbedBuilder { }
                .WithAuthor(track.Author)
                .WithTitle(track.Title)
                .WithDescription($"Requested by {Context.User.Mention}")
                .WithFooter(queue
                    ? $"Queue Position: {queuePosition} | Length: {track.Duration}"
                    : $"Length: {track.Duration}")
                .WithColor(queue ? Color.Gold : Color.Teal)
                .WithImageUrl(track.FetchArtworkAsync().Result);
            

            return Task.FromResult(embed);
        }
        
    }
}

