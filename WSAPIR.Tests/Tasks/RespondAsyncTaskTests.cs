using System.Net.WebSockets;
using Microsoft.Extensions.Logging;
using Moq;
using WSAPIR.Models;
using WSAPIR.Tasks;

namespace WSAPIR.Tests.Tasks
{
    public class RespondAsyncTaskTests
    {
        private readonly Mock<ILogger<RespondAsyncTask>> _mockLogger;
        private readonly RespondAsyncTask _task;

        public RespondAsyncTaskTests()
        {
            _mockLogger = new Mock<ILogger<RespondAsyncTask>>();
            _task = new RespondAsyncTask(_mockLogger.Object);
        }

        [Fact]
        public async Task RunTask_Should_LogError_On_WebSocketException()
        {
            var mockWebSocket = new Mock<WebSocket>();

            mockWebSocket
                .Setup(ws => ws.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new WebSocketException("Test WebSocketException"));

            var wws = new WrappedWebSocket
            {
                WebSocket = mockWebSocket.Object,
                UserId = 123
            };

            var request = new WebSocketRequest
            {
                ApiName = "TestApi",
                Endpoint = "/test",
                Method = "GET",
                Data = "Test Data"
            };

            var exception = await Assert.ThrowsAsync<WebSocketException>(() => _task.RunTask(wws, request, CancellationToken.None));

            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("Error sending response")),
                    It.Is<Exception>(ex => ex == exception),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task RunTask_Should_Send_Response_And_LogInformation()
        {
            var mockWebSocket = new Mock<WebSocket>();
            var sendAsyncCalled = false;

            mockWebSocket
                .Setup(ws => ws.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .Callback(() => sendAsyncCalled = true)
                .Returns(Task.CompletedTask);

            var wws = new WrappedWebSocket
            {
                WebSocket = mockWebSocket.Object,
                UserId = 123
            };

            var request = new WebSocketRequest
            {
                ApiName = "TestApi",
                Endpoint = "/test",
                Method = "GET",
                Data = "Test Data"
            };

            await _task.RunTask(wws, request, CancellationToken.None);

            Assert.True(sendAsyncCalled);

            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("Response sent to connection")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
