using PortHub.Api.Models;
using PortHub.Api.Interface;

namespace PortHub.Api.Services
{
    public class TicketService : ITicketService
    {
        private static readonly List<Ticket> _tickets = new();
        private static int _nextId = 1;

        public List<Ticket> GetAll()
        {
            return _tickets;
        }

        public Ticket? GetById(int id)
        {
            return _tickets.FirstOrDefault(t => t.Id == id);
        }

        public Ticket Add(Ticket ticket)
        {
            ticket.Id = _nextId++;
            ticket.Status ??= "válido";
            _tickets.Add(ticket);
            return ticket;
        }

        public Ticket? Update(Ticket ticket, int id)
        {
            var existing = _tickets.FirstOrDefault(t => t.Id == id);
            if (existing == null)
                return null;

            existing.Flight_id = ticket.Flight_id;
            existing.Passenger_name = ticket.Passenger_name;
            existing.Seat = ticket.Seat;
            existing.Status = ticket.Status;
            return existing;
        }

        public bool Delete(int id)
        {
            var ticket = _tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null) return false;

            _tickets.Remove(ticket);
            return true;
        }
    }
}
