using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using WSAPIR.Interfaces;
using WSAPIR.Models;

namespace WSAPIR.Main
{
    /// <summary>
    /// Handles WebSocket connections and processes incoming requests.
    /// </summary>
    public class WebSocketHandler : IWebSocketHandler
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IWebSocketTaskFactory _webSocketTaskFactory;
        private readonly ILogger<WebSocketHandler> _logger;
        private readonly IWebSocketAuth _webSocketAuth;

        public WebSocketHandler(
            IConnectionManager connectionManager,
            IWebSocketTaskFactory webSocketTaskFactory,
            ILogger<WebSocketHandler> logger,
            IWebSocketAuth webSocketAuth)
        {
            _connectionManager = connectionManager;
            _webSocketTaskFactory = webSocketTaskFactory;
            _logger = logger;
            _webSocketAuth = webSocketAuth;
        }

        /// <inheritdoc />
        public async Task HandleWebSocketAsync(WrappedWebSocket wws)
        {
            var cancellationToken = new CancellationTokenSource().Token;

            try
            {
                while (wws.WebSocket.State == WebSocketState.Open)
                {
                    var buffer = new ArraySegment<byte>(new byte[4096]);
                    var result = await wws.WebSocket.ReceiveAsync(buffer, cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        if (buffer.Array != null)
                        {
                            var message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                            var request = JsonConvert.DeserializeObject<WebSocketRequest>(message);

                            if (request != null)
                            {
                                if (await IsValidRequestAsync(request, cancellationToken))
                                {
                                    var task = _webSocketTaskFactory.GetTask(request.TaskName);
                                    if (task != null)
                                    {
                                        await task.RunTask(wws, request, cancellationToken);
                                    }
                                    else
                                    {
                                        _logger.LogError("Invalid Task in request: {TaskName}", request.TaskName);
                                        await HandleInvalidRequestAsync(wws, "Invalid Task in request", cancellationToken);
                                    }
                                }
                                else
                                {
                                    _logger.LogError("Invalid Request received: {Request}", request);
                                    await CloseWebSocketAsync(wws, "Invalid Request - Disconnected", cancellationToken);
                                }
                            }
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await CloseWebSocketAsync(wws, "Closed by the client", cancellationToken);
                    }
                }
            }
            catch (WebSocketException ex)
            {
                await HandleWebSocketExceptionAsync(wws, ex, cancellationToken);
            }
            catch (JsonException ex)
            {
                await HandleJsonExceptionAsync(wws, ex, cancellationToken);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(wws, ex, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task<bool> IsValidRequestAsync(WebSocketRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.JWT) || string.IsNullOrEmpty(request.TaskName))
            {
                return false;
            }

            try
            {
                return await _webSocketAuth.IsAuthenticatedAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during request validation.");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task HandleInvalidRequestAsync(WrappedWebSocket wws, string errorMessage, CancellationToken cancellationToken)
        {
            var response = new WebSocketResponse
            {
                TaskName = nameof(HandleInvalidRequestAsync),
                Data = errorMessage
            };

            await wws.WebSocket.SendAsync(response.ToBuffer(), WebSocketMessageType.Text, true, cancellationToken);
        }

        /// <inheritdoc />
        public async Task HandleExceptionAsync(WrappedWebSocket wws, Exception ex, CancellationToken cancellationToken)
        {
            if (wws.WebSocket.State == WebSocketState.Open)
            {
                await HandleInvalidRequestAsync(wws, "Internal server error.", cancellationToken);
            }
            else
            {
                _logger.LogError(ex, "WebSocket errored - removing connection from group {GroupId}", _connectionManager.GetGroupId(wws));
                _connectionManager.RemoveFromGroup(wws);
            }
            _logger.LogError(ex, "Internal server error.");
        }

        /// <inheritdoc />
        public async Task HandleWebSocketExceptionAsync(WrappedWebSocket wws, WebSocketException ex, CancellationToken cancellationToken)
        {
            if (wws.WebSocket.State == WebSocketState.Open)
            {
                await HandleInvalidRequestAsync(wws, "WebSocket error.", cancellationToken);
            }
            else
            {
                _logger.LogError(ex, "WebSocket errored - removing connection from group {GroupId}", _connectionManager.GetGroupId(wws));
                _connectionManager.RemoveFromGroup(wws);
            }
        }

        /// <inheritdoc />
        public async Task HandleJsonExceptionAsync(WrappedWebSocket wws, JsonException ex, CancellationToken cancellationToken)
        {
            if (wws.WebSocket.State == WebSocketState.Open)
            {
                await HandleInvalidRequestAsync(wws, "Invalid WebSocket Request format.", cancellationToken);
            }
            else
            {
                _logger.LogError(ex, "WebSocket errored - removing connection from group {GroupId}", _connectionManager.GetGroupId(wws));
                _connectionManager.RemoveFromGroup(wws);
            }
            _logger.LogError(ex, "Invalid WebSocket Request format received.");
        }

        /// <inheritdoc />
        public Task CloseWebSocketAsync(WrappedWebSocket wws, string statusDescription, CancellationToken cancellationToken)
        {
            return wws.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, statusDescription, cancellationToken)
                .ContinueWith(t => _connectionManager.RemoveFromGroup(wws));
        }
    }
}
