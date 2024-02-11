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
    private  SqliteCommand Command { get; set; }
    private string DataBaseDirectory { get; }

    private SqliteConnection Sqlite { get; }

    public LecturerPokemon(DiscordSocketClient Client)
    {
        this.Client = Client;
        DataBaseDirectory = Directory.GetCurrentDirectory() + "/Pokemon Database.db";
        Sqlite = new SqliteConnection($"Data Source={DataBaseDirectory}");
        Command = Sqlite.CreateCommand();

    }

    public string GetLecturerInfo(string LecturerName = "")
    {
        string Name = "";
        string Input = "test"; 
        Sqlite.OpenAsync();
        Command = Sqlite.CreateCommand();
        Command.CommandText =
            @"
                SELECT LecturerNames
                FROM Lecturers
                WHERE LecturerName = '$LecturerName'
            ";
        Command.Parameters.AddWithValue("$LecturerName", Input);
        
        using (var Reader = Command.ExecuteReader())
        {
            while (Reader.Read())
            {
                Name = Reader.GetString(0);
            }
        }

        Sqlite.CloseAsync();
        return Name;
    }
} 