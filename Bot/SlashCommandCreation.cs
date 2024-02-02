namespace Bot;

using Discord.Commands;

public class SlashCommandCreation
{
    private Discord.Guild Guild;

    public SlashCommandCreation(Discord.Client client)
    {
        Guild = _Client.GetGuild(1153315295306465381);
    }

    public async Task CreateCommands()
    {
        await PingCommand(Guild); // Pass Guild as an argument
    }

    private async Task PingCommand(Discord.Guild guild) // Correct scope and access modifier
    {
        var Ping = new SlashCommandBuilder();
        Ping.WithName("ping");
        Ping.WithDescription("Lets play tennis");

        try
        {
            await guild.CreateApplicationCommandAsync(Ping.Build()); // Use passed Guild object
        }
        catch (HttpException httpError)
        {
            Console.WriteLine(httpError.Reason);
        }
    }
}