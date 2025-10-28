using PortHub.Api.Models;

namespace PortHub.Api.Interface
{
    public interface ITicketService
    {
        List<Ticket> GetAll();
        Ticket? GetById(int id);
        Ticket? Update(Ticket ticket, int id);
        bool Delete(int id);
        Ticket Add(Ticket ticket);

         // Nuevo método: verificar validez del ticket
        bool ValidateTicket(int id);
    }
}