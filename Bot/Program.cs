using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;

public class Program
{
    public static Task Main(string[] Args) => new Program().MainAsync();
    private DiscordSocketClient _Client;
    
    public async Task MainAsync()
    {
        _Client = new DiscordSocketClient();
        _Client.Log += Log;
        _Client.Ready += Client_Ready;
        _Client.SlashCommandExecuted += SlashCommandHandler;
        
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
        command.RespondAsync($"Ping!");
    }
    
    public async Task Client_Ready()
    {
        
        var guild = client.GetGuild(1153315295306465381);
        
        var Ping = new SlashCommandBuilder();
        Ping.WithName("ping");
        Ping.WithDescription("Lets play tennis");

        try
        {
            await guild.CreateApplicationCommandAsync(guildCommand.Build());

        }
        catch(HttpException httpError)
        { 
            Console.WriteLine(httpError.Reason);
        }
    }
    
}