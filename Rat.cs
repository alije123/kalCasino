namespace kalCasino;

public class Rat
{
    public Rat(long balance)
    {
        var n = Math.Abs(balance) % 100;
        
        if(n % 10 is 0 || n % 10 is >= 5 and <= 9 || n is > 9 and < 20) 
            Word = "реткоинов";
        else if (n % 10 == 1)
            Word = "реткоин";
        else
            Word = "реткоина";
    }

    public string Word { get; }
}