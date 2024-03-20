

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
using Bot.EmbedMaker;
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
        private readonly EmbedMaker.EmbedMaker _embedMaker;
        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        public Music(DiscordSocketClient discord,
            InteractionService interactions,
            IServiceProvider services,
            ILogger<InteractionService> logger,
            LavaNode lavaNode,
            AudioService audioService, 
            EmbedMaker.EmbedMaker embedMaker)
        {
            _discord = discord;
            _interactions = interactions;
            _services = services;
            _interactions.Log += Msg => LogHelper.OnLogAsync(logger, Msg);
            _audioService = audioService;
            _embedMaker = embedMaker;
            _lavaNode = lavaNode;
        }

        [SlashCommand("join", "Join the voice channel", runMode: Discord.Interactions.RunMode.Async)]
        public async Task JoinAsync()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                var embed = await _embedMaker.ErrorMessage("You must be connected to a voice channel!");
                await RespondAsync(embed: embed.Build());
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
                    
                    var embed = await _embedMaker.JoinLeave(Context.User, true);
                    await RespondAsync(embed: embed.Build());
                }
                catch (Exception exception)
                {
                    var embed = await _embedMaker.Update(exception.Message);
                    await RespondAsync(embed : embed.Build());
                }
            }
        }
        
        [SlashCommand("leave","Leave the voice channel")]
        public async Task LeaveAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                var embed = await _embedMaker.ErrorMessage("I'm not connected to any voice channels!");
                await RespondAsync(embed: embed.Build());
                return;
            }

            var voiceChannel = (Context.User as IVoiceState).VoiceChannel ?? player.VoiceChannel;
            if (voiceChannel == null) {
                var embed = await _embedMaker.ErrorMessage("Not sure which voice channel to disconnect from.");
                await RespondAsync(embed: embed.Build());
                return;
            }

            try
            {
                await _lavaNode.LeaveAsync(voiceChannel);

                var embed = await _embedMaker.JoinLeave(Context.User, false);
                await RespondAsync(embed: embed.Build());
            }
            catch (Exception exception) {
                await RespondAsync(exception.Message);
            }
        }
        
        [SlashCommand("play", "Play your music!", runMode: Discord.Interactions.RunMode.Async)]
        public async Task PlayAsync([Remainder] string searchQuery) {
            if (string.IsNullOrWhiteSpace(searchQuery)) {
                var embed = await _embedMaker.ErrorMessage("Please provide search terms.");
                await RespondAsync(embed: embed.Build());
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                var voiceState = Context.User as IVoiceState;
                if (voiceState?.VoiceChannel == null)
                {
                    var embed = await _embedMaker.ErrorMessage("You must be connected to a voice channel!");
                    await RespondAsync(embed: embed.Build());
                    return;
                }

                if (_lavaNode.TryGetPlayer(Context.Guild, out var playerTest))
                {
                    await _lavaNode.LeaveAsync(playerTest.VoiceChannel);
                    await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                }
                else
                {
                    try
                    {
                        await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                    }
                    catch (Exception exception)
                    {
                        var embed = await _embedMaker.Update(exception.Message);
                        await RespondAsync(embed : embed.Build());
                    }
                }
            }
            
            var searchResponse = await _lavaNode.SearchAsync(SearchType.YouTube, searchQuery);
            if (searchResponse.Status == SearchStatus.LoadFailed ||
                searchResponse.Status == SearchStatus.NoMatches) 
            { 
                var embed = await _embedMaker.ErrorMessage($"I wasn't able to find anything for `{searchQuery}`.");
                await RespondAsync(embed: embed.Build());
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
                    var embed = await _embedMaker.PlayQueue(track, true, Context.User, player.Vueue.Count);
                    await RespondAsync(embed: embed.Build());
                }
            }
            else {
                var track = searchResponse.Tracks.First();

                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name)) {
                    for (var i = 0; i < searchResponse.Tracks.Count; i++) {
                        if (i == 0) {
                            await player.PlayAsync(track);
                            var embed = await _embedMaker.PlayQueue(track, false, Context.User);
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
                    
                    var embed = await _embedMaker.PlayQueue(track, false, Context.User);
                    await RespondAsync(embed: embed.Build());
                }
            }
        }
        
        [SlashCommand("pause", "Pause a song!")]
        public async Task PauseAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                var embed = await _embedMaker.ErrorMessage("I'm not connected to a voice channel.");
                await RespondAsync(embed : embed.Build());
                return;
            }

            if (player.PlayerState != PlayerState.Playing) {
                var embed = await _embedMaker.ErrorMessage("I cannot pause when I'm not playing anything!");
                await RespondAsync(embed : embed.Build());
                return;
            }

            try {
                await player.PauseAsync();
                var embed = await _embedMaker.Update($"Paused: {player.Track.Title}");
                await RespondAsync(embed : embed.Build());
            }
            catch (Exception exception) {
                var embed = await _embedMaker.Update(exception.Message);
                await RespondAsync(embed : embed.Build());
            }
        }
        
        [SlashCommand("resume", "Resume a song!")]
        public async Task ResumeAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                var embed = await _embedMaker.ErrorMessage("I'm not connected to a voice channel.");
                await RespondAsync(embed : embed.Build());
                return;
            }

            if (player.PlayerState != PlayerState.Paused) {
                var embed = await _embedMaker.ErrorMessage("I cannot resume when I'm not playing anything!");
                await RespondAsync(embed : embed.Build());
                return;
            }

            try {
                await player.ResumeAsync();
                var embed = await _embedMaker.Update($"Resumed: {player.Track.Title}");
                await RespondAsync(embed : embed.Build());
            }
            catch (Exception exception) {
                var embed = await _embedMaker.Update(exception.Message);
                await RespondAsync(embed : embed.Build());
            }
        }
        
        [SlashCommand("skip", "Skip a song!")]
        public async Task SkipAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                var embed = await _embedMaker.ErrorMessage("I'm not connected to a voice channel.");
                await RespondAsync(embed : embed.Build());
                return;
            }

            if (player.PlayerState != PlayerState.Playing) {
                var embed = await _embedMaker.ErrorMessage("I cannot skip when I'm not playing anything!");
                await RespondAsync(embed : embed.Build());
                return;
            }
            
            try {
                var tracks = await player.SkipAsync();
                
                var embed = await _embedMaker.Skip(tracks.Item2, tracks.Item1, Context.User);
                await RespondAsync(embed: embed.Build());
            }
            catch (Exception exception) {
                await ReplyAsync(exception.Message);
            }
        }
        
        [SlashCommand("clear", "Clears the queue!")]
        public async Task StopAsync() {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                var embed = await _embedMaker.ErrorMessage("I'm not connected to a voice channel.");
                await RespondAsync(embed : embed.Build());
                return;
            }

            if (player.PlayerState == PlayerState.Stopped) {
                var embed = await _embedMaker.ErrorMessage("I am already stopped.");
                await RespondAsync(embed : embed.Build());
                return;
            }

            try {
                player.Vueue.Clear();
                await player.StopAsync();
            }
            catch (Exception exception) {
                var embed = await _embedMaker.Update(exception.Message);
                await RespondAsync(embed : embed.Build());
            }
        }
        [SlashCommand("volume", "Choose a volume between 1 and 100")]
        public async Task Volume(int volume) {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player)) {
                var embed = await _embedMaker.ErrorMessage("I'm not connected to a voice channel.");
                await RespondAsync(embed : embed.Build());
                return;
            }
            try
            {
                await player.SetVolumeAsync(volume);
                var embed = await _embedMaker.Update($"Changed the volume to {volume}%");
                await RespondAsync(embed : embed.Build());
            }
            catch (Exception exception) {
                var embed = await _embedMaker.Update(exception.Message);
                await RespondAsync(embed : embed.Build());
            }
        }
    }
}

