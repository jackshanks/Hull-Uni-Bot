namespace DefaultNamespace;

public class AutoRole : DiscordSocketClient
{
    protected override async Task OnMemberJoined(SocketGuildUser user)
    {
        var unverified = user.Guild.Roles.FirstOrDefault(x => x.Name == "Unverified");
        if (unverified != null)
        {
            await user.AddRolesAsync(unverified);
        }
    }
}