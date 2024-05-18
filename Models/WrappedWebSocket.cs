using System.Net.WebSockets;

namespace WSAPIR.Models
{
    public class WrappedWebSocket
    {
        public required WebSocket WebSocket { get; set; }
        public string? Module { get; set; }
        public int UserId { get; set; }
    }
}

