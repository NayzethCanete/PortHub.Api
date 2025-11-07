
using System.ComponentModel.DataAnnotations;
namespace PortHub.Api.Dtos;

public record BoardingRequestDto(
    string TicketNumber,
    int SlotId,
    long GateId,
    int FlightCode
);

public record BoardingResponseDto(
    long Id,
    string TicketNumber,
    long GateId,
    DateTime AccessTime,
    bool Validated
);

public record BoardingValidateRequestDto(
    string TicketNumber,
    int SlotId
);

public class RequestBoardingDto
{
    //[Required]
    public string? TicketNumber { get; set; }

//[Required]
    public string? FlightCode { get; set; }

//    [Required]
    public int GateId { get; set; }
}
    
