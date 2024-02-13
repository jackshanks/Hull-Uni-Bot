using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Bot.LogHandle;

namespace Bot.HostingServices
{
    public class DiscordStartupService : IHostedService
    {
        private readonly DiscordSocketClient _Discord;

        public DiscordStartupService(DiscordSocketClient Discord, ILogger<DiscordSocketClient> Logger)
        {
            _Discord = Discord;
            _Discord.Log += Msg => LogHelper.OnLogAsync(Logger, Msg);
        }

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            await _Discord.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BotToken"));
            await _Discord.StartAsync();
        }

        public async Task StopAsync(CancellationToken CancellationToken)
        {
            await _Discord.LogoutAsync();
            await _Discord.StopAsync();
        }
    }
}