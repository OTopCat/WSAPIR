using Moq;
using Newtonsoft.Json;
using System.Net.WebSockets;
using WSAPIR.Models;
using Microsoft.Extensions.Logging;
using WSAPIR.Tasks;

namespace WSAPIR.Tests.Tasks
{
    public class SendMessageAsyncTaskTests
    {
        private readonly Mock<ILogger<SendMessageAsyncTask>> _mockLogger;
        private readonly SendMessageAsyncTask _task;

        public SendMessageAsyncTaskTests()
        {
            _mockLogger = new Mock<ILogger<SendMessageAsyncTask>>();
            _task = new SendMessageAsyncTask(_mockLogger.Object);
        }

        [Fact]
        public async Task RunTask_Should_Send_Message_And_LogInformation()
        {
            var mockWebSocket = new Mock<WebSocket>();
            var wws = new WrappedWebSocket { WebSocket = mockWebSocket.Object, UserId = 123 };
            var response = new WebSocketResponse { TaskName = "TestTask", Data = "Response data" };
            var request = new WebSocketRequest
            {
                ApiName = "TestApi",
                Endpoint = "/test",
                Method = "POST",
                Data = JsonConvert.SerializeObject(response)
            };

            await _task.RunTask(wws, request, CancellationToken.None);

            mockWebSocket.Verify(ws => ws.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SendMessageAsyncTask: Message sent to connection")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task RunTask_Should_LogError_On_WebSocketException()
        {
            var mockWebSocket = new Mock<WebSocket>();
            mockWebSocket
                .Setup(ws => ws.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new WebSocketException());

            var wws = new WrappedWebSocket { WebSocket = mockWebSocket.Object, UserId = 123 };
            var response = new WebSocketResponse { TaskName = "TestTask", Data = "Response data" };
            var request = new WebSocketRequest
            {
                ApiName = "TestApi",
                Endpoint = "/test",
                Method = "POST",
                Data = JsonConvert.SerializeObject(response)
            };

            await _task.RunTask(wws, request, CancellationToken.None);

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SendMessageAsyncTask: Error sending message to connection")),
                    It.IsAny<WebSocketException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
