using System; 
namespace PortHub.Api.Dtos
{
    public record AirlineRequestDto(string Name, string Code, string Country, string BaseAddress);
    public record AirlineResponseDto(long Id, string Name, string Code, string Country, string BaseAddress);

}