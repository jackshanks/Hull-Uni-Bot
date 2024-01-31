namespace DefaultNamespace;

public async Task Client_Ready()
{
    var guild = client.GetGuild(guildId);

    // SlashCommandBuilder
    var PingPong = new SlashCommandBuilder();

    // CommandName
    PingPong.WithName("Ping");

    // Descriptions can have a max length of 100.
    PingPong.WithDescription("Am I online? Lets play table tennis to find out.");

    try
    {
        await guild.CreateApplicationCommandAsync(PingPong.Build());
    }
    catch(ApplicationCommandException exception)
    {
        var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
        Console.WriteLine(json);
    }
}