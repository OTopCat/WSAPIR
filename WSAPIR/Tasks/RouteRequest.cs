using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WSAPIR.Interfaces;
using WSAPIR.Models;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;

namespace WSAPIR.Tasks
{
    /// <summary>
    /// Task to route an API request from WebSocket data.
    /// </summary>
    public class RouteRequest : IWebSocketTask
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConnectionManager _connectionManager;
        private readonly IWebSocketTaskFactory _webSocketTaskFactory;
        private readonly ILogger<RouteRequest> _logger;
        private readonly ApiUrlsSettings _apiUrlsSettings;

        public RouteRequest(
            IHttpClientFactory httpClientFactory,
            IConnectionManager connectionManager,
            IWebSocketTaskFactory webSocketTaskFactory,
            ILogger<RouteRequest> logger,
            IOptions<ApiUrlsSettings> apiUrlsSettings)
        {
            _httpClientFactory = httpClientFactory;
            _connectionManager = connectionManager;
            _webSocketTaskFactory = webSocketTaskFactory;
            _logger = logger;
            _apiUrlsSettings = apiUrlsSettings.Value;
        }

        public string TaskName => nameof(RouteRequest);

        /// <summary>
        /// Routes an API request from the provided WebSocket request data.
        /// </summary>
        /// <param name="wws">The wrapped WebSocket connection.</param>
        /// <param name="request">The WebSocket request containing the API request data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunTask(WrappedWebSocket wws, WebSocketRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var apiRequest = JsonConvert.DeserializeObject<ApiRequest>(request.Data);
                if (apiRequest == null)
                {
                    throw new ArgumentException("Invalid API request data.");
                }

                var client = _httpClientFactory.CreateClient();
                var apiUrl = new Uri(new Uri(_apiUrlsSettings.Urls[apiRequest.ApiName]), apiRequest.Endpoint);

                var requestMessage = new HttpRequestMessage(new HttpMethod(apiRequest.Method), apiUrl);

                if (!string.IsNullOrEmpty(apiRequest.Data))
                {
                    requestMessage.Content = new StringContent(apiRequest.Data, Encoding.UTF8, "application/json");
                }

                // Add the JWT token to the authorization header
                if (!string.IsNullOrEmpty(request.JWT))
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.JWT);
                }

                var response = await client.SendAsync(requestMessage, cancellationToken);
                var responseData = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogInformation("Request routed to {ApiName} with endpoint {Endpoint}", apiRequest.ApiName, apiRequest.Endpoint);

                if (!string.IsNullOrEmpty(apiRequest.CallbackTask))
                {
                    var callbackTask = _webSocketTaskFactory.GetTask(apiRequest.CallbackTask);
                    if (callbackTask != null)
                    {
                        await callbackTask.RunTask(wws, responseData, cancellationToken);
                    }
                    else
                    {
                        _logger.LogError("Callback task {CallbackTask} not found.", apiRequest.CallbackTask);
                    }
                }
                else
                {
                    var responseMessage = new WebSocketResponse
                    {
                        TaskName = TaskName,
                        Data = responseData
                    };

                    await wws.WebSocket.SendAsync(responseMessage.ToBuffer(), WebSocketMessageType.Text, true, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error routing API request.");
                await HandleErrorAsync(wws, "Error routing API request.", cancellationToken);
            }
        }


        /// <summary>
        /// Placeholder as need this construcrtor do dynamicaly add tasks
        /// </summary>
        /// <param name="wws">The wrapped WebSocket connection.</param>
        /// <param name="data">The data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Throws NotImplementedException.</returns>
        public Task RunTask(WrappedWebSocket wws, string? data, CancellationToken cancellationToken)
        {
            // This is a system task, so no need to implement call for it
            throw new NotImplementedException();
        }

        private async Task HandleErrorAsync(WrappedWebSocket wws, string errorMessage, CancellationToken cancellationToken)
        {
            var response = new WebSocketResponse
            {
                TaskName = nameof(RouteRequest),
                Data = errorMessage
            };

            await wws.WebSocket.SendAsync(response.ToBuffer(), WebSocketMessageType.Text, true, cancellationToken);
        }
    }
}
