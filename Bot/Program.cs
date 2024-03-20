using System.Collections.Immutable;
using Bot.EmbedMaker;
using Bot.HostingServices;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Victoria;
using Discord.Audio;
using Victoria.Node;
using Victoria.WebSocket;

var config = new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.All
};

var lavalinkConfig = new NodeConfiguration()
{
    Hostname = "127.0.0.1", // From your server configuration.
    Port = 80, // From your server configuration
    Authorization = "youshallnotpass",
    
    
};

var builder = new HostApplicationBuilder(args);

builder.Services.AddSingleton(new DiscordSocketClient(config));
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddHostedService<DiscordStartupService>();
builder.Services.AddHostedService<InteractionHandlingService>();
builder.Services.AddSingleton<LavaNode>();
builder.Services.AddSingleton(lavalinkConfig);
builder.Services.AddSingleton<AudioService>();
builder.Services.AddSingleton<EmbedMaker>();

var app = builder.Build();
app.Run();