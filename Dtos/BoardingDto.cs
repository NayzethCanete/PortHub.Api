namespace PortHub.Api.Dtos;

public record BoardingRequestDto(
    string TicketNumber,
    int SlotId,
    long GateId,
    int FlightCode
);

public record BoardingResponseDto(
    long Id, 
    string TicketNumber, 
    long GateId, 
    DateTime AccessTime, 
    bool Validated
);

public record BoardingValidateRequestDto(
    int TicketId,
    int SlotId
);