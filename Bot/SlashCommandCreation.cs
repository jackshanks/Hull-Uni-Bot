namespace Bot;

public class SlashCommandCreation
{
    private Discord.Guild Guild = _Client.GetGuild(1153315295306465381);
    
    public async Task CreateCommands()
    {
        PingCommand();
    } 

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