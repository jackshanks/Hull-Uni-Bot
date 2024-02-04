namespace Bot;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;

public class Program
{
    //Set "MainAsync" as the program that runs on start
    public static Task Main(string[] Args) => new Program().MainAsync();
    
    //Initialises the "_Client" which is the connection to the discord server (sort of)
    private DiscordSocketClient _Client;
    
    //Initialises Classes i have created
    private SlashCommandCreation _CommandCreation;
    private SlashCommandHandle _CommandHandle;
    private InteractionService _InteractionService;
    private BigBrotherModeration _BigBrotherModeration;
    
    public async Task MainAsync()
    {
        DiscordSocketConfig Config = new DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            MessageCacheSize = 100
        };
        
        
        //Creation of the initialised objects above
        _Client = new DiscordSocketClient(Config);
        _InteractionService = new InteractionService(_Client.Rest);
        _CommandCreation = new SlashCommandCreation(_Client);
        _CommandHandle = new SlashCommandHandle(_Client);
        _BigBrotherModeration = new BigBrotherModeration(_Client);
        
        //If an event/error is detected the "Log" task will run
        _Client.Log += Log;

        //When the client turns on, it runs this to create the commands in "SlashCommandCreation.cs"
        _Client.Ready += async () =>
        {
            await Task.Delay(1000); // Wait for a second
            await _CommandCreation.CreateCommands();
        };
        
        //When a slash command is dectected it will run the corrosponding logic
        _Client.SlashCommandExecuted += _CommandHandle.SlashCommandExecuted;
        _Client.MessageReceived += _BigBrotherModeration.CheckContents;
        
        //Runs the bot
        var Token = Environment.GetEnvironmentVariable("BotToken");

        await _Client.LoginAsync(TokenType.Bot, Token);
        await _Client.StartAsync();

        await Task.Delay(-1);
    }
    
    //Displays any logs to console
    private Task Log(LogMessage Msg)
    {
        Console.WriteLine(Msg.ToString());
        return Task.CompletedTask;
    }
}