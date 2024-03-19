using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Victoria.Node;
using Victoria.Node.EventArgs;
using Victoria.Player;

namespace Bot.HostingServices {
    public sealed class AudioService {
        private readonly LavaNode _lavaNode;
        private readonly ILogger _logger;
        public readonly HashSet<ulong> VoteQueue;
        private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _disconnectTokens;

        public AudioService(LavaNode lavaNode, ILoggerFactory loggerFactory) {
            _lavaNode = lavaNode;
            _logger = loggerFactory.CreateLogger<LavaNode>();
            _disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();

            _lavaNode.OnUpdateReceived += OnPlayerUpdated;
            _lavaNode.OnStatsReceived += OnStatsReceived;
            _lavaNode.OnTrackEnd += OnTrackEnded;
            //_lavaNode.OnTrackStart += OnTrackStarted;
            _lavaNode.OnTrackException += OnTrackException;
            _lavaNode.OnTrackStuck += OnTrackStuck;
            _lavaNode.OnWebSocketClosed += OnWebSocketClosed;

            VoteQueue = new HashSet<ulong>();
        }

        private Task OnPlayerUpdated(UpdateEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg) {
            _logger.LogInformation($"Track update received for {arg.Track.Title}: {arg.Position}");
            return Task.CompletedTask;
        }

        private Task OnStatsReceived(StatsEventArg arg) {
            _logger.LogInformation($"Lavalink has been up for {arg.Uptime}.");
            return Task.CompletedTask;
        }

        /*private async Task OnTrackStarted(TrackStartEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg) {
            if (!_disconnectTokens.TryGetValue(arg.Player.VoiceChannel.Id, out var value)) {
                return;
            }

            if (value.IsCancellationRequested) {
                return;
            }

            value.Cancel(true);
            await arg.Player.TextChannel.SendMessageAsync("Auto disconnect has been cancelled!");
        }*/

        private async Task OnTrackEnded(TrackEndEventArg<LavaPlayer<LavaTrack>, LavaTrack> args) {

            var player = args.Player;
            if (!player.Vueue.TryDequeue(out var queueable)) {
                await player.TextChannel.SendMessageAsync("Queue completed! Please add more tracks to rock n' roll!");
                return;
            }
    
            if (!(queueable is LavaTrack track)) {
                await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
                return;
            }

            await args.Player.PlayAsync(track);
            await args.Player.TextChannel.SendMessageAsync(
                $"{args.Track}: {args.Track.Title}\nNow playing: {track.Title}");
        }

        private async Task InitiateDisconnectAsync(LavaPlayer player, TimeSpan timeSpan) {
            if (!_disconnectTokens.TryGetValue(player.VoiceChannel.Id, out var value)) {
                value = new CancellationTokenSource();
                _disconnectTokens.TryAdd(player.VoiceChannel.Id, value);
            }
            else if (value.IsCancellationRequested) {
                _disconnectTokens.TryUpdate(player.VoiceChannel.Id, new CancellationTokenSource(), value);
                value = _disconnectTokens[player.VoiceChannel.Id];
            }

            await player.TextChannel.SendMessageAsync($"Auto disconnect initiated! Disconnecting in {timeSpan}...");
            var isCancelled = SpinWait.SpinUntil(() => value.IsCancellationRequested, timeSpan);
            if (isCancelled) {
                return;
            }

            await _lavaNode.LeaveAsync(player.VoiceChannel);
            await player.TextChannel.SendMessageAsync("Invite me again sometime, sugar.");
        }

        private async Task OnTrackException(TrackExceptionEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg) {
            _logger.LogError($"Track {arg.Track.Title} threw an exception. Please check Lavalink console/logs.");
            arg.Player.Vueue.Enqueue(arg.Track);
            await arg.Player.TextChannel?.SendMessageAsync(
                $"{arg.Track.Title} has been re-added to queue after throwing an exception.");
        }

        private async Task OnTrackStuck(TrackStuckEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg) {
            _logger.LogError(
                $"Track {arg.Track.Title} got stuck for {arg.Threshold}ms. Please check Lavalink console/logs.");
            arg.Player.Vueue.Enqueue(arg.Track);
            await arg.Player.TextChannel?.SendMessageAsync(
                $"{arg.Track.Title} has been re-added to queue after getting stuck.");
        }

        private Task OnWebSocketClosed(WebSocketClosedEventArg arg) {
            _logger.LogCritical($"Discord WebSocket connection closed with following reason: {arg.Reason}");
            return Task.CompletedTask;
        }
    }
}