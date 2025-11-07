using PortHub.Api.Models;

namespace PortHub.Api.Interfaces
{
    public interface IBoardingService
    {
        IEnumerable<Boarding> GetAll();
        Boarding? GetById(int id);
        Boarding Add(Boarding boarding);
        Boarding? Update(Boarding boarding, int id);
        bool Delete(int id);
        Task<(Boarding? boarding, bool success, string message)> ValidateAndRegisterBoardingAsync(
        string ticketNumber, 
        int slotId); }
}