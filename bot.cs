using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using kalCasino.Commands;
using Microsoft.Extensions.Logging;
using DSharpPlus.SlashCommands;

namespace kalCasino;

public class Bot
{
    public DiscordClient? Client { get; private set; }
    public InteractivityExtension? Interactivity { get; private set; }
    public CommandsNextExtension? Commands { get; private set; }
    public SlashCommandsExtension SlashCommands { get; private set; }

    public async Task RunAsync()
    {
        var config = new DiscordConfiguration
        {
            Token = "MTAwMTYyNDk2MjUzODQyNjM5OA.G-Gkvd.iwZVdSvAq8I3VZGhHJ9QcY-XIUp4whs2C-Q3H8",
            TokenType = TokenType.Bot,
            MinimumLogLevel = LogLevel.Debug,
            AutoReconnect = true
        };

        Client = new DiscordClient(config);
        
        var slash = Client.UseSlashCommands();

        Client.Ready += OnClientReady;

        Client.UseInteractivity(new InteractivityConfiguration
        {
            Timeout = TimeSpan.FromMinutes(2)
        });

        slash.RegisterCommands<KalCommands>(1001625197503332525);

        await Client.ConnectAsync();

        await Task.Delay(-1);
    }

    private static Task OnClientReady(object sender, ReadyEventArgs e)
    {
        return Task.CompletedTask;
    }
}