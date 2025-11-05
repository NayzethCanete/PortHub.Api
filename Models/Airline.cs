namespace PortHub.Api.Models;

public class Airline
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Country { get; set; }
    public string BaseAddress { get; set; }
<<<<<<< HEAD
    public ICollection<Flight> Flights { get; set; }

=======
    public string ApiKey { get; set; }
    public string ApiUrl { get; set; }
    
    // Navegación
    public ICollection<Slot> Slots { get; set; } = new List<Slot>();
>>>>>>> BD-setup
}