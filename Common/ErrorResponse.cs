namespace PortHub.Api.Common
{
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
