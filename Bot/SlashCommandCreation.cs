namespace Bot;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;

public class SlashCommandCreation
{
    private readonly IGuild _Guild;

    public SlashCommandCreation(DiscordSocketClient Client)
    {
        _Guild = Client.GetGuild(1153315295306465381);
    }

    public Task CreateCommands()
    {
        return PingCommand(_Guild); // Pass Guild as an argument
    }

    private async Task PingCommand(IGuild Guild) // Correct scope and access modifier
    {
        var Ping = new SlashCommandBuilder();
        Ping.WithName("ping");
        Ping.WithDescription("Lets play tennis");

        try
        {
            await Guild.CreateApplicationCommandAsync(Ping.Build()); // Use passed Guild object
        }
        catch (HttpException HttpError)
        {
            Console.WriteLine(HttpError.Reason);
        }
    }
}