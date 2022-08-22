using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace kalCasino.Commands;

public class Other : ApplicationCommandModule
{
    private const string NeutralColor = "AC60F9";
    
    [SlashCommand("ping", "Жив ли бот?")]
    public async Task Ping(InteractionContext ctx)
    {
        var log = new Logger();
        log.CommandСall(ctx);
        
        var onlineEmbed = new DiscordEmbedBuilder
        {
            Title = "Бот на связи ёпта",
            Color = new DiscordColor(NeutralColor)
        };
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
            new DiscordInteractionResponseBuilder().AddEmbed(onlineEmbed));
    }
}