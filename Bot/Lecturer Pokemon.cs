using System.Text;
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

    public async Task<string> GetLecturerInfo(string LecturerName = "")
    {
        string Name = "";
        const string Input = "test"; 
        await Sqlite.OpenAsync();
        Command = Sqlite.CreateCommand();
        Command.CommandText =
            @"
                SELECT LecturerNames
                FROM Lecturers
                WHERE LecturerNames = '$LecturerName'
            ";
        
        Command.Parameters.AddWithValue("$LecturerName", Input);
        StringBuilder NameBuilder = new StringBuilder();

        await using (var Reader = await Command.ExecuteReaderAsync())
        {
            while (await Reader.ReadAsync())
            {
                NameBuilder.AppendLine(Reader.GetString(0));
            }
        }

        await Sqlite.CloseAsync();
        return await Task.FromResult(NameBuilder.ToString());
    }
}   