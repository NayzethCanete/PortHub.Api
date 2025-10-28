namespace PortHub.Api.Dtos;

public record RequestSlotDto(int? id, DateTime Date, int Runway, int Gate_id, string Status, int Flight_id);
public record ResponseSlotDto(int? id, DateTime Date, int Runway, int Gate_id, string Status, int Flight_id);