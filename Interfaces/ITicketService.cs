using PortHubApi.Models;

namespace PortHubApi.Interface
{
    public interface ITicketService
    {
        List<Ticket> GetAll();
        Ticket? GetById(int id);
        Ticket? Update(Ticket ticket, int id);
        bool Delete(int id);
        Ticket Add(Ticket ticket);
    }
}