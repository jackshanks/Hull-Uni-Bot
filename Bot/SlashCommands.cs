using Discord;
using Discord.WebSocket;
using Discord.Commands;
namespace Bot;

public class SlashCommands
{

    public async Task SlashCommandExecuted(SocketSlashCommand Interaction)
    {
        if (Interaction.Data.Name == "ping")
        {
            await Interaction.RespondAsync("Pong!");
        }
    }
    
    
    public async Task PingCommand(ISlashCommandInteraction Interaction)
    {
        await Interaction.RespondAsync("Pong!");
    }
}