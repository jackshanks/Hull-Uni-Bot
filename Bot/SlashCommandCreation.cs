using System.Reflection;

namespace Bot;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;

public class SlashCommandCreation
{
    private readonly DiscordSocketClient _Client;

    public SlashCommandCreation(DiscordSocketClient Client)
    {
// Gets the guild information for later use
        _Client = Client;
    }

	//Creates all commands with the relevant methods
    public Task CreateCommands()
    {
        IGuild Guild = _Client.GetGuild(1153315295306465381);

        Type MyType = typeof(SlashCommandCreation);
        foreach (var Method in MyType.GetMethods())
        {
            if (Method.IsStatic)
            {
                MyType.InvokeMember(Method.Name, BindingFlags.InvokeMethod, null, this, new object[] { Guild });
            }
        }

        return Task.CompletedTask;
    }

// ~~ ALL SLASH COMMANDS CREATION ~~

    private static async Task PingCommand(IGuild Guild)
    {
        var Ping = new SlashCommandBuilder()
            .WithName("ping")
            .WithDescription("Lets play tennis");

        try
        {
            await Guild.CreateApplicationCommandAsync(Ping.Build()); // Use passed Guild object
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message} {ex.StackTrace}");
        }
    }
	
	private static async Task RoleCommand(IGuild Guild)
    {
        var Role = new SlashCommandBuilder()
        .WithName("role")
        .WithDescription("Lets you change your role")
        .AddOption("name", ApplicationCommandOptionType.String, "Choose the name of the role", isRequired: true)
        .AddOption("hex", ApplicationCommandOptionType.String, "Choose the colour of the role", isRequired: true);

        try
        {
            await Guild.CreateApplicationCommandAsync(Role.Build()); // Use passed Guild object
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message} {ex.StackTrace}");
        }
    }
}