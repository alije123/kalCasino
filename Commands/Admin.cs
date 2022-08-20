using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using kalCasino.Database;
using Microsoft.EntityFrameworkCore;

namespace kalCasino.Commands;

public class Admin : ApplicationCommandModule
{
    private const string ErrorColor = "F64444";
    private const string NeutralColor = "AC60F9";
    
    [SlashCommand("give", "Дать реткоины", false)]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task Add(InteractionContext ctx,
        [Option("value", "Выбери сколько ты хочешь выдать")] long giveValue, 
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
                Title = "Железяки не принимают реткоины",
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = "Ошибка"
                },
                Color = new DiscordColor(ErrorColor),
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Попробуй дать другим"
                }
            };
        }
        else if (commandUser == ctx.Client.CurrentUser)
        {
            sendingEmbed = new DiscordEmbedBuilder
            {
                Title = "Нельзя давать реткоины боту казино",
                Description = "Это может разрушить вселенную",
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = "Ошибка"
                },
                Color = new DiscordColor(ErrorColor),
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "Попробуй дать другим"
                }
            };
        }
        else
        {
            var findUser = commandUser ?? ctx.User;
            
            await using var db = new DataContext();
            
            var userFromDb = await new DbUser(db, findUser.Id).GetUser();
            var botFromDb = await db.Users.FirstAsync(p => p.DiscordId == ctx.Client.CurrentUser.Id);

            userFromDb.Balance += giveValue;
            botFromDb.Balance -= giveValue;
            await db.SaveChangesAsync();
            
            var balance = userFromDb.Balance;
            
            var rat = new Rat(balance);
            var kal = new Rat(giveValue);
            
            sendingEmbed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = commandUser?.AvatarUrl ?? ctx.User.AvatarUrl,
                    Name = commandUser?.Username ?? ctx.User.Username
                },
                Title = $"Поступить зачисление в размере {giveValue} {kal.Word}",
                Description = $"Баланс теперь {balance} {rat.Word}",
                Color = new DiscordColor(NeutralColor),
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = @"https://cdn.discordapp.com/attachments/1002188468174196756/1002592396434997308/unknown.png"
                }
            };
        }
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .AddEmbed(sendingEmbed));
    }

    [SlashCommand("take", "Забрать себе реткоины", false)]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task Subtract(InteractionContext ctx, 
        [Option("value", "Выбери сколько ты хочешь забрать")] long takeValue,
        [Option("user", "Выбери у кого ты хочешь забрать")] DiscordUser commandUser)
    {
        await ctx.CreateResponseAsync(InteractionResponseType
            .DeferredChannelMessageWithSource);

        var log = new Logger();
        log.CommandСall(ctx);

        DiscordEmbedBuilder sendingEmbed;

        if (commandUser.IsBot && commandUser != ctx.Client.CurrentUser)
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

            var adminFromDb = await new DbUser(db, ctx.User.Id).GetUser();

            if (userFromDb.Balance < takeValue)
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
                userFromDb.Balance -= takeValue;
                adminFromDb.Balance += takeValue;

                await db.SaveChangesAsync();
                
                var takeRat = new Rat(takeValue);
                var resultRat = new Rat(userFromDb.Balance);

                sendingEmbed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = commandUser.Username,
                        IconUrl = commandUser.AvatarUrl
                    },
                    Title = $"{ctx.User.Username} забрать себе {takeValue} {takeRat.Word}",
                    Description = $"Баланс теперь {userFromDb.Balance} {resultRat.Word}",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = @"https://cdn.discordapp.com/attachments/1002188468174196756/1003133521918951476/unknown.png"
                    },
                    Color = new DiscordColor(NeutralColor)
                };
            }
        }
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .AddEmbed(sendingEmbed));
    }
}