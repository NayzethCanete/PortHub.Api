namespace PortHub.Api.Models;

public class Boarding
{
    public int BoardingId { get; set; }  
    public int TicketId { get; set; }
    public DateTime AccessTime { get; set; }
    public bool Validation { get; set; }  
    
    // Relación con Slot (agregada)
    public int SlotId { get; set; }
    public Slot Slot { get; set; }
}