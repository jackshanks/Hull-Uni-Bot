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

        public async Task StartAsync(CancellationToken CancellationToken)
        {
            _Discord.Ready += () => _Interactions.RegisterCommandsToGuildAsync(1153315295306465381);
            _Discord.InteractionCreated += OnInteractionAsync;
            _Discord.UserJoined += OnUserJoined; 

            await _Interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _Services);
        }

        public Task StopAsync(CancellationToken CancellationToken)
        {
            _Interactions.Dispose();
            return Task.CompletedTask;
        }

        private async Task OnInteractionAsync(SocketInteraction Interaction)
        {
            try
            {
                var Context = new SocketInteractionContext(_Discord, Interaction);
                var Result = await _Interactions.ExecuteCommandAsync(Context, _Services);

                if (!Result.IsSuccess)
                    await Context.Channel.SendMessageAsync(Result.ToString());
            }
            catch
            {
                if (Interaction.Type == InteractionType.ApplicationCommand)
                {
                    await Interaction.GetOriginalResponseAsync()
                        .ContinueWith(Msg => Msg.Result.DeleteAsync());
                }
            }
        }
        
        private async Task OnUserJoined(SocketGuildUser user)
        {
            var Unverified = user.Guild.Roles.FirstOrDefault(x => x.Name == "Unverified");
            if (Unverified != null)
            {
                await user.AddRolesAsync(Unverified);
            }
        }
    }
}