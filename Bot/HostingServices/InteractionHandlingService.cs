using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bot.LogHandle;

namespace Bot.HostingServices
{
    public class InteractionHandlingService : IHostedService
    {
        private readonly DiscordSocketClient _Discord;
        private readonly InteractionService _Interactions;
        private readonly IServiceProvider _Services;

        public InteractionHandlingService(
            DiscordSocketClient Discord,
            InteractionService Interactions,
            IServiceProvider Services,
            ILogger<InteractionService> Logger)
        {
            _Discord = Discord;
            _Interactions = Interactions;
            _Services = Services;
            
            _Interactions.Log += Msg => LogHelper.OnLogAsync(Logger, Msg);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _Discord.Ready += () => _Interactions.RegisterCommandsToGuildAsync(1153315295306465381);
            _Discord.InteractionCreated += OnInteractionAsync;
            _Discord.UserJoined += HandleUserJoin;

            await _Interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _Services);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _Interactions.Dispose();
            return Task.CompletedTask;
        }

        private async Task OnInteractionAsync(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(_Discord, interaction);
                var result = await _Interactions.ExecuteCommandAsync(context, _Services);

                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ToString());
            }
            catch
            {
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await interaction.GetOriginalResponseAsync()
                        .ContinueWith(Msg => Msg.Result.DeleteAsync());
                }
            }
        }

        private async Task HandleUserJoin(SocketGuildUser user)
        {
            await user.AddRoleAsync(1211131520786636820);
        }
    }
}