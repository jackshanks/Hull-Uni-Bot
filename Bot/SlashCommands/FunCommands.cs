using Discord;
using Discord.Interactions;
using System;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Discord.WebSocket;


namespace Bot.SlashCommands;

public class FunCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly Random _Random = new();
    private readonly string[] _SimonFacts = {
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
    
    [SlashCommand("ping", "Play some table tennis")]
    public Task PingCommand()
        => RespondAsync($"Pong! Your response time was {Context.Client.Latency} ms");

    [SlashCommand("role", "Use this to change your role")]
    public Task RoleCommand(string Name, string Hex)
    {
        var Guild = Context.Guild;
        string RoleName = "$" + Name;
        
        Color Color = new Color(uint.Parse(Hex, System.Globalization.NumberStyles.HexNumber));
        
        ulong CurrentRoleId = Guild.CurrentUser.Roles.FirstOrDefault(R => R.Name.StartsWith("$"))?.Id ?? 0;
        if (CurrentRoleId != 0)
        {
            SocketRole RoleToDelete = Guild.Roles.FirstOrDefault(r2 => r2.Id == CurrentRoleId);
            RoleToDelete?.DeleteAsync();
        }
        
        var NewRole = Guild.CreateRoleAsync(RoleName, null, Color);
        var NewRoleId = Guild.Roles.FirstOrDefault(F => F.Name.StartsWith(NewRole.ToString()!))?.Id ?? 0;
        
        ulong WantedPositionId = Guild.Roles.FirstOrDefault(X => X.Name.StartsWith("/"))?.Id ?? 0;
        
        if (WantedPositionId != 0)
        {
            SocketRole Role2 = Guild.Roles.FirstOrDefault(r4=> r4.Id == WantedPositionId);
            Guild.GetRole(NewRoleId).ModifyAsync(P => P.Position = Role2.Position);
        }

        Guild.CurrentUser.AddRoleAsync(Context.Guild.GetRole(NewRoleId));
        
        return RespondAsync($"**Role Name:** {RoleName} \n**Hex Code:** {Hex}");
    }

    [SlashCommand("simon-fact", "Get a random fact Simon!")]
    public Task SimonFact()
    {
        var RandomInt = _Random.Next(0, _SimonFacts.Length);
        return RespondAsync($"Did you know that Simon {_SimonFacts[RandomInt]}");
    }

}