using Bot.HostingServices;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using IHost BotHost = Host.CreateDefaultBuilder(args)
    .ConfigureServices(Services =>
    {
        Services.AddSingleton<DiscordSocketClient>();       // Add the discord client to services
        Services.AddSingleton<InteractionService>();        // Add the interaction service to services
        Services.AddHostedService<InteractionHandlingService>();    // Add the slash command handler
        Services.AddHostedService<DiscordStartupService>();         // Add the discord startup service
    })
    .Build();

await BotHost.RunAsync();