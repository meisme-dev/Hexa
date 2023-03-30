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
        public ApplicationCommandProperties? properties { get; set; }
        public ulong guild { get; set; }
    }

    public class SlashCommandHandler
    {
        private DiscordSocketClient client;
        public List<SlashCommand> slashCommands = new List<SlashCommand>();
        private List<ApplicationCommandProperties> applicationCommandProperties = new();

        public SlashCommandHandler(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task RegisterSlashCommand(SlashCommand slashCommand)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Info, "Register", "Register slash command with name: " + slashCommand.name + ", description: " + slashCommand.description);
            Console.WriteLine(logMessage.ToString());
            SlashCommandBuilder slashCommandBuilder = new SlashCommandBuilder()
                .WithName(slashCommand.name)
                .WithDescription(slashCommand.description)
                .AddOptions(slashCommand.options.ToArray());
            try
            {
                if(slashCommand.guild != 0) {
                    await client.GetGuild(slashCommand.guild).CreateApplicationCommandAsync(slashCommandBuilder.Build());
                } else {
                    await client.CreateGlobalApplicationCommandAsync(slashCommandBuilder.Build());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            slashCommand.properties = slashCommandBuilder.Build();
            slashCommands.Add(slashCommand);           
        }

        public void DeleteAllSlashCommands() {
            List<ApplicationCommandProperties> applicationCommandProperties = new List<ApplicationCommandProperties>();
            foreach(var slashCommand in slashCommands) {
                ArgumentNullException.ThrowIfNull(slashCommand.properties);
                applicationCommandProperties.Add(slashCommand.properties);
            }
            client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
        }
    }
}