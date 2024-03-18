using Bot.HostingServices;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Clients;
using Lavalink4NET.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var config = new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.All
};
    
var builder = new HostApplicationBuilder(args);
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddHostedService<DiscordStartupService>();
builder.Services.AddHostedService<InteractionHandlingService>(); 
builder.Services.AddSingleton<IAudioService>();

builder.Services.AddLavalink<IDiscordClientWrapper>();
builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));


builder.Build().Run();  