using System.Diagnostics;
using Discord;
using Discord.Commands;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;
using kalCasino.Database;
using Microsoft.EntityFrameworkCore;

namespace kalCasino.Commands;

public class Plain : ModuleBase<SocketCommandContext>
{
    private const uint ErrorColor = 0xF64444;
    private const uint NeutralColor = 0xAC60F9;
    
    public InteractiveService Interactive { get; set; }

    /*
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
    */

    [Command("top")]
    [Summary("Посмотреть топ юзеров по балансу")]
    public async Task Top()
    {
        try
        {
            var db = new DataContext();

            var sw = new Stopwatch();
            
            sw.Start();
        
            var topUsers = await db.Users
                .Where(gay => gay.Balance != 0 
                                    && gay.DiscordId != Context.Client.CurrentUser.Id)
                .OrderByDescending(o => o.Balance).ToListAsync();
            
            sw.Stop();

            Console.WriteLine($"На хуёвый запрос с бд затрачено: {sw.ElapsedMilliseconds/1000f} секунд");
            
            sw.Restart();
            
            var topUsersCopy = topUsers.ToList();
            
            sw.Stop();
            Console.WriteLine($"На копирование листа затрачено: {sw.ElapsedMilliseconds/1000f} секунд");
            
            sw.Restart();

            var kw = new Stopwatch();
            foreach (var user in topUsers)
            {
                kw.Restart();
                if (await Context.Client.GetUserAsync(user.DiscordId) == null) 
                        topUsersCopy.Remove(user);
                kw.Stop();
                Console.WriteLine($"-- На получение юзера затрачено: {kw.ElapsedMilliseconds/1000f} секунд");
            }
            
            sw.Stop();
            
            Console.WriteLine($"На проверку на null затрачено: {sw.ElapsedMilliseconds/1000f} секунд");

            sw.Restart();

            var pages = new List<PageBuilder>();

            for (var i = 0; i <= (topUsersCopy.Count / 8); i++)
            {
                var page = new PageBuilder();

                page.Title = "Топ пользователей по балансу";
                page.Color = new Color(NeutralColor);
                    for (var j = 0; j < 8; j++)
                {
                    var currentPosition = i * 8 + j;
                    try
                    {
                        var userFromDb = topUsersCopy[currentPosition];
                        var userFromDiscord = await Context.Client.GetUserAsync(userFromDb.DiscordId);
                        var balance = userFromDb.Balance;
                        var rat = new Rat(balance);
                        page.AddField($"{currentPosition + 1}. {userFromDiscord.Username}",
                            $"{balance} {rat.Word}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Юзеров больше нет");
                    }
                }
                    
                pages.Add(page);
            }
            
            sw.Stop();
            
            Console.WriteLine($"На костыльный перебор юзеров и создание страниц затрачено: {sw.ElapsedMilliseconds/1000f} секунд");
            
            sw.Restart();

            // await ctx.Interaction.SendPaginatedResponseAsync(pages: pages, asEditResponse: true, ephemeral: false,
            //     user: ctx.User);
            var paginator = new StaticPaginatorBuilder()
                .WithPages(pages)
                .Build();
            
            sw.Stop();
            
            Console.WriteLine($"На билд страниц затрачено: {sw.ElapsedMilliseconds/1000f} секунд");

            await Interactive.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(10));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            var errorEmbed = new EmbedBuilder
            {
                Title = "Произошла какая-то ебучая ошибка",
                Description = "Сообщи пж дауну, который криво написал этого бота",
                Color = new Color(ErrorColor)
            }.Build();
            // await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(errorEmbed));
            await ReplyAsync(embed:errorEmbed);
        }
    }

    [Command("test")]
    public async Task Test()
    {
        var sw = new Stopwatch();
        sw.Start();

        await Context.Client.GetUserAsync(584140343558275109);
        
        sw.Stop();

        await ReplyAsync(message:$"На получение юзера вне контекста затрачено {sw.ElapsedMilliseconds/1000f}");
    }

}