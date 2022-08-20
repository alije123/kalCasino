using kalCasino.Models;
using Microsoft.EntityFrameworkCore;

namespace kalCasino.Database;

public class DbUser
{
    private DataContext Context { get; set; }
    private ulong Id { get; set; }

    public DbUser(DataContext context, in ulong id)
    {
        Context = context;
        Id = id;
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
            Console.WriteLine(e);
            Context.Users.Add(userFromDb);
            await Context.SaveChangesAsync();
        }

        return userFromDb;
    }
}