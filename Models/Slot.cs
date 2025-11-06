using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema; 

namespace PortHub.Api.Models
{
    public class Slot
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        
        [Required] 
        public DateTime ScheduleTime { get; set; }
        
        public string Runway { get; set; }
        public int? GateId { get; set; }
        public string? FlightCode { get; set; }
        
        [Required] 
        public string? Status { get; set; } = "Reservado";

        // Relaciones (Combinado)
    //    [ForeignKey("Flight_id")]
//        public Flight? Flight { get; set; }
        
     //   [ForeignKey("Gate_id")]
        public Gate? Gate { get; set; }
        
        public ICollection<Boarding> Boardings { get; set; } = new List<Boarding>();
    }
}