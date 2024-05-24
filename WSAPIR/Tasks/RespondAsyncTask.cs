using WSAPIR.Interfaces;
using WSAPIR.Models;
using System.Net.WebSockets;
using System.Text;

namespace WSAPIR.Tasks
{
    /// <summary>
    /// Task to send a response to a specific WebSocket connection.
    /// </summary>
    public class RespondAsyncTask : IWebSocketTask
    {
        private readonly ILogger<RespondAsyncTask> _logger;

        public RespondAsyncTask(ILogger<RespondAsyncTask> logger)
        {
            _logger = logger;
        }

        public string TaskName => nameof(RespondAsyncTask);

        /// <summary>
        /// Sends a response to the specified WebSocket connection.
        /// </summary>
        /// <param name="wws">The wrapped WebSocket connection.</param>
        /// <param name="request">The WebSocket request containing the response data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunTask(WrappedWebSocket wws, WebSocketRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Data))
            {
                _logger.LogError("Response data is null or empty for connection {ConnectionId}.", wws.UserId);
                return;
            }

            var responseMessage = Encoding.UTF8.GetBytes(request.Data);
            var buffer = new ArraySegment<byte>(responseMessage);

            try
            {
                await wws.WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
                _logger.LogInformation("Response sent to connection {ConnectionId}.", wws.UserId);
            }
            catch (WebSocketException ex)
            {
                _logger.LogError(ex, "Error sending response to connection {ConnectionId}.", wws.UserId);
                throw;
            }
        }

        /// <summary>
        /// Placeholder for Interface to dynamically add tasks.
        /// </summary>
        public Task RunTask(WrappedWebSocket wws, string? data, CancellationToken cancellationToken)
        {
            // Placeholder for Interface to dynamically add tasks
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a response to the specified WebSocket connection.
        /// </summary>
        /// <param name="wws">The wrapped WebSocket connection.</param>
        /// <param name="response">The WebSocket response containing the response data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunTask(WrappedWebSocket wws, WebSocketResponse response, CancellationToken cancellationToken)
        {
            var responseMessage = Encoding.UTF8.GetBytes(response.Data);
            var buffer = new ArraySegment<byte>(responseMessage);

            try
            {
                await wws.WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
                _logger.LogInformation("Response sent to connection {ConnectionId}.", wws.UserId);
            }
            catch (WebSocketException ex)
            {
                _logger.LogError(ex, "Error sending response to connection {ConnectionId}.", wws.UserId);
                throw;
            }
        }
    }
}
