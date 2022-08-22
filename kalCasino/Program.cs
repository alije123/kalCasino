using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using kalCasino;

var host = Host.CreateDefaultBuilder()   
    .ConfigureDiscordHost((context, config) =>
    {
        config.SocketConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All | GatewayIntents.AllUnprivileged,
            LogLevel = LogSeverity.Verbose,
            AlwaysDownloadUsers = true,
            MessageCacheSize = 200
        };

        config.Token = context.Configuration["token"];
    })
    // Optionally wire up the command service
    .UseCommandService((context, config) =>
    {
        config.DefaultRunMode = RunMode.Async;
        config.CaseSensitiveCommands = false;
    })
    // Optionally wire up the interactions service
    .UseInteractionService((context, config) =>
    {
        config.LogLevel = LogSeverity.Info;
        config.UseCompiledLambda = true;
    })
    .ConfigureServices((context, services) =>
    {
        //Add any other services here
        services.AddHostedService<CommandHandler>();
        services.AddHostedService<InteractionHandler>();
    }).Build();
  
await host.RunAsync();