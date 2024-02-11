using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
using Microsoft.Data.Sqlite;


namespace Bot;

public class LecturerPokemon
{
    public DiscordSocketClient Client { get; }
    public string BotDirectory { get; }
    private  SqliteCommand Command { get; }

    public LecturerPokemon(DiscordSocketClient Client)
    {
        this.Client = Client;
        BotDirectory = Directory.GetCurrentDirectory();
        
        using (var Connection = new SqliteConnection("Data Source=Pokemon Database.db"))
        {
            Connection.Open();
            Command = Connection.CreateCommand();
            Command.CommandText =
                @"
                SELECT LecturerNames
                FROM Lecturers
                WHERE LecturerName = 'Test'
                ";
        }
    }

    public string GetLecturerInfo(string LecturerName = "")
    {
        using (var Reader = Command.ExecuteReader())
        {
            while (Reader.Read())
            {
                var Name = Reader.GetString(0);

                return Name;
            }
        }

        return "null";
    }
}