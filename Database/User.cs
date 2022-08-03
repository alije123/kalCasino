using DSharpPlus.SlashCommands;

namespace kalCasino.Database;

public class User
{
    public User(InteractionContext ctx, ulong? userId = null)
    {
        UserId = userId ?? ctx.User.Id;
    }

    private ulong UserId { get; }
    public long Balance { get; private set; }

    private async Task Exists()
    {
        var exists = await Db.GetValueBool(@$"SELECT EXISTS (SELECT balance FROM userbalances WHERE id = {UserId})");
        if (!exists)
        {
            await Db.Do($@"INSERT INTO polniykal.userbalances(id) VALUES({UserId})");
        }
    }

    public async Task GetBalance()
    {
        await Exists();
        Balance = await Db.GetValueLong(@$"SELECT balance FROM userbalances WHERE id = {UserId}");
    }

    public async Task EditBalance(long editValue, Operation operation)
    {
        await Exists();
        switch (operation)
        {
            case Operation.Add:
                await Db.Do($@"UPDATE userbalances SET balance = balance + {editValue} WHERE id={UserId}");
                break;
            case Operation.Subtract:
                await Db.Do($@"UPDATE userbalances SET balance = balance - {editValue} WHERE id={UserId}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation, "Надо было указать Operation.Add, либо Subtract");
        }
        
        GetBalance();
    }

    public enum Operation
    {
        Add,
        Subtract
    }
}