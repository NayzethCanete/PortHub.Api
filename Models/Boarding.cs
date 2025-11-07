using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortHub.Api.Models
{
    public class Boarding
    {
        [Key]
        public int BoardingId { get; set; }

        [Required]
        public string TicketNumber { get; set; } = null!;

        [Required]
        public string PassengerName { get; set; } = null!;

        public string Seat { get; set; } = "SIN ASIGNAR";

        public string Status { get; set; } = "Embarcado";

        public DateTime BoardingTime { get; set; } = DateTime.UtcNow;

        [ForeignKey("Gate")]
        public int GateId { get; set; }
        public Gate? Gate { get; set; }

        [ForeignKey("Slot")]
        public int SlotId { get; set; }
        public Slot? Slot { get; set; }

        [Required]
        public string FlightCode { get; set; } = null!;
    }
}
