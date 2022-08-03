using DSharpPlus.SlashCommands;

namespace kalCasino;

public class Logger
{
    private DateTime Time { get; } = new DateTime();

    public void CommandСall(InteractionContext ctx)
    {
        WriteDate();
        WriteColorMessage($"{ctx.User.Id.ToString()} ({ctx.User.Username})", 
            ConsoleColor.DarkBlue);
        Console.Write(": Команда ");
        WriteColorMessage($"/{ctx.CommandName}", ConsoleColor.DarkGreen);
        Console.WriteLine(" получена");
    }
    
    public void Other(string id, string message)
    {
        WriteDate();
        WriteColorMessage(id, 
            ConsoleColor.DarkBlue);
        Console.WriteLine(": " + message);
    }
    private static void WriteDate()
    {
        WriteColorMessage($"({DateTime.Now}) ", 
            ConsoleColor.DarkGray);
    }

    private static void WriteColorMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }
}