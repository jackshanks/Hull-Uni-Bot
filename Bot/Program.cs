using Discord;
using Discord.Net;
using Discord.WebSocket;

public class Program
{
    public static Task Main(string[] Args) => new Program().MainAsync();
    private DiscordSocketClient _Client;
    
    public async Task MainAsync()
    {
        _Client = new DiscordSocketClient();
        
        var Token = Environment.GetEnvironmentVariable("BotToken");;

        await _Client.LoginAsync(TokenType.Bot, Token);
        await _Client.StartAsync();

        await Task.Delay(-1);
    }
    
}