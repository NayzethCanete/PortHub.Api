using PortHub.Api.Models;

namespace PortHubApi.Interface
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