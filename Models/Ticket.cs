using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortHub.Api.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string FlightCode { get; set; }
        
        public string? PassengerName { get; set; }        
        public string? Seat { get; set; }
        
        [Required]
        public string Status { get; set; } = "Emitido";

        // Relaciones

        //public virtual Boarding? Boarding { get; set; }
    }
}