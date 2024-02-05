﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
using FFMpegCore;
namespace Bot;

public class SlashCommandHandle
{
    private readonly DiscordSocketClient _Client;
    private Random _Random;
    private string[] _SimonFacts;
    
    public SlashCommandHandle(DiscordSocketClient GetClient)
    {
        _Client = GetClient;
        _Random = new Random();
        _SimonFacts = new[]
        {
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
    }

    public Task SlashCommandExecuted(SocketSlashCommand Interaction)
    {
        return Interaction.Data.Name switch
        {
            "ping" => PingCommand(Interaction),
            "role" => RoleCommand(Interaction),
            "join" => JoinChannel(Interaction),
            "simon-fact" => SimonFact(Interaction),
            _ => Task.CompletedTask
        };
    }

// ~~ ALL SLASH COMMAND EXECUTION ~~

    private async Task PingCommand(ISlashCommandInteraction Interaction)
    {
        await Interaction.RespondAsync($"Pong! Your response time was {_Client.Latency}ms");
    }

    
    
    private async Task RoleCommand(ISlashCommandInteraction Interaction)
    {
        
        try
        {
            //Define Objects
            var Guild = _Client.GetGuild(1153315295306465381);
            SocketGuildUser User = (SocketGuildUser)Interaction.User;

            //Set the role name and hex code
            string RoleName = ("$"+(string)Interaction.Data.Options.ElementAt(0).Value);
            string HexCodeInput = (string)Interaction.Data.Options.ElementAt(1).Value;
            
            // Validate HexCode input
            uint ColorInt = uint.Parse(HexCodeInput, System.Globalization.NumberStyles.HexNumber);
            Discord.Color Color = new Color(ColorInt);
            
            
            // Get the role ID of any role with $
            ulong RoleId = User.Roles.FirstOrDefault(r => r.Name.StartsWith("$"))?.Id ?? 0;

            // Find the role to delete using SocketRole if an ID is found
            if (RoleId != 0)
            {
                SocketRole RoleToDelete = Guild.Roles.FirstOrDefault(r2 => r2.Id == RoleId);
                await RoleToDelete.DeleteAsync();
            }
            
            var Role = await Guild.CreateRoleAsync(RoleName, null, Color, false, false, null);

            ulong RoleId2 = Guild.Roles.FirstOrDefault(r3 => r3.Name.StartsWith("/"))?.Id ?? 0;

            if (RoleId2 != 0)
            {
                SocketRole Role2 = Guild.Roles.FirstOrDefault(r4=> r4.Id == RoleId2);
                await Role.ModifyAsync(p => p.Position = Role2.Position);

            }

            await User.AddRoleAsync(Role);

            await Interaction.RespondAsync($"**Role Name:** {RoleName} \n**Hex Code:** {HexCodeInput}");
        }
        catch (Exception ex)
        {
            await Interaction.RespondAsync($"An error occurred: {ex.Message} {ex.StackTrace}");
        }
    }
    
    private async Task JoinChannel(ISlashCommandInteraction Interaction)
    {
        var VoiceChannel = (Interaction.User as IGuildUser)?.VoiceChannel;
        
        if (VoiceChannel== null) { await Interaction.RespondAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }
        
        var AudioClient = await VoiceChannel.ConnectAsync();
    }
    
    private async Task SimonFact(ISlashCommandInteraction Interaction)
    {
        var RandomInt = _Random.Next(0, _SimonFacts.Length);
        await Interaction.RespondAsync($"Did you know that Simon {_SimonFacts[RandomInt]}");
    }

    
}  