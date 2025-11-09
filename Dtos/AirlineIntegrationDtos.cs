using System.Text.Json.Serialization;


/*

Se usa cuando se comunica con la API externa de la aerolínea
Representa el formato esperado por la API externa para validar un ticket


*/



namespace PortHub.Api.Dtos
{
    public class ExternalTicketValidationRequest
    {

        //Lo mapea en formato JSON esperado por la API externa

        [JsonPropertyName("NumeroTicket")]
        public object NumeroTicket { get; set; }

        [JsonPropertyName("FlightCode")]
        public string FlightCode { get; set; }

        public ExternalTicketValidationRequest(string ticketNumber, string flightCode)
        {

            //Intenta parsear el ticket como número, si no es posible lo deja como string, para evitar problemas de tipo en la API externa
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

    //Se usa dentro del sistema PortHub para representar solicitudes y respuestas de validación de tickets


    public record TicketValidationRequest(
        string TicketNumber,
        string FlightCode);

    //es la rta que devuelve el back a quien lo solicito, luego de solicitar verificacion 
    public record TicketValidationResponse(
        bool IsValid,
        string Message, //Mensaje descriptivo, ej. ticket valido o no valido
        TicketDetails? Details = null //Info opcional del pasajero si el ticket es valido
    );


    //Solo se usa si la validacion es existosa 

    public record TicketDetails(
        string PassengerName,
        string Seat);
}