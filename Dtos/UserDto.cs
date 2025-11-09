namespace PortHub.Api.Dtos;

public record RegistrarUserDto(
    string Username,
    string PasswordHash);
public record ActualizarUserDto(
    string Username,
    string PasswordHash);
public record UserDto(
    string Username);