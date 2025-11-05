<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;
=======
namespace PortHub.Api.Models;
>>>>>>> BD-setup

namespace PortHub.Api.Models;

public class Slot
{
<<<<<<< HEAD
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
=======
    public int Id { get; set; }
    public string FlightNumber { get; set; }
    public DateTime ScheduledTime { get; set; }
    public string Runway { get; set; }  
    public SlotStatus Status { get; set; }
    
 
    public int AirlineId { get; set; }
    public Airline Airline { get; set; }
    
    public int? GateId { get; set; }
    public Gate Gate { get; set; }
    
    public ICollection<Boarding> Boardings { get; set; } = new List<Boarding>();
>>>>>>> BD-setup
}