using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using kalCasino.Database;

namespace kalCasino.Commands;

public class Moderator : ApplicationCommandModule
{
    private const string ErrorColor = "F64444";
    private const string NeutralColor = "AC60F9";

    [SlashCommand("return", "Забрать реткоины", false)]
    [SlashRequirePermissions(Permissions.BanMembers)]
    public async Task Return(InteractionContext ctx, 
        [Option("value", "Выбери сколько ты хочешь забрать")] long returnValue,
        [Option("user", "Выбери у кого ты хочешь забрать")] DiscordUser commandUser)
    {
        await ctx.CreateResponseAsync(InteractionResponseType
            .DeferredChannelMessageWithSource);

        var log = new Logger();
        log.CommandСall(ctx);
        
        DiscordEmbedBuilder sendingEmbed;

        if (commandUser.IsBot 
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
                    Text = "Попробуй забрать у других"
                }
            };
        }
        else if (commandUser == ctx.Client.CurrentUser)
        {
            sendingEmbed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = "Ошибка"
                },
                Title = "Нельзя забирать реткоины у бота казино",
                Description = "Это может разрушить вселенную",
                Color = new DiscordColor(ErrorColor),
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Попробуй забрать у других"
                }
            };
        }
        else if (commandUser == ctx.User)
        {
            sendingEmbed = new DiscordEmbedBuilder
            {
                Title = "Ты не можешь забрать реткоины сам у себя",
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = "Ошибка"
                },
                Color = new DiscordColor(ErrorColor),
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Попробуй забрать у других"
                }
            };
        }
        else
        {
            await using var db = new DataContext();
            
            var userFromDb = await new DbUser(db, commandUser.Id).GetUser();
            var botFromDb = await new DbUser(db, ctx.Client.CurrentUser.Id).GetUser();

            if (userFromDb.Balance < returnValue)
            {
                sendingEmbed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = commandUser.Username,
                        IconUrl = commandUser.AvatarUrl
                    },
                    Title = "Ошибка",
                    Description = "На балансе недостаточно реткоинов",
                    Color = new DiscordColor(ErrorColor),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "Попробуй забрать у других"
                    }
                };
            }
            else
            {
                userFromDb.Balance -= returnValue;
                botFromDb.Balance += returnValue;
                
                var returnRat = new Rat(returnValue);
                var resultRat = new Rat(userFromDb.Balance);

                sendingEmbed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = commandUser.Username,
                        IconUrl = commandUser.AvatarUrl
                    },
                    Title = $"Партия забрать себе {returnValue} {returnRat.Word}",
                    Description = $"Баланс теперь {userFromDb.Balance} {resultRat.Word}",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = @"https://cdn.discordapp.com/attachments/1002188468174196756/1003205839693283379/unknown.png"
                    },
                    Color = new DiscordColor(NeutralColor)
                };
            }
        }
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .AddEmbed(sendingEmbed));
    }
}