namespace PortHub.Api.Dtos;

// Datos recibidos al crear o modificar un slot
public record RequestSlotDto(
    DateTime Date,
    string Runway,
    int? Gate_id,
    string? Status,
    string FlightCode
);

// Datos devueltos al cliente
public record ResponseSlotDto(
    int? Id,
    DateTime Date,
    string? Runway,
    int GateId,
    string Status,
    string FlightCode
);
