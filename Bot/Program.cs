﻿using Bot.HostingServices;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Victoria;
using Victoria.Node;

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
        services.AddLavaNode(x => {
            x.SelfDeaf = false;
            x.Hostname = "lavalink.chompubot.work";
            x.Port = 30216;
            x.Authorization = "somboytiger";
        });
        
    })
    .Build();

await botHost.RunAsync();