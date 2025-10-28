namespace PortHub.Api.Dtos;

// Datos recibidos al crear o modificar un slot
public record RequestSlotDto(
    DateTime Date,
    int Runway,
    int Gate_id,
    string Status,
    int Flight_id
);

// Datos devueltos al cliente
public record ResponseSlotDto(
    int? Id,
    DateTime Date,
    int Runway,
    int Gate_id,
    string Status,
    int Flight_id
);
