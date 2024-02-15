using Discord;
using Discord.Interactions;
using System;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Bot.LogHandle;
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

    [SlashCommand("role", "Use this to change your role")]
    public Task RoleCommand(string Name, string Hex)
    {
        var Guild = _Discord.GetGuild(1153315295306465381);
        SocketGuildUser User = (SocketGuildUser)Context.User;

        //Set the role name and hex code
        string RoleName = ("$"+Name);
        Color Color = new Color(uint.Parse(Hex, System.Globalization.NumberStyles.HexNumber));
            
            
        // Get the role ID of any role with $
        ulong RoleId = User.Roles.FirstOrDefault(X => X.Name.StartsWith("$"))?.Id ?? 0;

        // Find the role to delete using SocketRole if an ID is found
        if (RoleId != 0)
        {
            SocketRole RoleToDelete = Guild.Roles.FirstOrDefault(X => X.Id == RoleId)!;
            RoleToDelete.DeleteAsync();
        }
            
        var Role = Guild.CreateRoleAsync(RoleName, null, Color);
        var HopeRole = Guild.GetRole((ulong)Role.Id);

        ulong RoleId2 = Guild.Roles.FirstOrDefault(X => X.Name.StartsWith("/"))?.Id ?? 0;

        if (RoleId2 != 0)
        {
            SocketRole Role2 = Guild.Roles.FirstOrDefault(X=> X.Id == RoleId2)!;
            HopeRole.ModifyAsync(X => X.Position = Role2.Position);

        }

        User.AddRoleAsync(HopeRole);

        return RespondAsync($"**Role Name:** {RoleName} \n**Hex Code:** {Hex}");
    }

    [SlashCommand("simon-fact", "Get a random fact Simon!")]
    public Task SimonFact()
    {
        var RandomInt = _Random.Next(0, _SimonFacts.Length);
        return RespondAsync($"Did you know that Simon {_SimonFacts[RandomInt]}");
    }

}