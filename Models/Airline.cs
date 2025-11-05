using System; 
namespace PortHub.Api.Models;
public class Airline
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Country { get; set; }
    public string BaseAddress { get; set; }
    public ICollection<Flight> Flights { get; set; }

}