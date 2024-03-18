using System.Collections.Immutable;
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
using Lavalink4NET.Clients.Events;
using Lavalink4NET.Events;
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
builder.Services.AddLavalink<YourDiscordClientWrapperImplementation>();
var app = builder.Build();
app.Services.GetRequiredService<IAudioService>();
app.Run();

public class YourDiscordClientWrapperImplementation : IDiscordClientWrapper
{
    public ValueTask<ImmutableArray<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId, bool includeBots = false,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public ValueTask<ClientInformation> WaitForReadyAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public event AsyncEventHandler<VoiceServerUpdatedEventArgs>? VoiceServerUpdated;
    public event AsyncEventHandler<VoiceStateUpdatedEventArgs>? VoiceStateUpdated;
}