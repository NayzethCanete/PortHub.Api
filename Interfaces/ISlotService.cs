using PortHubApi.Models;

namespace PortHubApi.Interface
{
    public interface ISlotService
    {
        List<Slot> GetAll();
        Slot? GetById(int id);
        Slot? Update(Slot slot, int id);
        bool Delete(int id);
        Slot Add(Slot slot);
    }
}