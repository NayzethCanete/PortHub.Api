namespace PortHub.Api.Dtos;


//Para la solicitud de login

public record LoginDto(
    tring Username,
    string Password);
public record LoginResponseDto(
    string Token,
    string Username);
public record CreateTokenDto(
    string Username,
    int Id);

