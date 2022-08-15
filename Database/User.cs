using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace kalCasino.Database;

[Index(nameof(DiscordId), IsUnique = true)]
public class User
{
    [Key]
    public int Key { get; set; }
    [Precision(10)]
    public long DiscordId { get; set; }
    public long Balance { get; set; }
    public DateTime? LatestTimelyDate { get; set; } 
    public long? LatestTimelyValue { get; set; }
    public DateTime? TimelyEnd { get; set; } 
    public DateTime? LatestSteal { get; set; }
 
    public ICollection<Twink>? Twinks { get; set; } 
    public ICollection<History>? Histories { get; set; } 
    public ICollection<VoiceConfig>? VoiceConfigs { get; set; }
}

public class Twink
{
    [Precision(10)]
    public long DiscordId { get; set; }
    
    public User User { get; set; } = null!;
}

public class History
{
    public DateTime ChangeDate { get; set; }
    public long ValueChanged { get; set; } = 0;
    public long? WhoChangedId { get; set; }
    public string? Reason { get; set; }
}

public class VoiceConfig
{
    public string ParameterName { get; set; } = null!;
    public string Value { get; set; } = null!;

    public User User { get; } = null!;
}
