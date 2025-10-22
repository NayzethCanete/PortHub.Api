using System;
using System.ComponentModel.DataAnnotations;

namespace PortHubApi.Models;
public class Boarding
{
    public int BoardingId { get; set; }
    public int? TicketId { get; set; }
    public DateTime AccessTime { get; set; }
    public int? GateId { get; set; }
    public bool Validation { get; set; }
}
