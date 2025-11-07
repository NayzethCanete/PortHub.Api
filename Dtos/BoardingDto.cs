using System.ComponentModel.DataAnnotations;

namespace PortHub.Api.Dtos
{
    public record RequestBoardingDto(
        [Required] int FlightId, 
        [Required] string PassengerName,
        [Required] string Seat,
        [Required] string TicketNumber 
    );

    public record ResponseBoardingDto(
        int Id,
        int FlightId,
        string PassengerName,
        string Seat,
        string Status
    );

     public record BoardingRegistrationRequest(
        string TicketNumber,
        int GateId,
        string FlightCode
    );

    public record BoardingRegistrationResponse(
        bool Success,
        string Message,
        DateTime AccessTime,
        string Gate,
        string FlightCode
    );
}