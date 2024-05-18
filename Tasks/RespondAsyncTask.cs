using WSAPIR.Interfaces;
using WSAPIR.Models;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace WSAPIR.Main
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
            try
            {
                var response = JsonConvert.DeserializeObject<WebSocketResponse>(request.Data);
                await wws.WebSocket.SendAsync(response.ToBuffer(), WebSocketMessageType.Text, true, cancellationToken);
                _logger.LogInformation("RespondAsyncTask: Response sent to user {UserId}.", wws.UserId);
            }
            catch (WebSocketException ex)
            {
                _logger.LogError(ex, "RespondAsyncTask: Error sending response to user {UserId}.", wws.UserId);
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
