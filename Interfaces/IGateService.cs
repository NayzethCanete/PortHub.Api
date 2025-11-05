using PortHub.Api.Models;

namespace PortHub.Api.Interfaces
{
    public interface IGateService
    {
        List<Gate> GetAll();
        Gate? GetById(int id);
        Gate? Update(Gate gate, int id);
        bool Delete(int id);
        Gate Add(Gate gate);
    }
}