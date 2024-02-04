using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
namespace Bot;

public class SlashCommandHandle
{
    private readonly DiscordSocketClient _Client;
    
    public SlashCommandHandle(DiscordSocketClient GetClient)
    {
        _Client = GetClient;
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

    private async Task PingCommand(ISlashCommandInteraction Interaction)
    {
        await Interaction.RespondAsync($"Pong! Your response time was {_Client.Latency}");
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
}  