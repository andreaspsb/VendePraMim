namespace VendePraMim.Api.Models
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Error { get; set; }
        public object? Details { get; set; }
    }
}
