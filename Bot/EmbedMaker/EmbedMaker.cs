using Discord;
using Discord.WebSocket;
using Victoria;
using Victoria.Player;

namespace Bot.EmbedMaker;

public class EmbedMaker
{
    public Task<EmbedBuilder> PlayQueue(LavaTrack track, bool queue, SocketUser? user, int queuePosition = 0)
    {

        var embed = new EmbedBuilder { }
            .WithAuthor(track.Author)
            .WithTitle(track.Title)
            .WithDescription($"Requested by {user.Mention}")
            .WithFooter(queue
                ? $"Queue Position: {queuePosition} | Length: {track.Duration}"
                : $"Length: {track.Duration}")
            .WithColor(queue ? Color.Gold : Color.Teal)
            .WithImageUrl(track.FetchArtworkAsync().Result);
            

        return Task.FromResult(embed);
    }
    
    public Task<EmbedBuilder> JoinLeave(SocketUser user, bool join)
    {

        var embed = new EmbedBuilder { }
            .WithTitle(join ? "Connected!" : "Disconnected!")
            .WithDescription(join
                ? $"Connected to {(user as IVoiceState)?.VoiceChannel.Mention}."
                : $"Left {(user as IVoiceState)?.VoiceChannel.Mention}.")
            .WithColor(join ? Color.Green : Color.Red);
            

        return Task.FromResult(embed);
    }
    
    public Task<EmbedBuilder> ErrorMessage(string errorMessage)
    {

        var embed = new EmbedBuilder { }
            .WithTitle("Error!")
            .WithDescription(errorMessage);
            
        return Task.FromResult(embed);
    }
    
    public Task<EmbedBuilder> Update(string updateMessage)
    {

        var embed = new EmbedBuilder { }
            .WithTitle("Update!")
            .WithDescription(updateMessage)
            .WithColor(Color.LightOrange);
            
        return Task.FromResult(embed);
    }

}