using System.Net.WebSockets;

namespace WSAPIR.Models
{
    /// <summary>
    /// instance of websocket with some extra keys for dictionary to store connections
    /// </summary>
    public class WrappedWebSocket
    {
        /// <summary>
        /// websocet obj
        /// </summary>
        public required WebSocket WebSocket { get; set; }

        /// <summary>
        /// module in order to track users in multi API env.
        /// </summary>
        public string? Module { get; set; }

        /// <summary>
        /// User ID from claim
        /// </summary>
        public int UserId { get; set; }
    }
}

