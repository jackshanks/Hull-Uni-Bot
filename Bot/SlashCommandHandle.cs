using Discord;
using Discord.Net;
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

    private async Task RoleCommand(ISlashCommandInteraction Interaction)
    {
        SocketGuildUser User = (SocketGuildUser)Interaction.User;
        
        string RoleName = (string)Interaction.Data.Options.First().Value;
        uint HexCode = (uint)Interaction.Data.Options.ElementAt(1).Value;
        
        await Interaction.RespondAsync($"**Role Name:** {RoleName} \n**Hex Code:** {HexCode}");

        var Color = new Color(HexCode);

        var Role = await _Guild.CreateRoleAsync(RoleName, null, Color);

        await User.AddRoleAsync(Role);
    }
}