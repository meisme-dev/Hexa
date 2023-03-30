using Discord;
using Discord.WebSocket;

namespace Hexa
{
    public struct SlashCommand
    {
        required public string name { get; set; }
        required public string description { get; set; }
        required public List<SlashCommandOptionBuilder> options { get; set; }
        required public Action<SocketSlashCommand> handler { get; set; }
    }

    public class SlashCommandHandler
    {
        private DiscordSocketClient client;
        public List<SlashCommand> slashCommands = new List<SlashCommand>();

        public SlashCommandHandler(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task RegisterSlashCommand(SlashCommand slashCommand)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Info, "Register", "Register slash command with name: " + slashCommand.name + ", description: " + slashCommand.description);
            Console.WriteLine(logMessage.ToString());
            SlashCommandBuilder slashCommandBuilder = new SlashCommandBuilder()
                .WithName("test")
                .WithDescription("test")
                .AddOptions(slashCommand.options.ToArray());
            try
            {
                await client.CreateGlobalApplicationCommandAsync(slashCommandBuilder.Build());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            slashCommands.Add(slashCommand);
        }
    }
}