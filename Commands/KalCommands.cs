using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using kalCasino.Database;

namespace kalCasino.Commands;

public class KalCommands : ApplicationCommandModule
{
    private const string ErrorColor = "F64444";
    private const string NeutralColor = "AC60F9";

    [SlashCommand("ping", "Жив ли бот?", true)]
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

    [SlashCommand("balance", "Сколько у тебя на балансе?", true)]
    public async Task Balance(InteractionContext ctx, 
        [Option("user", "Ты можешь выбрать другого человека")] DiscordUser? user = null)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        
        var log = new Logger();
        log.CommandСall(ctx);
        
        var sendingEmbed = new DiscordEmbedBuilder();

        if (user != null && user.IsBot && user != ctx.Client.CurrentUser)
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
            var kaloed = new User(ctx ,user?.Id);
            long balance = kaloed.Balance;
        
            Rat rat = new Rat(balance);
        
            sendingEmbed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = user?.AvatarUrl ?? ctx.User.AvatarUrl,
                    Name = user?.Username ?? ctx.User.Username
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
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(sendingEmbed));
    }

    [SlashCommand("give", "Дать реткоины", false)]
    [SlashRequirePermissions(Permissions.BanMembers)]
    public async Task Add(InteractionContext ctx,[Option("value", "Выбери сколько ты хочешь выдать")] long giveValue, 
        [Option("user", "Ты можешь выбрать другого человека")] DiscordUser? user = null)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        
        var log = new Logger();
        log.CommandСall(ctx);

        var sendingEmbed = new DiscordEmbedBuilder();

        if (user != null && user.IsBot && user != ctx.Client.CurrentUser)
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
        else if (user == ctx.Client.CurrentUser)
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
            var kaloed = new User(ctx, user?.Id);
            await kaloed.EditBalance(giveValue, User.Operation.Add);

            var bot = new User(ctx, ctx.Client.CurrentUser.Id);
            await bot.EditBalance(giveValue, User.Operation.Subtract);

            long balance = kaloed.Balance;
            Rat rat = new Rat(balance);

            Rat kal = new Rat(giveValue);
            
            sendingEmbed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = user?.AvatarUrl ?? ctx.User.AvatarUrl,
                    Name = user?.Username ?? ctx.User.Username
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
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(sendingEmbed));
    }

    [SlashCommand("take", "Забрать себе реткоины", false)]
    [SlashRequirePermissions(Permissions.Administrator)]
    public async Task Subtract(InteractionContext ctx, [Option("value", "Выбери сколько ты хочешь забрать")] long takeValue,
        [Option("user", "Выбери у кого ты хочешь забрать")] DiscordUser user)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var log = new Logger();
        log.CommandСall(ctx);

        var sendingEmbed = new DiscordEmbedBuilder();

        if (user.IsBot && user != ctx.Client.CurrentUser)
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
        else if (user == ctx.Client.CurrentUser)
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
        else
        {
            var kaloed = new User(ctx, user.Id);

            var admin = new User(ctx, ctx.User.Id);
            
            if (kaloed.Balance < takeValue)
            {
                sendingEmbed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = user.Username,
                        IconUrl = user.AvatarUrl
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
                await kaloed.EditBalance(takeValue, User.Operation.Subtract);
                await admin.EditBalance(takeValue, User.Operation.Add);

                Rat takeRat = new Rat(takeValue);
                Rat resultRat = new Rat(kaloed.Balance);

                sendingEmbed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = user.Username,
                        IconUrl = user.AvatarUrl
                    },
                    Title = $"{ctx.User.Username} забрать себе {takeValue} {takeRat.Word}",
                    Description = $"Баланс теперь {kaloed.Balance} {resultRat.Word}",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = @"https://cdn.discordapp.com/attachments/1002188468174196756/1003133521918951476/unknown.png"
                    },
                    Color = new DiscordColor(NeutralColor)
                };
            }
        }
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(sendingEmbed));
    }
    
    [SlashCommand("return", "Забрать реткоины", false)]
    [SlashRequirePermissions(Permissions.BanMembers)]
    public async Task Return(InteractionContext ctx, [Option("value", "Выбери сколько ты хочешь забрать")] long returnValue,
        [Option("user", "Выбери у кого ты хочешь забрать")] DiscordUser user)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var log = new Logger();
        log.CommandСall(ctx);

        var sendingEmbed = new DiscordEmbedBuilder();

        if (user.IsBot && user != ctx.Client.CurrentUser)
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
        else if (user == ctx.Client.CurrentUser)
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
        else
        {
            var kaloed = new User(ctx, user.Id);

            var bot = new User(ctx, ctx.Client.CurrentUser.Id);
            
            if (kaloed.Balance < returnValue)
            {
                sendingEmbed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = user.Username,
                        IconUrl = user.AvatarUrl
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
                await kaloed.EditBalance(returnValue, User.Operation.Subtract);
                await bot.EditBalance(returnValue, User.Operation.Add);
                
                var takeRat = new Rat(returnValue);
                var resultRat = new Rat(kaloed.Balance);

                sendingEmbed = new DiscordEmbedBuilder
                {
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = user.Username,
                        IconUrl = user.AvatarUrl
                    },
                    Title = $"Партия забрать себе {returnValue} {takeRat.Word}",
                    Description = $"Баланс теперь {kaloed.Balance} {resultRat.Word}",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = @"https://cdn.discordapp.com/attachments/1002188468174196756/1003205839693283379/unknown.png"
                    },
                    Color = new DiscordColor(NeutralColor)
                };
            }
        }
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(sendingEmbed));
    }











    /*[Command ("add")]
    [Description("Складывает числа")]
    [RequireRoles(RoleCheckMode.All, 855342615267246110)]
    public async Task Add(CommandContext ctx, [Description("Первое число")] int numberOne,
        [Description("Второе число")] int numberTwo)
    {
        await ctx.Channel
            .SendMessageAsync((numberOne + numberTwo)
                .ToString()).ConfigureAwait(false);
    }
    
    [Command("respondmessage")]
    public async Task RespondMessage(CommandContext ctx)
    {
         var interactivity = ctx.Client.GetInteractivity();

         var message = await interactivity.WaitForMessageAsync(x 
             => x.Channel == ctx.Channel)
             .ConfigureAwait(false);

         await ctx.Channel.SendMessageAsync(message.Result.Content);
    }
    
    [Command("respondreaction")]
    public async Task RespondEmoji(CommandContext ctx)
    {
        var interactivity = ctx.Client.GetInteractivity();

        var message = await interactivity.WaitForReactionAsync(x 
                => x.Channel == ctx.Channel)
            .ConfigureAwait(false);

        await ctx.Channel.SendMessageAsync(message.Result.Emoji);
    }

    [Command("poll")]
    public async Task Poll(CommandContext ctx, TimeSpan duration, params DiscordEmoji[] emojiOptions)
    {
        var interactivity = ctx.Client.GetInteractivity();
        var options = emojiOptions.Select(x => x.ToString());

        var pollEmbed = new DiscordEmbedBuilder
        {
            Title = "Poll",
            Description = string.Join(" ", options)
        };

        var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

        foreach (var option in emojiOptions)
        {
            await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
        }

        var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);
        var distinctResult = result.Distinct();
        
        var results = distinctResult.Select(x => $"{x.Emoji}: {x.Total}");
        
        await ctx.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);
    }*/
    
    
    
}