using Discord.Net.Rest;
using Microsoft.VisualBasic;

namespace Bot;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
public class BigBrotherModeration
{
    private readonly DiscordSocketClient _Client;
    
    public BigBrotherModeration(DiscordSocketClient GetClient)
    {
        _Client = GetClient;
    }

    public async Task CheckContents(SocketMessage Message)
    {
        Console.WriteLine("Message Detected");
        
        if (Message.Content.Contains("Us Girls") || Message.Content.Contains("Us Boys"))
        {
            await Message.DeleteAsync();
            await Message.Author.SendMessageAsync("Your message was deleted because it contained a phrase that is not allowed.", true);
        }
        else
        {
            if (Message.Content != null)
            {
                await Message.Author.SendMessageAsync(Message.Content);
            }
        }
    }
        
}