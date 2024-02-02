using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;

namespace Bot;

public class Program
{
    public static Task Main(string[] Args) => new Program().MainAsync();
    private DiscordSocketClient _Client;
    
    public async Task MainAsync()
    {
        _Client = new DiscordSocketClient();
        _Client.Log += Log;
        _Client.MessageReceived += Message;
        
        var Token = Environment.GetEnvironmentVariable("BotToken");;

        await _Client.LoginAsync(TokenType.Bot, Token);
        await _Client.StartAsync();

        await Task.Delay(-1);
    }
    
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
    
    private Task Message(SocketMessage msg)
    {
        if (!msg.Author.IsBot)
        {
            if (!(Discord.Commands.HasStringPrefix(msg,'!', 0) || msg.Author.IsBot))
            {
                msg.Channel.SendMessageAsync("Hello from your bot!");   
            }
        }
        return Task.CompletedTask;
    }
}