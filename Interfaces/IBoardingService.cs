using PortHub.Api.Models;

<<<<<<< HEAD
namespace PortHub.Api.Interface
=======
namespace PortHub.Api.Interfaces;

public interface IBoardingService
>>>>>>> BD-setup
{
    List<Boarding> GetAll();
    Boarding GetById(int id);
    List<Boarding> GetBySlotId(int slotId);
    Boarding Add(Boarding boarding);
    Boarding Update(Boarding boarding, int id);
    bool Delete(int id);
}