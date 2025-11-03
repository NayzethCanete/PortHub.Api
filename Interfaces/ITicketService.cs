using PortHub.Api.Models;
using PortHub.Api.Dtos;

namespace PortHub.Api.Interface
{
    public interface ITicketService
    {
        List<Ticket> GetAll();
        Ticket? GetById(int id);
        Ticket? Update(Ticket ticket, int id);
        bool Delete(int id);
        Ticket Add(Ticket ticket);

        // Método: Verificar validez del ticket, con Dto.
        TicketValidationResponse ValidateTicket(TicketValidationRequest request);
    }
}