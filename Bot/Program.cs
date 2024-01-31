


using Discord;
using Discord.WebSocket;

public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();
    
    private DiscordSocketClient _client;

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();

        _client.Log += Log;

        //  You can assign your bot token to a string, and pass that in to connect.
        //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
        var token = "MTIwMTg2NjkwNzMyMjY3OTMyNg.Gkp4H1.Cl4lxDuAtd7sBCA3LXLvf-ljNHCM2IwDh_JDMM";

        // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
        // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
        // var token = File.ReadAllText("token.txt");
        // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    
    private Task Log(LogMessage Msg)
    {
        Console.WriteLine(Msg.ToString());
        return Task.CompletedTask;
    }
}