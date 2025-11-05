using PortHub.Api.Models;

namespace PortHub.Api.Interfaces 
{
    public interface IBoardingService
    {
        List<Boarding> GetAll();
        Boarding GetById(int id);
        List<Boarding> GetBySlotId(int slotId);
        Boarding Add(Boarding boarding);
        Boarding Update(Boarding boarding, int id);
        bool Delete(int id);
    }
}