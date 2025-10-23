using System; 
namespace PortHub.Api.Models;

public enum SlotStatus { RESERVED, CONFIRMED, RELEASED }

public class Slot
{
    public int Id { get; set; }
    public DateTime ScheduledTime { get; set; }
    public int Runway { get; set; } // pista
    public int GateId { get; set; }
    public Gate Gate { get; set; }
    public SlotStatus Status { get; set; } //Libre, reservado, confirmado
    public int FlightId { get; set; }
    public Flight Flight { get; set; }
}
