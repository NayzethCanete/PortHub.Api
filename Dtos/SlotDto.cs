namespace PortHub.Api.Dtos;


//Son los datos que el cliente envia al servidor para reservar un slot,
//es la estructua que usan las airlines 

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