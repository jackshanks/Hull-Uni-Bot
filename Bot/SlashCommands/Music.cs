

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
using Lavalink4NET;
using Victoria;
using Victoria.Node;
using Victoria.Player;
using Victoria.Responses.Search;
using PlayerState = Victoria.Player.PlayerState;
using RunMode = Discord.Commands.RunMode;

namespace Bot.SlashCommands
{
    public class Music : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DiscordSocketClient _discord;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;
        private readonly LavaNode _lavaNode;
        private readonly AudioService _audioService;
        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);
        
        public Music (
            DiscordSocketClient discord,
            InteractionService interactions,
            IServiceProvider services,
            ILogger<InteractionService> logger,
            LavaNode lavaNode)
        {
            _discord = discord;
            _interactions = interactions;
            _services = services;
            _lavaNode = lavaNode;
            _interactions.Log += Msg => LogHelper.OnLogAsync(logger, Msg);
        }
        
        
        [SlashCommand("join", "Join the channel", runMode: Discord.Interactions.RunMode.Async)]
        public async Task JoinAsync() {
            if (_lavaNode.HasPlayer(Context.Guild)) {
                await ReplyAsync("I'm already connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null) {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            try {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception) {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("leave", "Leave a voice channel.")]
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

            try {
                await _lavaNode.LeaveAsync(voiceChannel);
                await ReplyAsync($"I've left {voiceChannel.Name}!");
            }
            catch (Exception exception) {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("play","Play a track.")]
        public async Task PlayAsync([Remainder] string searchQuery) {
            if (string.IsNullOrWhiteSpace(searchQuery)) {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild)) {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            var queries = searchQuery.Split(' ');
            foreach (var query in queries) {
                var searchResponse = await _lavaNode.SearchAsync(SearchType.Direct,query);
                if (searchResponse.Status == SearchStatus.LoadFailed ||
                    searchResponse.Status == SearchStatus.NoMatches) {
                    await ReplyAsync($"I wasn't able to find anything for `{query}`.");
                    return;
                }

                var player = _lavaNode.TryGetPlayer(Context.Guild, out LavaPlayer<LavaTrack> _lavaPlayer);

                if (_lavaPlayer.PlayerState is PlayerState.Playing or PlayerState.Paused) {
                    if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name)) {
                        foreach (var track in searchResponse.Tracks) {
                            _lavaPlayer.Vueue.Enqueue(track);
                        }

                        await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                    }
                    else {
                        var track = searchResponse.Tracks.ElementAt(0);
                        _lavaPlayer.Vueue.Enqueue(track);
                        await ReplyAsync($"Enqueued: {_lavaPlayer.Track.Title}");
                    }
                }
                else {
                    var track = (searchResponse.Tracks).ElementAt(0);

                    if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name)) {
                        for (var i = 0; i < searchResponse.Tracks.Count; i++) {
                                await _lavaPlayer.PlayAsync(track);
                            if (i == 0) {
                                await ReplyAsync($"Now Playing: {track.Title}");
                            }
                            else {
                                _lavaPlayer.Vueue.Enqueue(searchResponse.Tracks.ElementAt(i));
                            }
                        }

                        await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                    }
                    else {
                        await _lavaPlayer.PlayAsync(track);
                        await ReplyAsync($"Now Playing: {track.Title}");
                    }
                }
            }
        }

        [SlashCommand("pause", "Pause the player")]
        public async Task PauseAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing) {
                await ReplyAsync("I cannot pause when I'm not playing anything!");
                return;
            }

            try {
                await player.PauseAsync();
                await ReplyAsync($"Paused: {player.Track.Title}");
            }
            catch (Exception exception) {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("resume", "Restart the player")]
        public async Task ResumeAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Paused) {
                await ReplyAsync("I cannot resume when I'm not playing anything!");
                return;
            }

            try {
                await player.ResumeAsync();
                await ReplyAsync($"Resumed: {player.Track.Title}");
            }
            catch (Exception exception) {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("stop", "Force stop the player")]
        public async Task StopAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState == PlayerState.Stopped) {
                await ReplyAsync("Woah there, I can't stop the stopped forced.");
                return;
            }

            try {
                await player.StopAsync();
                await ReplyAsync("No longer playing anything.");
            }
            catch (Exception exception) {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("skip","Skip the track.")]
        public async Task SkipAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing) {
                await ReplyAsync("Woah there, I can't skip when nothing is playing.");
                return;
            }

            try {
                var oldTrack = player.Track;
                (LavaTrack Skipped, LavaTrack Current) currenTrack = await player.SkipAsync();
                await ReplyAsync($"Skipped: {oldTrack.Title}\nNow Playing: {currenTrack.Item2.Title}");
            }
            catch (Exception exception) {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("seek", "?")]
        public async Task SeekAsync(TimeSpan timeSpan) {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing) {
                await ReplyAsync("Woaaah there, I can't seek when nothing is playing.");
                return;
            }

            try {
                await player.SeekAsync(timeSpan);
                await ReplyAsync($"I've seeked `{player.Track.Title}` to {timeSpan}.");
            }
            catch (Exception exception) {
                await ReplyAsync(exception.Message);
            }
        }

        [SlashCommand("nowplaying", "Find the current song that is playing.")]
        public async Task NowPlayingAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing) {
                await ReplyAsync("Woah there, I'm not playing any tracks.");
                return;
            }

            var track = player.Track;
            var artwork = await track.FetchArtworkAsync();

            var embed = new EmbedBuilder {
                    Title = $"{track.Author} - {track.Title}",
                    ThumbnailUrl = artwork,
                    Url = track.Url
                }
                .AddField("Id", track.Id)
                .AddField("Duration", track.Duration)
                .AddField("Position", track.Position);

            await ReplyAsync(embed: embed.Build());
        }
    }
}