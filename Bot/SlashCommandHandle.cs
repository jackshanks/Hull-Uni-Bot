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
    }

// ~~ ALL SLASH COMMAND EXECUTION ~~

    public Task PingCommand(ISlashCommandInteraction Interaction)
    {
        return Interaction.RespondAsync("Pong!");
    }
}