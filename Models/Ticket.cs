using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortHub.Api.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FlightId { get; set; }
        
        public string? PassengerName { get; set; }         public string? Seat { get; set; }
        
        [Required]
        public string Status { get; set; }

        // Relaciones
        [ForeignKey("FlightId")]
        public virtual Flight? Flight { get; set; }
        
        public virtual Boarding? Boarding { get; set; }
    }
}