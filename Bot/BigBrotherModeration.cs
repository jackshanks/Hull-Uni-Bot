﻿using Discord.Net.Rest;
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
    private static readonly string[] UnpermittedPhrases = new string[] { "us girls", "us boys" };

    public BigBrotherModeration(DiscordSocketClient GetClient)
    {
        _Client = GetClient;
    }

    public async Task CheckContents(IMessage Message)
    {
        if (UnpermittedPhrases.Any(Phrase => Message.Content.ToLower().Contains(Phrase)))
        {
            await Message.DeleteAsync();
            await Message.Channel.SendMessageAsync($"{Message.Author.Mention}, your message was deleted because it contained a phrase that is not allowed.");
        }
    }
}