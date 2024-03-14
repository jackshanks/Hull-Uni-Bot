﻿using Discord;
using Discord.Interactions;
using System;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Bot.LogHandle;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;


namespace Bot.SlashCommands;

public class FunCommands : InteractionModuleBase<SocketInteractionContext>
{
    
    private readonly DiscordSocketClient _Discord;
    private readonly InteractionService _Interactions;
    private readonly IServiceProvider _Services;
    private readonly Random _Random;
    private readonly string[] _SimonFacts;

    public FunCommands(
        DiscordSocketClient Discord,
        InteractionService Interactions,
        IServiceProvider Services,
        ILogger<InteractionService> Logger)
    {
        _Discord = Discord;
        _Interactions = Interactions;
        _Services = Services;
        _Random = new();
        _SimonFacts = new[] {
            "created the hamburger!", 
            "doesn't need to use the toilet!",
            "was born on top a sacred mountain!",
            "is loved by everyone across the world!",
            "learnt to drive when he was 3!",
            "doesn't have a spacebar; he simply stares at the words until they separate out of sheer respect!",
            "is a skilled amateur chess player who frequently participates in global tournaments!",
            "completed a charity unicycle ride along a significant section of the Great Wall of China!",
            "enjoys experimenting with unique recipes, including a watermelon-infused pizza that became an international hit!",
            "participated in a community hopscotch event, impressively lasting for several hours and gaining global recognition!"
        };
            
        _Interactions.Log += Msg => LogHelper.OnLogAsync(Logger, Msg);
    }
    [SlashCommand("ping", "Play some table tennis")]
    public Task PingCommand()
        => RespondAsync($"Pong! Your response time was {Context.Client.Latency} ms");

    [SlashCommand("simon-fact", "Get a random fact Simon!")]
    public Task SimonFact()
    {
        var RandomInt = _Random.Next(0, _SimonFacts.Length);
        return RespondAsync($"Did you know that Simon {_SimonFacts[RandomInt]}");
    }

    [SlashCommand("game-role", "Add games you are interested in!")]
    public Task GameRole(string roleName)
    {
        SocketRole role;
        var user = Context.User;

        ulong roleId = roleName switch
        {
            "overwatch" => 1216878995916853409,
            _ => 0
        };  

        if (roleId != 0)
        { role = Context.Guild.GetRole(roleId); }
        else
        { return ReplyAsync("Error, role not found."); }
        
        (user as IGuildUser)?.AddRoleAsync(role);
        return ReplyAsync($"You have been granted the {role?.Name} role.");
    }
}