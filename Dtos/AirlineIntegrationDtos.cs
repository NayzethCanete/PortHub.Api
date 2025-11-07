namespace PortHub.Api.Dtos
{
    public record TicketValidationRequest(
        string TicketNumber,
        string FlightCode,
        DateTime FlightDate // ✅ campo agregado y con tipo correcto
    );

    public record TicketValidationResponse(
        bool IsValid,
        string Message,
        TicketDetails? Details = null
    );

    public record TicketDetails(
        string PassengerName,
        string FlightCode,
        string Seat,
        DateTime DepartureTime
    );
}
