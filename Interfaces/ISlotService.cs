using PortHub.Api.Models;

namespace PortHub.Api.Interface
{
    public interface ISlotService
    {
        List<Slot> GetAll();
        Slot? GetById(int id);
        Slot? Update(Slot slot, int id);
        bool Delete(int id);
        Slot Add(Slot slot);
        Slot ReserveForAirline(Slot slot);
        Slot ConfirmForAirline(int id);
        Slot CancelForAirline(int id);
    }
}