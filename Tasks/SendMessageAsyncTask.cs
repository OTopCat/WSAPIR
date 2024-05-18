using WSAPIR.Interfaces;
using WSAPIR.Models;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace WSAPIR.Main
{
    /// <summary>
    /// Task to send a WebSocket message to a specific connection.
    /// </summary>
    public class SendMessageAsyncTask : IWebSocketTask
    {
        private readonly ILogger<SendMessageAsyncTask> _logger;

        public SendMessageAsyncTask(ILogger<SendMessageAsyncTask> logger)
        {
            _logger = logger;
        }

        public string TaskName => nameof(SendMessageAsyncTask);

        /// <summary>
        /// Sends a WebSocket message to the specified connection.
        /// </summary>
        /// <param name="wws">The wrapped WebSocket connection.</param>
        /// <param name="request">The WebSocket request containing the response data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunTask(WrappedWebSocket wws, WebSocketRequest request, CancellationToken cancellationToken)
        {
            var response = JsonConvert.DeserializeObject<WebSocketResponse>(request.Data);

            try
            {
                await wws.WebSocket.SendAsync(response.ToBuffer(), WebSocketMessageType.Text, true, cancellationToken);
                _logger.LogInformation("SendMessageAsyncTask: Message sent to connection {ConnectionId}.", wws.UserId);
            }
            catch (WebSocketException ex)
            {
                _logger.LogError(ex, "SendMessageAsyncTask: Error sending message to connection {ConnectionId}.", wws.UserId);
            }
        }

        /// <summary>
        /// Placeholder for Interface to dynamically add tasks.
        /// </summary>
        public Task RunTask(WrappedWebSocket wws, string data, CancellationToken cancellationToken)
        {
            // Placeholder for Interface to dynamically add tasks
            throw new NotImplementedException();
        }
    }
}
