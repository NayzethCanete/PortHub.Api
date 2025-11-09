namespace PortHub.Api.Dtos;




/*

DTO para solicitudes de creación o actualización de aerolíneas.

*/

//Son los datos que el cliente envia al servidor
public record AirlineRequestDto(
    string Name,
    string Code,
    string Country,
    string BaseAddress,
    string ApiUrl
);

//Son los datos que el servidor envia al cliente
public record AirlineResponseDto(
    long Id,
    string Name,
    string Code,
    string? Country,
    string? BaseAddress,
    string? ApiUrl,
    string? ApiKey = null
);