using WSAPIR.Interfaces;
using WSAPIR.Models;
using Newtonsoft.Json;

namespace WSAPIR.Tasks
{
    /// <summary>
    /// Task to send a response to all connections in the caller's group where the specified property value matches the caller's.
    /// </summary>
    public class SendToGroupExCallerMatchingPropertyAsyncTask : IWebSocketTask
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IWebSocketTaskFactory _webSocketTaskFactory;
        private readonly ILogger<SendToGroupExCallerMatchingPropertyAsyncTask> _logger;

        public SendToGroupExCallerMatchingPropertyAsyncTask(IConnectionManager connectionManager, IWebSocketTaskFactory webSocketTaskFactory, ILogger<SendToGroupExCallerMatchingPropertyAsyncTask> logger)
        {
            _connectionManager = connectionManager;
            _webSocketTaskFactory = webSocketTaskFactory;
            _logger = logger;
        }

        public string TaskName => nameof(SendToGroupExCallerMatchingPropertyAsyncTask);

        /// <summary>
        /// Sends a response to all connections in the caller's group where the specified property value matches the caller's.
        /// </summary>
        /// <param name="wws">The wrapped WebSocket connection.</param>
        /// <param name="request">The WebSocket request containing the response data and the property name to match.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunTask(WrappedWebSocket wws, WebSocketRequest request, CancellationToken cancellationToken)
        {
            int groupId = _connectionManager.GetGroupId(wws);
            var propertyData = JsonConvert.DeserializeObject<(string PropertyName, WebSocketResponse Response)>(request.Data);

            var connections = _connectionManager.GetConnections(groupId);
            var callerConnection = connections.FirstOrDefault(connection => connection.WebSocket == wws.WebSocket);
            if (callerConnection == null)
            {
                return;
            }

            var callerPropertyValue = _connectionManager.GetPropertyValue(callerConnection, propertyData.PropertyName);
            if (callerPropertyValue == null)
            {
                return;
            }

            var sendMessageTask = _webSocketTaskFactory.GetTask(nameof(SendMessageAsyncTask));
            var tasks = connections
                .Where(connection => connection.WebSocket != wws.WebSocket)
                .Select(connection => new { connection, connectionPropertyValue = _connectionManager.GetPropertyValue(connection, propertyData.PropertyName) })
                .Where(x => x.connectionPropertyValue != null && callerPropertyValue.Equals(x.connectionPropertyValue))
                .Select(x => sendMessageTask.RunTask(x.connection, JsonConvert.SerializeObject(propertyData.Response), cancellationToken)).ToList();

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Placeholder for Interface to dynamically add tasks.
        /// </summary>
        public Task RunTask(WrappedWebSocket wws, string? data, CancellationToken cancellationToken)
        {
            // Placeholder for Interface to dynamically add tasks
            throw new NotImplementedException();
        }
    }
}
