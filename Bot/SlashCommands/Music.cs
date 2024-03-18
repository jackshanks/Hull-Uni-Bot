﻿

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
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm already connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }
            
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

        [SlashCommand("play", "Play your music!", runMode: Discord.Interactions.RunMode.Async)]
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
                var searchResponse = await _lavaNode.SearchAsync(default, query);
                if (searchResponse.Status == SearchStatus.LoadFailed ||
                    searchResponse.Status == SearchStatus.NoMatches) {
                    await ReplyAsync($"I wasn't able to find anything for `{query}`.");
                    return;
                }

                var yes = _lavaNode.TryGetPlayer(Context.Guild, out var player);

                if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused) 
                {
                    var track = searchResponse.Tracks.First();
                    player.Vueue.Enqueue(track);
                    await ReplyAsync($"Enqueued: {track.Title}");
                }
                else {
                    var track = searchResponse.Tracks.First();
                    await player.PlayAsync(track);
                    await ReplyAsync($"Now Playing: {track.Title}");
                }
            }
        }
    }
}

