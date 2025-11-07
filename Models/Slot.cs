using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using PortHub.Api.Models;

namespace PortHub.Api.Models
{
    public class Slot
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        
        [Required] 
        public DateTime ScheduleTime { get; set; }
        
        [Required]
        public string Runway { get; set; }
        
        public int? GateId { get; set; }
        
        public string? FlightCode { get; set; }
        
        [Required] 
        public string? Status { get; set; } = "Reservado";

    
        /// Fecha/hora en que expira la reserva temporal (solo para estado "Reservado")
        public DateTime? ReservationExpiresAt { get; set; }

        public int? AirlineId { get; set; }

        [ForeignKey("AirlineId")]
        public Airline? Airline { get; set; }
        // Relaciones
        public Gate? Gate { get; set; }
        public ICollection<Boarding> Boardings { get; set; } = new List<Boarding>();
    }
}