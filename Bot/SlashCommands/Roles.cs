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

public class RoleCommands : InteractionModuleBase<SocketInteractionContext>
{

    private readonly DiscordSocketClient _Discord;
    private readonly InteractionService _Interactions;
    private readonly IServiceProvider _Services;
    private readonly Random _Random;
    private readonly string[] _SimonFacts;

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
    [SlashCommand("spawner","Create the colour role menu")]
    public async Task ColourRole()
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select an option").WithCustomId("colour-role")
            .AddOption("Red", "red").AddOption("Yellow", "yellow").AddOption("Green", "green")
            .AddOption("Cyan", "cyan").AddOption("Blue", "blue").AddOption("Dark Blue", "darkblue")
            .AddOption("Purple", "purple").AddOption("Pink", "pink").AddOption("Silver", "silver");

        var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);

        await RespondAsync("Select your colour!", components: builder.Build());
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
                {"purple", 1218193017530482739}, {"pink", 1218193095540211773}, {"silver", 1218193213622452344}
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
}