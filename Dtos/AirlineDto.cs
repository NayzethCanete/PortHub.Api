namespace PortHub.Api.Dtos;

public record AirlineRequestDto(
    string Name, 
    string Code, 
    string Country, 
    string BaseAddress,
    string ApiUrl
);

public record AirlineResponseDto(
    long Id, 
    string Name, 
    string Code, 
    string Country, 
    string BaseAddress,
    string ApiUrl,
    string ApiKey = null
);