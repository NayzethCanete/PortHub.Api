namespace PortHub.Api.Common
{

    /*
    Clase para representar respuestas de error en la API.
    Proporciona un formato consistente para los mensajes de error.
    */
    public class ErrorResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }

        public ErrorResponse(string code, string message, string? details = null)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        public ErrorResponse() { }
    }
}
