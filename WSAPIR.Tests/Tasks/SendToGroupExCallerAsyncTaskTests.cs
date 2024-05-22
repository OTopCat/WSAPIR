using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using WSAPIR.Interfaces;
using WSAPIR.Models;
using System.Net.WebSockets;
using WSAPIR.Tasks;

namespace WSAPIR.Tests.Tasks
{
    public class SendToGroupExCallerAsyncTaskTests
    {
        private readonly Mock<IConnectionManager> _mockConnectionManager;
        private readonly Mock<IWebSocketTaskFactory> _mockWebSocketTaskFactory;
        private readonly Mock<ILogger<SendToGroupExCallerAsyncTask>> _mockLogger;
        private readonly SendToGroupExCallerAsyncTask _task;

        public SendToGroupExCallerAsyncTaskTests()
        {
            _mockConnectionManager = new Mock<IConnectionManager>();
            _mockWebSocketTaskFactory = new Mock<IWebSocketTaskFactory>();
            _mockLogger = new Mock<ILogger<SendToGroupExCallerAsyncTask>>();
            _task = new SendToGroupExCallerAsyncTask(_mockConnectionManager.Object, _mockWebSocketTaskFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task RunTask_Should_Send_Response_To_All_Connections_Excluding_Caller()
        {
            var wws = new WrappedWebSocket { WebSocket = new Mock<WebSocket>().Object, UserId = 123 };
            var request = new WebSocketRequest { Data = JsonConvert.SerializeObject(new WebSocketResponse { TaskName = "TestTask", Data = "Response data" }) };

            var connections = new List<WrappedWebSocket>
            {
                new WrappedWebSocket { WebSocket = new Mock<WebSocket>().Object, UserId = 124 },
                new WrappedWebSocket { WebSocket = new Mock<WebSocket>().Object, UserId = 125 }
            };

            _mockConnectionManager.Setup(m => m.GetGroupId(wws)).Returns(1);
            _mockConnectionManager.Setup(m => m.GetConnections(1)).Returns(connections);
            var sendMessageTaskMock = new Mock<IWebSocketTask>();
            _mockWebSocketTaskFactory.Setup(f => f.GetTask(nameof(SendMessageAsyncTask))).Returns(sendMessageTaskMock.Object);

            await _task.RunTask(wws, request, CancellationToken.None);

            sendMessageTaskMock.Verify(m => m.RunTask(It.IsAny<WrappedWebSocket>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SendToGroupExCallerAsyncTask: Response sent to connections in group")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task RunTask_Should_LogError_On_Exception()
        {
            var wws = new WrappedWebSocket { WebSocket = new Mock<WebSocket>().Object, UserId = 123 };
            var request = new WebSocketRequest { Data = "{Invalid JSON Data}" };

            _mockConnectionManager.Setup(m => m.GetGroupId(wws)).Returns(1);

            await Assert.ThrowsAsync<JsonReaderException>(() => _task.RunTask(wws, request, CancellationToken.None));

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error sending response to group")),
                    It.IsAny<JsonReaderException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once
            );
        }
    }
}
