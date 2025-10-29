namespace PortHub.Api.Models;

public enum SlotStatus { RESERVED, CONFIRMED, RELEASED }

public class Slot
{
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
}