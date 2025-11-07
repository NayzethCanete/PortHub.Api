using PortHub.Api.Models;

namespace PortHub.Api.Interfaces
{
    public interface ISlotService
    {
        IEnumerable<Slot> GetAll();
        Slot? GetById(int id);
        Slot? Update(Slot slot, int id);
        bool Delete(int id);
        
        // Métodos para integración con aerolínea
        Slot ReserveSlot(Slot slot);
        Slot ConfirmSlot(int id, int airlineId);
        Slot CancelSlot(int id, int airlineId);
    }
}
