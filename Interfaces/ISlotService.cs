using PortHubApi.Models;

namespace PortHubApi.Interface
{
    public interface ISlotService
    {
        List<Slot> GetAll();
    }
}