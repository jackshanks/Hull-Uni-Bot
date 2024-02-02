namespace Bot;

public class SlashCommandCreation
{
    private var Guild = _Client.GetGuild(1153315295306465381);
    
    public async Task CreateCommands()
    {
        PingCommand();
    }

    private async Task PingCommand()
    {
        var Ping = new SlashCommandBuilder();
        Ping.WithName("ping");
        Ping.WithDescription("Lets play tennis");

        try
        {
            await guild.CreateApplicationCommandAsync(Ping.Build());

        }
        catch(HttpException httpError)
        { 
            Console.WriteLine(httpError.Reason);
        }
    }
}