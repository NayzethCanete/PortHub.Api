namespace PortHub.Api.Dtos;

public record RequestTicketDto(int Flight_id, string Passenger_name, string Seat, string Status);
public record ResponseTicketDto(int Flight_id, string Passenger_name, string Seat, string Status);