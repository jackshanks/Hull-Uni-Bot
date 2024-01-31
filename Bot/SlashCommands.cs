namespace DefaultNamespace;

using Discord.Commands;

async Task PingCommand(ISlashCommandInteraction interaction)
{
    await interaction.RespondAsync("Pong!");
}