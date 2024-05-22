using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using WSAPIR.Interfaces;
using WSAPIR.Models;
using WSAPIR.Tasks;

namespace WSAPIR.Tests.Tasks
{
    public class SendToGroupAsyncTaskTests
    {
        private readonly Mock<IConnectionManager> _mockConnectionManager;
        private readonly Mock<IWebSocketTaskFactory> _mockWebSocketTaskFactory;
        private readonly Mock<ILogger<SendToGroupAsyncTask>> _mockLogger;
        private readonly Mock<IWebSocketTask> _mockSendMessageTask;
        private readonly SendToGroupAsyncTask _task;

        public SendToGroupAsyncTaskTests()
        {
            _mockConnectionManager = new Mock<IConnectionManager>();
            _mockWebSocketTaskFactory = new Mock<IWebSocketTaskFactory>();
            _mockLogger = new Mock<ILogger<SendToGroupAsyncTask>>();
            _mockSendMessageTask = new Mock<IWebSocketTask>();

            _task = new SendToGroupAsyncTask(
                _mockConnectionManager.Object,
                _mockWebSocketTaskFactory.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task RunTask_Should_Send_Response_To_All_Connections_In_Group()
        {
            var groupId = 1;
            var response = new WebSocketResponse { TaskName = "TestTask", Data = "TestData" };
            var request = new WebSocketRequest { Data = JsonConvert.SerializeObject(response) };
            var wrappedWebSocket = new WrappedWebSocket { UserId = 123, WebSocket = new Mock<System.Net.WebSockets.WebSocket>().Object };

            var connections = new List<WrappedWebSocket>
            {
                new WrappedWebSocket { UserId = 124, WebSocket = new Mock<System.Net.WebSockets.WebSocket>().Object },
                new WrappedWebSocket { UserId = 125, WebSocket = new Mock<System.Net.WebSockets.WebSocket>().Object }
            };

            _mockConnectionManager.Setup(m => m.GetGroupId(It.IsAny<WrappedWebSocket>())).Returns(groupId);
            _mockConnectionManager.Setup(m => m.GetConnections(groupId)).Returns(connections);
            _mockWebSocketTaskFactory.Setup(f => f.GetTask(nameof(SendMessageAsyncTask))).Returns(_mockSendMessageTask.Object);

            await _task.RunTask(wrappedWebSocket, request, CancellationToken.None);

            _mockSendMessageTask.Verify(m => m.RunTask(It.IsAny<WrappedWebSocket>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(connections.Count));
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SendToGroupAsyncTask: Response sent to connections in group")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>())
            );
        }

        [Fact]
        public async Task RunTask_Should_LogError_On_Exception()
        {
            var groupId = 1;
            var request = new WebSocketRequest { Data = "{invalidJson}" };
            var wrappedWebSocket = new WrappedWebSocket { UserId = 123, WebSocket = new Mock<System.Net.WebSockets.WebSocket>().Object };

            _mockConnectionManager.Setup(m => m.GetGroupId(It.IsAny<WrappedWebSocket>())).Returns(groupId);
            _mockWebSocketTaskFactory.Setup(f => f.GetTask(nameof(SendMessageAsyncTask))).Returns(_mockSendMessageTask.Object);

            await Assert.ThrowsAsync<JsonReaderException>(() => _task.RunTask(wrappedWebSocket, request, CancellationToken.None));

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
