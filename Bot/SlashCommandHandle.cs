using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
namespace Bot;

public class SlashCommandHandle
{
    private readonly SocketGuild _Guild;
    
    public SlashCommandHandle(SocketGuild GetGuild)
    {
        _Guild = GetGuild;
    }

    public Task SlashCommandExecuted(SocketSlashCommand Interaction)
    {
        return Interaction.Data.Name switch
        {
            "ping" => PingCommand(Interaction),
            "role" => RoleCommand(Interaction),
            _ => Task.CompletedTask
        };
    }

// ~~ ALL SLASH COMMAND EXECUTION ~~

    private Task PingCommand(ISlashCommandInteraction Interaction)
    {
        return Interaction.RespondAsync("Pong!");
    }

    private Task RoleCommand(ISlashCommandInteraction Interaction)
    {
        var RoleName = Interaction.Data.Options.First().Value;
        var HexCode = Interaction.Data.Options.ElementAt(1).Value;
        
        return Interaction.RespondAsync($"**Role Name:** {RoleName} \n**Hex Code:** {HexCode}\n {Interaction.User}");
        
        //var role = context._Guild.CrateRoleAsync($"{Interaction.User}");
        //await user.AddRoleAsync(role);
    }
}