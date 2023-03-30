using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

public class Program
{
    private DiscordSocketClient client;
    private Hexa.SlashCommandHandler slashCommandHandler;
    private string token;

    public static Task Main(string[] args) => new Program().Run();

    public Program()
    {
        client = new DiscordSocketClient();
        slashCommandHandler = new Hexa.SlashCommandHandler(client);
        client.Log += Log;
        client.Ready += Ready;
        client.SlashCommandExecuted += SlashCommandExecuted;
        IConfigurationRoot? config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        string? token = config["token"];
        ArgumentNullException.ThrowIfNull(token);
        this.token = token;
    }

    public async Task Run()
    {
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
        await Task.Delay(-1);
    }

    public async void EchoHandler(SocketSlashCommand cmd)
    {
        try
        {
            await cmd.RespondAsync(text: (string)(cmd.Data.Options.First().Value));
        }
        catch (Exception e)
        {
            await cmd.RespondAsync(text: e.ToString());
        }
    }

    private Task Ready()
    {
        SlashCommandOptionBuilder builder = new SlashCommandOptionBuilder()
        .AddOption(
            name: "text",
            ApplicationCommandOptionType.String,
            "Text",
            isRequired: true
        );
        List<SlashCommandOptionBuilder> options = new() {
            builder
        };

        Hexa.SlashCommand slashCommand = new Hexa.SlashCommand()
        {
            name = "echo",
            description = "Repeats the text you give it",
            options = options,
            handler = EchoHandler,
        };

        slashCommandHandler?.RegisterSlashCommand(slashCommand).Wait();
        return Task.CompletedTask;
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private Task SlashCommandExecuted(SocketSlashCommand slashCommand)
    {
        foreach (var command in slashCommandHandler.slashCommands)
        {
            if (command.name == slashCommand.CommandName)
            {
                command.handler.Invoke(slashCommand);
            }
        }
        return Task.CompletedTask;
    }
}