using Bot.HostingServices;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Audio;
using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Extensions;
using Lavalink4NET.Rest;

var config = new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.All
};

using IHost botHost = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        
        services.AddSingleton(new DiscordSocketClient(config));
        services.AddSingleton<InteractionService>();        // Add the interaction service to services
        services.AddHostedService<InteractionHandlingService>();    // Add the slash command handler
        services.AddHostedService<DiscordStartupService>();         // Add the discord startup service
        services.AddSingleton<IAudioService>();
        services.AddLavalinkCore();

    })
    .Build();

await botHost.RunAsync();