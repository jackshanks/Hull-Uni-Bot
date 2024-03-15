using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Bot.LogHandle;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;


namespace Bot.SlashCommands;

public class FunCommands : InteractionModuleBase<SocketInteractionContext>
{
    
    private readonly DiscordSocketClient _Discord;
    private readonly InteractionService _Interactions;
    private readonly IServiceProvider _Services;
    private readonly Random _Random;
    private readonly string[] _SimonFacts;

    public FunCommands(
        DiscordSocketClient Discord,
        InteractionService Interactions,
        IServiceProvider Services,
        ILogger<InteractionService> Logger)
    {
        _Discord = Discord;
        _Interactions = Interactions;
        _Services = Services;
        _Random = new();
        _SimonFacts = new[] {
            "created the hamburger!", 
            "doesn't need to use the toilet!",
            "was born on top a sacred mountain!",
            "is loved by everyone across the world!",
            "learnt to drive when he was 3!",
            "doesn't have a spacebar; he simply stares at the words until they separate out of sheer respect!",
            "is a skilled amateur chess player who frequently participates in global tournaments!",
            "completed a charity unicycle ride along a significant section of the Great Wall of China!",
            "enjoys experimenting with unique recipes, including a watermelon-infused pizza that became an international hit!",
            "participated in a community hopscotch event, impressively lasting for several hours and gaining global recognition!"
        };
            
        _Interactions.Log += Msg => LogHelper.OnLogAsync(Logger, Msg);
    }
    [SlashCommand("ping", "Play some table tennis")]
    public Task PingCommand()
        => RespondAsync($"Pong! Your response time was {Context.Client.Latency} ms");

    [SlashCommand("simon-fact", "Get a random fact Simon!")]
    public Task SimonFact()
    {
        var RandomInt = _Random.Next(0, _SimonFacts.Length);
        return RespondAsync($"Did you know that Simon {_SimonFacts[RandomInt]}");
    }

    [SlashCommand("game-role", "Add games you are interested in!")]
    public Task GameRole([Choice ("League of Legends", "lol"), 
                          Choice("Valorant","valorant"),
                          Choice("Overwatch","overwatch"),
                          Choice("Helldivers 2","helldivers"),
                          Choice("Stardew Vally","stardew"),
                          Choice("Lethal Company","lethal")]string roleName)
            
    {
        SocketRole role;
        var user = Context.User;

        ulong roleId = roleName switch
        {
            "overwatch" => 1216878995916853409,
            "lol" => 1217811848385138748,
            "valorant" => 1217811869159395469,
            "helldivers" => 1217811907126231131,
            "stardew" => 1217811946733310065,
            "lethal" => 1213923585937113100,
            _ => 0
        };

        if (roleId != 0)
        {
            role = Context.Guild.GetRole(roleId);
            if (user is SocketGuildUser sgu)
            {
                if (sgu.Roles.Any(userRole => userRole.Id == role.Id))
                {
                    (user as IGuildUser)?.RemoveRoleAsync(role);
                    return RespondAsync($"You already have the {role.Name} role so it has been removed.",
                        ephemeral: true);
                }
                else
                {
                    (user as IGuildUser)?.AddRoleAsync(role);
                    return RespondAsync($"The role {role.Name} has been added.", ephemeral: true);
                }
            }
        }
        return ReplyAsync("Error, role not found.");
    }
    
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("spawner","Create the colour role menu")]
    public async Task ColourRole()
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select an option")
            .WithCustomId("colour-role")
            .AddOption("Red", "red")
            .AddOption("Yellow", "yellow");

        var builder = new ComponentBuilder()
            .WithSelectMenu(menuBuilder);

        await RespondAsync("Select your colour!", components: builder.Build());
    }

    [ComponentInteraction("colour-role")]
    private async Task MyMenuHandler(SocketMessageComponent interaction)
    {
        var user = interaction.User;
        try
        {
            var selectedColour = interaction.Data.ToString()!;

            switch (selectedColour)
            {
                case "red":
                    // Add the red role to the user
                    await ((user as IGuildUser)!).AddRoleAsync(1217889821905649746);
                    await interaction.RespondAsync("You have selected the red role!",ephemeral:true);
                    break;
                case "yellow":
                    // Add the yellow role to the user
                    await ((user as IGuildUser)!).AddRoleAsync(1217897797903188069);
                    await interaction.RespondAsync("You have selected the yellow role!",ephemeral:true);
                    break;
                default:
                    await interaction.RespondAsync("Invalid selection. Please try again.");
                    break;
            }
        }
        catch (Exception ex)
        {
            // Log any errors
            Console.WriteLine(ex.Message);
            await interaction.RespondAsync("An error occurred while processing your request. Please try again later.");
        }
    }
}