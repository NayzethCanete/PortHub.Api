using PortHub.Api.Models;
namespace PortHub.Api.Interfaces
{
    public interface IUserService
    {
        List<User> GetAll();
        
        User? GetById(int Id);

        User? UpdateUser(User user, int Id);

        bool DeleteUser(int Id);

        User AddUser(User user);

        User? GetByUsername(string Username);

    }
}