using WSAPIR.Interfaces;
using WSAPIR.Models;
using Newtonsoft.Json;

namespace WSAPIR.Tasks
{
    /// <summary>
    /// Task to send a response to all connections in a specified group.
    /// </summary>
    public class SendToGroupAsyncTask : IWebSocketTask
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IWebSocketTaskFactory _webSocketTaskFactory;
        private readonly ILogger<SendToGroupAsyncTask> _logger;

        public SendToGroupAsyncTask(IConnectionManager connectionManager, IWebSocketTaskFactory webSocketTaskFactory, ILogger<SendToGroupAsyncTask> logger)
        {
            _connectionManager = connectionManager;
            _webSocketTaskFactory = webSocketTaskFactory;
            _logger = logger;
        }

        public string TaskName => nameof(SendToGroupAsyncTask);

        /// <summary>
        /// Sends a response to all connections in a specified group.
        /// </summary>
        /// <param name="wws">The wrapped WebSocket connection.</param>
        /// <param name="request">The WebSocket request containing the response data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunTask(WrappedWebSocket wws, WebSocketRequest request, CancellationToken cancellationToken)
        {
            try
            {
                int groupId = _connectionManager.GetGroupId(wws);
                var response = JsonConvert.DeserializeObject<WebSocketResponse>(request.Data);

                var connections = _connectionManager.GetConnections(groupId);
                var sendMessageTask = _webSocketTaskFactory.GetTask(nameof(SendMessageAsyncTask));

                var tasks = connections.Select(connection => sendMessageTask.RunTask(connection, JsonConvert.SerializeObject(response), cancellationToken)).ToList();
                await Task.WhenAll(tasks);

                _logger.LogInformation("SendToGroupAsyncTask: Response sent to connections in group {GroupId}.", groupId);
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError(ex, "Error sending response to group");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                throw;
            }
        }


        /// <summary>
        /// Placeholder for Interface to dynamically add tasks.
        /// </summary>
        public Task RunTask(WrappedWebSocket wws, string? data, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
