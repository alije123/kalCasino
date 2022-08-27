using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using kalCasino.Models;
using Microsoft.EntityFrameworkCore;

namespace kalCasino.Database;

public class DbUser
{
    private DataContext Context { get; set; }
    private ulong Id { get; set; }
    private InteractionContext Ctx { get; set; }

    public DbUser(DataContext context, in ulong id, InteractionContext type) 
    {
        Context = context;
        Id = id;
        Ctx = type;
    }

    public async Task<User> GetUser()
    {
        var userFromDb = new User
        {
            DiscordId = Id
        };
        try
        {
            userFromDb = await Context.Users.FirstAsync(p => p.DiscordId == Id);
        }
        catch (Exception e)
        {
            if (Ctx.User.Id == Id)
            {
                var kalEmbed = new DiscordEmbedBuilder
                {
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = "https://cdn.discordapp.com/attachments/1002188468174196756/1010589352600027207/chad.webp"
                    },
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = "Привет",
                    },
                    Title = $"{Ctx.User.Username}, Добро пожаловать в каловое казино",
                    Description = "В нём ты сможешь весело проводить время",
                    Color = new Optional<DiscordColor>(DiscordColor.Green)
                };
                await new DiscordMessageBuilder().WithEmbed(kalEmbed).SendAsync(Ctx.Channel);
            }
            Console.WriteLine(e);
            Context.Users.Add(userFromDb);
            await Context.SaveChangesAsync();
        }

        return userFromDb;
    }
}