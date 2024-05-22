using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using WSAPIR.Interfaces;
using WSAPIR.Models;
using WSAPIR.Main;
using WSAPIR.Tasks;

namespace WSAPIR.Tests.Tasks
{
    public class RouteRequestTests
    {
        private readonly Mock<ILogger<RouteRequest>> _mockLogger;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<IConnectionManager> _mockConnectionManager;
        private readonly Mock<IWebSocketTaskFactory> _mockTaskFactory;
        private readonly RouteRequest _task;

        public RouteRequestTests()
        {
            _mockLogger = new Mock<ILogger<RouteRequest>>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockConnectionManager = new Mock<IConnectionManager>();
            _mockTaskFactory = new Mock<IWebSocketTaskFactory>();
            var apiUrlsSettings = Options.Create(new ApiUrlsSettings
            {
                Urls = new Dictionary<string, string> { { "TestApi", "http://testapi.com/" } }
            });
            _task = new RouteRequest(_mockHttpClientFactory.Object, _mockConnectionManager.Object, _mockTaskFactory.Object, _mockLogger.Object, apiUrlsSettings);
        }

        [Fact]
        public async Task RunTask_Should_Route_Request_And_Send_Response()
        {
            var wws = new WrappedWebSocket
            {
                WebSocket = new Mock<WebSocket>().Object,
                UserId = 123
            };

            var request = new WebSocketRequest
            {
                Data = JsonConvert.SerializeObject(new ApiRequest
                {
                    ApiName = "TestApi",
                    Endpoint = "/test",
                    Method = "GET"
                })
            };

            var httpClientMock = new Mock<HttpClient>();
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent("response data")
            };

            var clientHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            clientHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(new HttpClient(clientHandlerMock.Object));

            await _task.RunTask(wws, request, CancellationToken.None);

            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Request routed to TestApi with endpoint /test")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }
    }
}
