/*using PortHub.Api.Models;
using PortHub.Api.Interfaces;
using PortHub.Api.Dtos;
using PortHub.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace PortHub.Api.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public TicketService(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public List<Ticket> GetAll()
        {
            return _context.Tickets.ToList();
        }

        public Ticket? GetById(int id)
        {
            return _context.Tickets.FirstOrDefault(t => t.Id == id);
        }

        public Ticket Add(Ticket ticket)
        {
            ticket.Status ??= "Emitido";
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return ticket;
        }

        public Ticket? Update(Ticket ticket, int id)
        {
            var existing = _context.Tickets.FirstOrDefault(t => t.Id == id);
            if (existing == null)
                return null;

            existing.FlightCode = ticket.FlightCode;
            existing.PassengerName = ticket.PassengerName;
            existing.Seat = ticket.Seat;
            existing.Status = ticket.Status;

            _context.SaveChanges();
            return existing;
        }

        public bool Delete(int id)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null) return false;

            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
            return true;
        }

        // VALIDACIÓN CON API DE AEROLÍNEA
        public async Task<TicketValidationResponse> ValidateTicketAsync(int ticketId, string flightDate, string airlineApiUrl)
        {
            try
            {
                var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);
                if (ticket == null)
                {
                    return new TicketValidationResponse(false, "Ticket no encontrado en el sistema");
                }

                var request = new TicketValidationRequest(ticketId, flightDate);
                var response = await _httpClient.PostAsJsonAsync($"{airlineApiUrl}/validate-ticket", request);

                if (response.IsSuccessStatusCode)
                {
                    var validationResponse = await response.Content.ReadFromJsonAsync<TicketValidationResponse>();
                    return validationResponse ?? new TicketValidationResponse(false, "Error al procesar respuesta");
                }

                return new TicketValidationResponse(false, $"Error al contactar aerolínea: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return new TicketValidationResponse(false, $"Error de conexión: {ex.Message}");
            }
        }
    }
}*/