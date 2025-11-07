namespace PortHub.Api.Dtos;

public record RequestSlotDto(
    DateTime Date,
    string Runway,
    int? Gate_id,
    string? Status,
    string FlightCode
);

public record ResponseSlotDto(
    int? Id,
    DateTime Date,
    string? Runway,
    int GateId,
    string Status,
    string FlightCode,
    DateTime? ReservationExpiresAt 
);