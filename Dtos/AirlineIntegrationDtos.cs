using System.Text.Json.Serialization;

namespace PortHub.Api.Dtos
{
        public class ExternalTicketValidationRequest
    {
        [JsonPropertyName("NumeroTicket")]
        public object NumeroTicket { get; set; }

        [JsonPropertyName("FlightCode")]
        public string FlightCode { get; set; }

        public ExternalTicketValidationRequest(string ticketNumber, string flightCode)
        {
            if (long.TryParse(ticketNumber, out long number))
            {
                NumeroTicket = number;
            }
            else
            {
                NumeroTicket = ticketNumber;
            }
            FlightCode = flightCode;
        }
    }

public record TicketValidationRequest(string TicketNumber, string FlightCode);

    public record TicketValidationResponse(
        bool IsValid,
        string Message,
        TicketDetails? Details = null 
    );

    public record TicketDetails(string PassengerName, string Seat);
}