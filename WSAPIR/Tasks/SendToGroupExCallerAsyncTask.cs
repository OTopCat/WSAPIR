﻿using WSAPIR.Interfaces;
using WSAPIR.Models;
using Newtonsoft.Json;

namespace WSAPIR.Tasks
{
    /// <summary>
    /// Task to send a response to all connections in the caller's group, excluding the caller.
    /// </summary>
    public class SendToGroupExCallerAsyncTask : IWebSocketTask
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IWebSocketTaskFactory _webSocketTaskFactory;
        private readonly ILogger<SendToGroupExCallerAsyncTask> _logger;

        public SendToGroupExCallerAsyncTask(IConnectionManager connectionManager, IWebSocketTaskFactory webSocketTaskFactory, ILogger<SendToGroupExCallerAsyncTask> logger)
        {
            _connectionManager = connectionManager;
            _webSocketTaskFactory = webSocketTaskFactory;
            _logger = logger;
        }

        public string TaskName => nameof(SendToGroupExCallerAsyncTask);

        /// <summary>
        /// Sends a response to all connections in the caller's group, excluding the caller.
        /// </summary>
        /// <param name="wws">The wrapped WebSocket connection.</param>
        /// <param name="request">The WebSocket request containing the response data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunTask(WrappedWebSocket wws, WebSocketRequest request, CancellationToken cancellationToken)
        {
            int groupId = _connectionManager.GetGroupId(wws);
            try
            {
                if (string.IsNullOrEmpty(request.Data))
                {
                    _logger.LogError("SendToGroupExCallerAsyncTask: Request data is null or empty.");
                    return;
                }

                var response = JsonConvert.DeserializeObject<WebSocketResponse>(request.Data);

                var connections = _connectionManager.GetConnections(groupId)
                    .Where(connection => connection.WebSocket != wws.WebSocket);
                var sendMessageTask = _webSocketTaskFactory.GetTask(nameof(SendMessageAsyncTask));

                var tasks = connections.Select(connection => sendMessageTask.RunTask(connection, JsonConvert.SerializeObject(response), cancellationToken)).ToList();
                await Task.WhenAll(tasks);

                _logger.LogInformation("SendToGroupExCallerAsyncTask: Response sent to connections in group {GroupId} except caller.", groupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending response to group {GroupId} except caller.", groupId);
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
        /// Sends a response to all connections in the caller's group, excluding the caller.
        /// </summary>
        /// <param name="wws">The wrapped WebSocket connection.</param>
        /// <param name="response">The WebSocket response containing the response data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunTask(WrappedWebSocket wws, WebSocketResponse response, CancellationToken cancellationToken)
        {
            int groupId = _connectionManager.GetGroupId(wws);
            try
            {
                var connections = _connectionManager.GetConnections(groupId)
                    .Where(connection => connection.WebSocket != wws.WebSocket);
                var sendMessageTask = _webSocketTaskFactory.GetTask(nameof(SendMessageAsyncTask));

                var tasks = connections.Select(connection => sendMessageTask.RunTask(connection, response, cancellationToken)).ToList();
                await Task.WhenAll(tasks);

                _logger.LogInformation("SendToGroupExCallerAsyncTask: Response sent to connections in group {GroupId} except caller.", groupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending response to group {GroupId} except caller.", groupId);
                throw;
            }
        }
    }
}
