using PortHub.Api.Dtos;
namespace PortHub.Api.Interfaces;

public interface IAuthService
{
    string CreateToken(CreateTokenDto createTokenDto);
    string Login(LoginDto loginDto);
}