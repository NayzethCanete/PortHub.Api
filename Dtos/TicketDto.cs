namespace PortHub.Api.Dtos;

public record RequestTicketDto(int FlightId, string PassengerName, string Seat, string Status);
public record ResponseTicketDto(int Id,int FlightId, string PassengerName, string Seat, string Status);

// DTO para enviar la solicitud de validación
public record TicketValidationRequest(int TicketNumber, string FlightDate);
// DTO para recibir la respuesta de validación
public record TicketValidationResponse(bool IsValid, string Message);