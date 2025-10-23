using System; 

namespace PortHub.Api.Dtos
{
    public record BoardingRequestDto(string TicketNumber, long GateId, int FlightId);
    public record BoardingResponseDto(long Id, string TicketNumber, long GateId, DateTime AccessTime, bool Validated);

}