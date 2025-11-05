using PortHub.Api.Models;

namespace PortHub.Api.Interface
{
    public interface ISlotService
    {
        IEnumerable<Slot> GetAll();
        Slot? GetById(int id);
        Slot Add(Slot slot);
        Slot? Update(Slot slot, int id);
        bool Delete(int id);
        
        // Métodos para integración con aerolínea
        Slot ReserveSlot(Slot slot);
        Slot ConfirmSlot(int id);
        Slot CancelSlot(int id);
    }
}
