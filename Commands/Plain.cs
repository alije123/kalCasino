using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using kalCasino.Database;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                var findUser = commandUser ?? ctx.User;
            
                await using var db = new DataContext();
            
                var userFromDb = await new DbUser(db, findUser.Id, ctx).GetUser();
            
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                sendingEmbed = new DiscordEmbedBuilder
                {
                    Title = "Произошла какая-то ебучая ошибка",
                    Description = "Сообщи пж дауну, который криво написал этого бота",
                    Color = new DiscordColor(ErrorColor)
                };
            }
        }
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .AddEmbed(sendingEmbed));
    }

    [SlashCommand("top", "Посмотреть топ юзеров по балансу")]
    public async Task Top(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        
        try
        {
            var sw = new Stopwatch();
            
            sw.Start();
            
            var db = new DataContext();
        
            var topUsers = await db.Users
                .Where(gay => gay.Balance != 0 
                                    && gay.DiscordId != ctx.Client.CurrentUser.Id)
                .OrderByDescending(o => o.Balance).ToListAsync();
            
            var topUsersCopy = topUsers.ToList();
            
            foreach (var user in topUsers)
            {
                try
                {
                    if (await ctx.Client.GetUserAsync(user.DiscordId, true) == null) 
                        topUsersCopy.Remove(user);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    topUsersCopy.Remove(user);
                }
            }
            
            var pages = new List<Page>();

            for (var i = 0; i <= (topUsersCopy.Count / 8); i++)
            {
                var page = new Page();

                var pageEmbed = new DiscordEmbedBuilder
                {
                    Title = "Топ пользователей по балансу",
                    Color = new DiscordColor(NeutralColor)
                };
                for (var j = 0; j < 8; j++)
                {
                    var currentPosition = i * 8 + j;
                    try
                    {
                        var userFromDb = topUsersCopy[currentPosition];
                        var userFromDiscord = await ctx.Client.GetUserAsync(userFromDb.DiscordId);
                        var balance = userFromDb.Balance;
                        var rat = new Rat(balance);
                        pageEmbed.AddField($"{currentPosition + 1}. {userFromDiscord.Username}",
                            $"{balance} {rat.Word}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                
                page.Embed = pageEmbed;
                page.Content = "fck you";
                pages.Add(page);
            }
            
            sw.Stop();
            Console.WriteLine($"На выполнение команды ушло {sw.Elapsed}");

            await ctx.Interaction.SendPaginatedResponseAsync(pages: pages, asEditResponse: true, ephemeral: false,
                user: ctx.User);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            var errorEmbed = new DiscordEmbedBuilder
            {
                Title = "Произошла какая-то ебучая ошибка",
                Description = "Сообщи пж дауну, который криво написал этого бота",
                Color = new DiscordColor(ErrorColor)
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(errorEmbed));
        }
    }


    [SlashCommand("test", "kal")]
    public async Task Test(InteractionContext ctx)
    {
        var sw = new Stopwatch();
        sw.Start();
        await ctx.Client.GetUserAsync(310911827997622272);
        sw.Stop();
        await ctx.CreateResponseAsync($"На получение юзера затрачено: {sw.ElapsedMilliseconds / 1000f} секунд");
    }
}