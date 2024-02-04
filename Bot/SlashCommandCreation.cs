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
    public async Task CreateCommands()
    {
        var Guild = _Client.GetGuild(1153315295306465381);
        await RoleCommand(Guild);
        await PingCommand(Guild);
    }

// ~~ ALL SLASH COMMANDS CREATION ~~

    private async Task PingCommand(IGuild Guild)
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
	
    private async Task RoleCommand(IGuild Guild)
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