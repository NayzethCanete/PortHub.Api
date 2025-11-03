using System;
using System.ComponentModel.DataAnnotations;

namespace PortHub.Api.Models;
public class Boarding
{
    public int BoardingId { get; set; }
    public int TicketId { get; set; }
    public int GateId { get; set; }
    public DateTime AccessTime { get; set; }
    public bool Validation { get; set; }
}
