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
            _Discord.SelectMenuExecuted += MyMenuHandler;
            
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
        
        private async Task MyMenuHandler(SocketMessageComponent interaction)
        {
            var user = interaction.User;
            try
            {
                string selectedColour = interaction.Data.ToString()!;

                switch (selectedColour)
                {
                    case "red":
                        // Add the red role to the user
                        await ((user as IGuildUser)!).AddRoleAsync(1217889821905649746);
                        await interaction.RespondAsync("You have selected the red role!",ephemeral:true);
                        break;
                    case "yellow":
                        // Add the yellow role to the user
                        await ((user as IGuildUser)!).AddRoleAsync(1217897797903188069);
                        await interaction.RespondAsync("You have selected the yellow role!",ephemeral:true);
                        break;
                    default:
                        await interaction.RespondAsync("Invalid selection. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Log any errors
                Console.WriteLine(ex.Message);
                await interaction.RespondAsync("An error occurred while processing your request. Please try again later.");
            }
        }
    }
}