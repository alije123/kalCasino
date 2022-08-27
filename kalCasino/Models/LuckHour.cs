using System.ComponentModel.DataAnnotations;

namespace kalCasino.Models;

public class LuckHour
{
    [Key]
    public DateTime Time { get; set; }
    public int Multiplier { get; set; }
}