using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortHub.Api.Models
{
    public class Boarding
    {
        [Key]
        public int BoardingId { get; set; }
        
        [Required]
        public int TicketId { get; set; }
        
        public int SlotId { get; set; }
        public DateTime AccessTime { get; set; }
        public bool Validation { get; set; } = false;

        [ForeignKey("TicketId")]
        public virtual Ticket? Ticket { get; set; }
        
        [ForeignKey("SlotId")]
        public virtual Slot? Slot { get; set; }
    }
}