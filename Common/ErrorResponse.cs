namespace PortHub.Api.Common
{
    public class ErrorResponse
    {
        public string Code { get; set; } = default!;
        public string Message { get; set; } = default!;
        public object? Details { get; set; }
    }
}