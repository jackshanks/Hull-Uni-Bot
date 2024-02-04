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
    private static readonly string[] UnpermittedPhrases = new string[] { "usgirls", "usboys", "nigger", "nigga" };

    public BigBrotherModeration(DiscordSocketClient GetClient)
    {
        _Client = GetClient;
    }

    public async Task CheckContents(IMessage Message)
    {
        Console.WriteLine(Message.CleanContent.ToLower().Trim());
        if (UnpermittedPhrases.Any(Phrase => Message.CleanContent.ToLower().Replace(" ", "").Contains(Phrase)))
        {
            await Message.DeleteAsync();
            await Message.Channel.SendMessageAsync($"{Message.Author.Mention}, your message was deleted because it contained a phrase that is not allowed.");
        }
        //if (Message.Content.ToLower().Contains(""))
    }
}