namespace DefaultNamespace;

using Discord.Commands;

public class SlashCommands
{
    async Task PingCommand(ISlashCommandInteraction interaction)
    {
        await interaction.RespondAsync("Pong!");
    }
}