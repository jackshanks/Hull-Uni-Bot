using System.Threading.Channels;
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
    private static readonly string[] UnpermittedPhrases = new string[] { "usgirls", "usboys", "nigger", "nigga", "reunion" };
    private readonly SocketGuild _Guild;

    public BigBrotherModeration(DiscordSocketClient GetClient)
    {
        _Client = GetClient;
        _Guild = _Client.GetGuild(1153315295306465381);
    }

    public async Task CheckContents(IMessage Message)
    {
        if (UnpermittedPhrases.Any(Phrase => Message.CleanContent.ToLower().Replace(" ", "").Contains(Phrase)))
        {
            await Message.DeleteAsync();
            await Message.Channel.SendMessageAsync($"{Message.Author.Mention}, your message was deleted because it contained a phrase that is not allowed.");
        }

        if (Message.Channel.Id == 1203755914042015814)
        {
            SocketTextChannel Channel = (SocketTextChannel)await _Client.GetChannelAsync(1153315299249102921);
            await Channel.SendMessageAsync(Message.Content);
        }
    }
}