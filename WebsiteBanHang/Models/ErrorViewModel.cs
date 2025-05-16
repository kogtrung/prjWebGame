using System;

namespace WebGame.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; } = string.Empty;

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string Message { get; set; } = "An error occurred while processing your request.";
        
        public int StatusCode { get; set; } = 500;
        
        public bool IsDbConnectionError { get; set; } = false;
    }
}
