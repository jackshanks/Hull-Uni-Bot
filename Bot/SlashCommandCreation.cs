﻿namespace Bot;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;

public class SlashCommandCreation
{
    private readonly DiscordSocketClient _Client;

    public SlashCommandCreation(DiscordSocketClient Client)
    {
        _Client = Client;
    }

    //Creates all commands with the relevant methods
    public async Task CreateCommands()
    {
        var Guild = _Client.GetGuild(1153315295306465381);

        await Task.WhenAll(
            RoleCommand(Guild), PingCommand(Guild), JoinChannel(Guild), SimonFact(Guild)
        );
    }

// ~~ ALL SLASH COMMANDS CREATION ~~

    private async Task PingCommand(IGuild Guild)
    {
        var Ping = new SlashCommandBuilder()
            .WithName("ping")
            .WithDescription("Lets play tennis");

        try
        {
            await Guild.CreateApplicationCommandAsync(Ping.Build()); // Use passed Guild object
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message} {ex.StackTrace}");
        }
    }
	
    private async Task RoleCommand(IGuild Guild)
    {
        var Role = new SlashCommandBuilder()
            .WithName("role")
            .WithDescription("Lets you change your role")
            .AddOption("name", ApplicationCommandOptionType.String, "Choose the name of the role", isRequired: true)
            .AddOption("hex", ApplicationCommandOptionType.String, "Choose the colour of the role", isRequired: true);

        try
        {
            await Guild.CreateApplicationCommandAsync(Role.Build()); // Use passed Guild object
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message} {ex.StackTrace}");
        }
    }
    
    private async Task JoinChannel(IGuild Guild)
    {
        var Role = new SlashCommandBuilder()
            .WithName("join")
            .WithDescription("Joins the voice channel");
        
        try
        {
            await Guild.CreateApplicationCommandAsync(Role.Build()); // Use passed Guild object
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message} {ex.StackTrace}");
        }
    }
    
    private async Task SimonFact(IGuild Guild)
    {
        var SimonFact = new SlashCommandBuilder()
            .WithName("simon-fact")
            .WithDescription("Get a random fact about simon!");

        try
        {
            await Guild.CreateApplicationCommandAsync(SimonFact.Build()); // Use passed Guild object
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message} {ex.StackTrace}");
        }
    }
    
}