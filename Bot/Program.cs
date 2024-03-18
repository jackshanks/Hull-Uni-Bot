using System.Collections.Immutable;
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
var lavaconfig = new NodeConfiguration
{
    Port = 2332,
    Authorization = "youshallnotpass",
    Hostname = "0.0.0.0"
};

var builder = new HostApplicationBuilder(args);

builder.Services.AddSingleton(new DiscordSocketClient(config));
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddHostedService<DiscordStartupService>();
builder.Services.AddHostedService<InteractionHandlingService>();
builder.Services.AddSingleton<LavaNode>();
builder.Services.AddSingleton(lavaconfig);


var app = builder.Build();
app.Run();