using System.ComponentModel.DataAnnotations;


/*

Representa los datos que el sistema devuelve  y envia para el registro de abordajes (boardings).

*/



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
        [Required] int TicketNumber,
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