using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortHub.Api.Models
{
    public class Flight
    {
        [Key]
        public int FlightId { get; set; }
        
        [Required]
        public string FlightCode { get; set; }
        
        [Required]
        public int AirlineId { get; set; } 
        
        [Required]
        public string Origin { get; set; }
        
        [Required]
        public string Destination { get; set; }
        
        [Required]
        public string Status { get; set; }
        
        public int? SlotId { get; set; } 
        
        [ForeignKey("AirlineId")]
        public virtual Airline? Airline { get; set; }
        
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        
        public virtual Slot? Slot { get; set; }
    }
}