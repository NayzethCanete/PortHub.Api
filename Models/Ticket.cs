using System; 
namespace PortHub.Api.Models;
public class Ticket
{
    public int Id { get; set; }
    public string TicketNumber { get; set; }
    public int FlightId { get; set; }
    public Flight Flight { get; set; }
    public string PassengerName { get; set; }
    public string Seat { get; set; }
    public string Status { get; set; }
}
