using System.Net.WebSockets;
using Newtonsoft.Json;
using WSAPIR.Models;

namespace WSAPIR.Main
{
    /// <summary>
    /// Interface for handling WebSocket connections and requests.
    /// </summary>
    public interface IWebSocketHandler
    {
        /// <summary>
        /// Handles the WebSocket connection and processes incoming messages.
        /// </summary>
        /// <param name="wws">The WrappedWebSocket representing the connection.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleWebSocketAsync(WrappedWebSocket wws);

        /// <summary>
        /// Validates a WebSocket request asynchronously.
        /// </summary>
        /// <param name="request">The WebSocketRequest to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains true if the request is valid; otherwise, false.</returns>
        Task<bool> IsValidRequestAsync(WebSocketRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Handles invalid requests by sending an error message.
        /// </summary>
        /// <param name="wws">The WrappedWebSocket representing the connection.</param>
        /// <param name="errorMessage">The error message to send.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleInvalidRequestAsync(WrappedWebSocket wws, string errorMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Handles exceptions that occur during WebSocket operations.
        /// </summary>
        /// <param name="wws">The WrappedWebSocket representing the connection.</param>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleExceptionAsync(WrappedWebSocket wws, Exception ex, CancellationToken cancellationToken);

        /// <summary>
        /// Handles WebSocket exceptions.
        /// </summary>
        /// <param name="wws">The WrappedWebSocket representing the connection.</param>
        /// <param name="ex">The WebSocket exception.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleWebSocketExceptionAsync(WrappedWebSocket wws, WebSocketException ex, CancellationToken cancellationToken);

        /// <summary>
        /// Handles JSON exceptions asynchronously.
        /// </summary>
        /// <param name="wws">The WrappedWebSocket representing the connection.</param>
        /// <param name="ex">The JSON exception.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleJsonExceptionAsync(WrappedWebSocket wws, JsonException ex, CancellationToken cancellationToken);

        /// <summary>
        /// Closes the WebSocket connection.
        /// </summary>
        /// <param name="wws">The WrappedWebSocket representing the connection.</param>
        /// <param name="statusDescription">The status description for the closure.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CloseWebSocketAsync(WrappedWebSocket wws, string statusDescription, CancellationToken cancellationToken);
    }
}
