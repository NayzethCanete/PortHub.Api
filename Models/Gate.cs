namespace PortHub.Api.Models;

public class Gate
{
    public int GateId { get; set; }  
    public string GateName { get; set; }  
    public string Location { get; set; }  
    public bool Available { get; set; } = true; 
    
    public ICollection<Slot> Slots { get; set; } = new List<Slot>();
}