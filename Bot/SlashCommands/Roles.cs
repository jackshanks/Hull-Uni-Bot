using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Bot.LogHandle;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace Bot.SlashCommands;

public class RoleCommands : InteractionModuleBase<SocketInteractionContext>
{

    private readonly DiscordSocketClient _Discord;
    private readonly InteractionService _Interactions;
    private readonly IServiceProvider _Services;

    public RoleCommands(
        DiscordSocketClient discord,
        InteractionService interactions,
        IServiceProvider services,
        ILogger<InteractionService> logger)
    {
        _Discord = discord;
        _Interactions = interactions;
        _Services = services;
        _Interactions.Log += msg => LogHelper.OnLogAsync(logger, msg);
    }
    
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("spawner-main","Create the colour role menu")]
    public async Task ColourRole()
    {
        var menuBuilder1 = new SelectMenuBuilder()
            .WithPlaceholder("Select an option").WithCustomId("colour-role")
            .AddOption("Red", "red").AddOption("Yellow", "yellow").AddOption("Green", "green")
            .AddOption("Cyan", "cyan").AddOption("Blue", "blue").AddOption("Dark Blue", "darkblue")
            .AddOption("Purple", "purple").AddOption("Pink", "pink").AddOption("Silver", "silver")
            .AddOption("Orange", "orange");

        var builder1 = new ComponentBuilder().WithSelectMenu(menuBuilder1);
        
        var menuBuilder2 = new SelectMenuBuilder()
            .WithPlaceholder("Select an option").WithCustomId("game-role")
            .AddOption("League of Legends", "lol").AddOption("Valorant", "valorant")
            .AddOption("Overwatch", "overwatch").AddOption("Helldivers 2", "helldivers")
            .AddOption("Stardew Valley", "stardew").AddOption("Lethal Company", "lethal");
        
        var builder2 = new ComponentBuilder().WithSelectMenu(menuBuilder2);

        await ReplyAsync("Select your colour!", components: builder1.Build());
        await RespondAsync("Select your game roles!", components: builder2.Build());
    }
    
    [ComponentInteraction("colour-role")]
    public async Task MyMenuHandler(string selectedRole)
    {
        try
        {
            var user = Context.User as IGuildUser;
            IReadOnlyCollection<ulong> userroles = user!.RoleIds;
            
            var roleIdMap = new Dictionary<string, ulong>
            {
                {"red", 1217889821905649746}, {"yellow", 1217897797903188069}, {"green", 1218192603330117713}, 
                {"cyan", 1218192706354806915}, {"blue", 1218192806577967174}, {"darkblue", 1218192942439858176},
                {"purple", 1218193017530482739}, {"pink", 1218193095540211773}, {"silver", 1218193213622452344},
                {"orange", 1218217701160517692}
            };

            foreach (var userRole in userroles)
            {
                if (roleIdMap.TryGetValue(Context.Guild.GetRole(userRole).Name, out var removeRoleId))
                { await user!.RemoveRoleAsync(removeRoleId); }
            }

            if (roleIdMap.TryGetValue(selectedRole, out var addRoleId))
            {
                await user!.AddRoleAsync(addRoleId);
                await RespondAsync($"You have selected the {selectedRole} role!", ephemeral: true);
            }
            else { await RespondAsync("Invalid selection. Please try again."); }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await RespondAsync("An error occurred while processing your request. Please try again later.");
        }
    }

    [ComponentInteraction("game-role")]
    public async Task GameRole(string selectedRole)
    {
        var user = Context.User as IGuildUser;

        var roleIdMap = new Dictionary<string, ulong>()
        {
            { "overwatch", 1216878995916853409 }, { "lol", 1217811848385138748 }, {"valorant", 1217811869159395469},
            {"helldivers", 1217811907126231131}, {"stardew", 1217811946733310065}, {"lethal", 1213923585937113100}
        };
        
        if (roleIdMap.TryGetValue(selectedRole, out var addRoleId))
        {
            await user!.AddRoleAsync(addRoleId);
            await RespondAsync($"You have selected the {selectedRole} role!", ephemeral: true);
        }
        else { await RespondAsync("Invalid selection. Please try again."); }
    }
}