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
        var User = Guild.GetUser(Context.User.Id);
        string RoleName = "$" + Name;
        
        Color Color = new Color(uint.Parse(Hex, System.Globalization.NumberStyles.HexNumber));
        
        var CurrentRole = User.Roles.FirstOrDefault(X => X.Name.StartsWith("$")) ?? null;
        if (CurrentRole != null) { CurrentRole.DeleteAsync(); }
        
        var NewRoleTemp = Guild.CreateRoleAsync(RoleName, null, Color);
        var NewRole = Context.Guild.Roles.FirstOrDefault(X => X.Name==NewRoleTemp.ToString());
        
        
        var WantedPosition =  Guild.Roles.FirstOrDefault(X => X.Name.StartsWith("/")) ?? null;
        
        if (WantedPosition != null && NewRole != null)
        {
            NewRole.ModifyAsync(P => P.Position = WantedPosition.Position);
        }

        User.AddRoleAsync(NewRole);
        
        return RespondAsync($"**Role Name:** {RoleName} \n**Hex Code:** {Hex}");
    }

    [SlashCommand("simon-fact", "Get a random fact Simon!")]
    public Task SimonFact()
    {
        var RandomInt = _Random.Next(0, _SimonFacts.Length);
        return RespondAsync($"Did you know that Simon {_SimonFacts[RandomInt]}");
    }

}