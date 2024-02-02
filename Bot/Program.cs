namespace Bot;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;

public class Program
{
    public static Task Main(string[] Args) => new Program().MainAsync();
    private DiscordSocketClient _Client;
    private SlashCommandCreation _CommandCreation;
    private SlashCommandHandle _CommandHandle;
    
    public async Task MainAsync()
    {
        _Client = new DiscordSocketClient();
        _CommandCreation = new SlashCommandCreation(_Client);
        _CommandHandle = new _SlashCommandHandle();
        
        _Client.Log += Log;
        _Client.Ready += () => Task.FromResult(_CommandCreation.CreateCommands());
        _Client.SlashCommandExecuted += Task.FromResult(_CommandHandle.SlashCommandExecuted);
        
        var Token = Environment.GetEnvironmentVariable("BotToken");

        await _Client.LoginAsync(TokenType.Bot, Token);
        await _Client.StartAsync();

        await Task.Delay(-1);
    }
    
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
    
    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        command.RespondAsync($"Pong!");
    }
}