using Bot;
using Discord.Net;
using Newtonsoft.Json;

namespace DefaultNamespace;

using Discord;
using Discord.WebSocket;
using Discord.Commands;


public class Program
{
    public static Task Main(string[] Args) => new Program().MainAsync();
    
    private DiscordSocketClient _Client;
	private SlashCommands _SlashCommands;

    public async Task MainAsync()
    {
        _Client = new DiscordSocketClient();

        _Client.Log += Log;
        
        var Token = Environment.GetEnvironmentVariable("BotToken");;

        await _Client.LoginAsync(TokenType.Bot, Token);
        await Client_Ready();
        await _Client.StartAsync();
        _Client.SlashCommandExecuted += _SlashCommands.SlashCommandExecuted;

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
    
    private Task Log(LogMessage Msg)
    {
        Console.WriteLine(Msg.ToString());
        return Task.CompletedTask;
    }
    
    public async Task Client_Ready()
    {
        // Let's do our global command
        var GlobalCommand = new SlashCommandBuilder();
        GlobalCommand.WithName("Ping");
        GlobalCommand.WithDescription("Lets play table tennis.");

        try
        {
            await _Client.CreateGlobalApplicationCommandAsync(GlobalCommand.Build());
        }
        catch(ApplicationCommandException Exception)
        {
            var json = JsonConvert.SerializeObject(Exception.Errors, Formatting.Indented);
            Console.WriteLine(json);
        }
    }
}