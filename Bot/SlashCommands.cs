namespace DefaultNamespace;

using Discord.Commands;

public class MyCommandsModule : ModuleBase<SocketCommandContext>
{
    [Command("Ping")]
    public async Task PingAsync()
    {
        await ReplyAsync("Pong!");
    }
}