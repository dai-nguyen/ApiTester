using Infrastructure.Enums;

namespace Infrastructure.Models
{
    public class HttpModel
    {
        public string Endpoint { get; set; }
        public HttpActions Action { get; set; }
        public string Body { get; set; }
        public string Response { get; set; }
    }
}