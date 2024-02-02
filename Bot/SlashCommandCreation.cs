namespace Bot;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;

public class SlashCommandCreation
{
    private readonly IGuild _Guild;

    public SlashCommandCreation(DiscordSocketClient Client)
    {
// Gets the guild information for later use
        _Guild = Client.GetGuild(1153315295306465381);
    }

	//Creates all commands with the relevant methods
    public async Task CreateCommands()
    {
        await RoleCommand(_Guild);
        await PingCommand(_Guild);
    }

// ~~ ALL SLASH COMMANDS CREATION ~~

    private async Task PingCommand(IGuild Guild)
    {
        var Ping = new SlashCommandBuilder();
        Ping.WithName("ping");
        Ping.WithDescription("Lets play tennis");

        try
        {
            await Guild.CreateApplicationCommandAsync(Ping.Build()); // Use passed Guild object
        }
        catch (HttpException HttpError)
        {
            Console.WriteLine(HttpError.Reason);
        }
    }
	
	private async Task RoleCommand(IGuild Guild)
    {
        var RoleCommand = new SlashCommandBuilder();
        RoleCommand.WithName("role");
        RoleCommand.WithDescription("Lets you create your own role");
        RoleCommand.AddOption("name", ApplicationCommandOptionType.String, "Please name your role", isRequired: true);
        RoleCommand.AddOption("hex", ApplicationCommandOptionType.String, "Please enter the hex code of your role", isRequired: true);

        try
        {
            await Guild.CreateApplicationCommandAsync(RoleCommand.Build()); // Use passed Guild object
        }
        catch (Exception Error)
        {
            Console.WriteLine(Error);
        }
    }
}