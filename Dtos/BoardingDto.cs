using System.ComponentModel.DataAnnotations;

namespace PortHub.Api.Dtos
{

    public record ResponseBoardingDto(
        int Id,
        string FlightCode,
        string PassengerName,
        string Seat,
        string Status
    );

    public record BoardingRegistrationRequest(
        [Required] string TicketNumber,
        [Required] string FlightCode
    );

    public record BoardingRegistrationResponse(
        bool Success,
        string Message,
        DateTime AccessTime,
        string Gate,
        string FlightCode
    );
}