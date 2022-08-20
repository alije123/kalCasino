using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kalCasino.Models;

public class Shop
{
    [Key]
    public int Key { get; set; }
    public string Name { get; set; }
    public long Price { get; set; } = 0;
    public string Type { get; set; }
    public long RoleId { get; set; }
}