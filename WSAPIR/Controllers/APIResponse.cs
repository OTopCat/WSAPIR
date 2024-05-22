using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WSAPIR.Interfaces;
using WSAPIR.Models;

namespace WSAPIR.Controllers
{
    /// <summary>
    /// API controller for handling responses.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ApiResponseController : ControllerBase
    {
        private readonly ApiUrlsSettings _apiUrlsSettings;
        private readonly IWebSocketTaskFactory _webSocketTaskFactory;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<ApiResponseController> _logger;

        public ApiResponseController(
            IOptions<ApiUrlsSettings> apiUrlsSettings,
            IWebSocketTaskFactory webSocketTaskFactory,
            IConnectionManager connectionManager,
            ILogger<ApiResponseController> logger)
        {
            _apiUrlsSettings = apiUrlsSettings.Value;
            _webSocketTaskFactory = webSocketTaskFactory;
            _connectionManager = connectionManager;
            _logger = logger;
        }

        /// <summary>
        /// Handles the API response.
        /// </summary>
        /// <param name="request">The request body containing the API response details.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> HandleResponse([FromBody] ApiResponse request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request body.");
            }

            if (!_apiUrlsSettings.Urls.ContainsValue(request.Sender))
            {
                _logger.LogWarning("Sender {Sender} is not authorized.", request.Sender.Replace(Environment.NewLine, ""));
                return Unauthorized("Sender not authorized.");
            }

            _logger.LogInformation("Processing response from {Sender}.", request.Sender.Replace(Environment.NewLine, ""));

            var task = _webSocketTaskFactory.GetTask(request.TaskName);
            if (task == null)
            {
                _logger.LogError("Task {TaskName} not found.", request.TaskName.Replace(Environment.NewLine, ""));
                return BadRequest("Task not found.");
            }

            var connections = _connectionManager.GetConnections(request.GroupId);
            var wrappedRequest = new WebSocketResponse
            {
                TaskName = request.TaskName,
                Data = JsonConvert.SerializeObject(new { Data = request.Data, Sender = request.Sender })
            };

            var cancellationToken = new System.Threading.CancellationToken();
            var tasks = connections.Select(connection => task.RunTask(connection, wrappedRequest, cancellationToken)).ToList();
            await Task.WhenAll(tasks);

            return Ok("Task executed successfully.");
        }
    }
}
