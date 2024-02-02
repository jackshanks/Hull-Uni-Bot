using Discord;
using Discord.WebSocket;
using Discord.Commands;
namespace Bot;

public class SlashCommandHandle
{
    public async Task SlashCommandExecuted(SocketSlashCommand Interaction)
    {
        if (Interaction.Data.Name == "ping")
        {
            await PingCommand(Interaction);
        }
        else if (Interaction.Data.Name == "role")
        {
            await RoleCommand(Interaction);
        }
    }

// ~~ ALL SLASH COMMAND EXECUTION ~~

    private Task PingCommand(ISlashCommandInteraction Interaction)
    {
        return Interaction.RespondAsync("Pong!");
    }

    private Task RoleCommand(ISlashCommandInteraction Interaction)
    {
        return Interaction.RespondAsync("DOESNT WORK YET");
    }
}