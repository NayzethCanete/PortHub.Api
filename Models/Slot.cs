using System.ComponentModel.DataAnnotations;

namespace PortHubApi.Models;

public class Slot
{
    [Required]
    public int? Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int Runway { get; set; }

    [Required]
    public int Gate_id { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; }

    [Required]
    public int Flight_id { get; set; }
}
