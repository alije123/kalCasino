using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kalCasino.Database;

namespace kalCasino.Commands;

public class Plain : ApplicationCommandModule
{
    private const string ErrorColor = "F64444";
    private const string NeutralColor = "AC60F9";
    
    [SlashCommand("balance", "Сколько у тебя на балансе?")]
    public async Task Balance(InteractionContext ctx, 
        [Option("user", "Ты можешь выбрать другого человека")] DiscordUser? commandUser = null)
    {
        await ctx.CreateResponseAsync(InteractionResponseType
            .DeferredChannelMessageWithSource);
        
        var log = new Logger();
        log.CommandСall(ctx);
        
        DiscordEmbedBuilder sendingEmbed;

        if (commandUser != null
            && commandUser.IsBot 
            && commandUser != ctx.Client.CurrentUser)
        {
            sendingEmbed = new DiscordEmbedBuilder
            {
                Title = "У железяк не бывает реткоинов",
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = "Ошибка"
                },
                Color = new DiscordColor(ErrorColor),
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Попробуй посмотреть у других"
                }
            };
        }
        else
        {
            var findUser = commandUser ?? ctx.User;
            
            await using var db = new DataContext();
            
            var userFromDb = await new DbUser(db, findUser.Id).GetUser();
            
            var balance = userFromDb.Balance;

            var rat = new Rat(balance);
        
            sendingEmbed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = findUser.AvatarUrl,
                    Name = findUser.Username
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Баланс"
                },
                Title = $"{balance} {rat.Word}",
                Color = new DiscordColor(NeutralColor),
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = @"https://cdn.discordapp.com/attachments/1002188468174196756/1002188621748649995/balance.png"
                }
            };
        }
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .AddEmbed(sendingEmbed));
    }

}