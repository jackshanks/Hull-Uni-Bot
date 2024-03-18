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

var builder = new HostApplicationBuilder(args);

builder.Services.AddSingleton(new DiscordSocketClient(config));
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddHostedService<DiscordStartupService>();
builder.Services.AddHostedService<InteractionHandlingService>();
builder.Services.AddLavalinkCore();

var app = builder.Build();

builder.Build().Run();