namespace PortHub.Api.Dtos;

public record LoginDto(string Username, string Password);
public record LoginResponseDto(string Token, string Username);
public record CreateTokenDto(string Username, int Id);

