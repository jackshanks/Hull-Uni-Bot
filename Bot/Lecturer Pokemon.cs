using Discord.WebSocket;

namespace Bot;

public class LecturerPokemon
{
    private readonly DiscordSocketClient _Client;
    
    public LecturerPokemon(DiscordSocketClient Client)
    {
        _Client = Client;
    }
}