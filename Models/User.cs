using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace kalCasino.Models;

[Index(nameof(DiscordId), IsUnique = true)]
public class User
{
    [Key]
    public int Key { get; set; }
    public ulong DiscordId { get; init; }

    public long Balance { get; set; } = 0;
    public Timely? Timely { get; set; }
    public Steal? Steal { get; set; }

    public List<Twink>? Twinks { get; set; } 
    public List<History>? Histories { get; set; } 
    public List<VoiceConfig>? VoiceConfigs { get; set; }
}

[Owned]
public class Timely
{
    public DateTime? LatestDate { get; set; }
    public long? LatestValue { get; set; }
    public DateTime? EndDate { get; set; }
}

[Owned]
public class Steal
{
    public DateTime? LatestDate { get; set; }
}

public class Twink
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong DiscordId { get; set; }

    public int UserKey { get; set; }
    [ForeignKey("UserKey")]
    public User User { get; set; }
}

public class History
{
    [Key]
    public int Key { get; set; }
    public DateTime ChangeDate { get; set; }
    public long ValueChanged { get; set; } = 0;
    public ulong? WhoChangedId { get; set; }
    public string? Reason { get; set; }
    
    public int UserKey { get; set; }
    [ForeignKey("UserKey")]
    public User User { get; set; }
}

public class VoiceConfig
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string ParameterName { get; set; }
    public string Value { get; set; }

    public int UserKey { get; set; }
    [ForeignKey("UserKey")]
    public User User { get; set; }
}
