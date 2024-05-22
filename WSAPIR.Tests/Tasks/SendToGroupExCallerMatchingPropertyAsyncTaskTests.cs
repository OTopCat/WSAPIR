using Moq;
using Newtonsoft.Json;
using WSAPIR.Interfaces;
using WSAPIR.Models;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using WSAPIR.Tasks;

namespace WSAPIR.Tests.Tasks
{
    public class SendToGroupExCallerMatchingPropertyAsyncTaskTests
    {
        private readonly Mock<IConnectionManager> _mockConnectionManager;
        private readonly Mock<IWebSocketTaskFactory> _mockWebSocketTaskFactory;
        private readonly Mock<ILogger<SendToGroupExCallerMatchingPropertyAsyncTask>> _mockLogger;
        private readonly SendToGroupExCallerMatchingPropertyAsyncTask _task;
        private readonly Mock<IWebSocketTask> _mockSendMessageTask;

        public SendToGroupExCallerMatchingPropertyAsyncTaskTests()
        {
            _mockConnectionManager = new Mock<IConnectionManager>();
            _mockWebSocketTaskFactory = new Mock<IWebSocketTaskFactory>();
            _mockLogger = new Mock<ILogger<SendToGroupExCallerMatchingPropertyAsyncTask>>();
            _mockSendMessageTask = new Mock<IWebSocketTask>();

            _mockWebSocketTaskFactory
                .Setup(f => f.GetTask(nameof(SendMessageAsyncTask)))
                .Returns(_mockSendMessageTask.Object);

            _task = new SendToGroupExCallerMatchingPropertyAsyncTask(
                _mockConnectionManager.Object,
                _mockWebSocketTaskFactory.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task RunTask_Should_Send_Response_To_Matching_Connections_Excluding_Caller()
        {
            var groupId = 1;
            var propertyName = "Role";
            var response = new WebSocketResponse { TaskName = "TestTask", Data = "Response data" };
            var request = new WebSocketRequest
            {
                ApiName = "TestApi",
                Endpoint = "/test",
                Method = "POST",
                Data = JsonConvert.SerializeObject((propertyName, response))
            };

            var callerWebSocket = new Mock<WebSocket>().Object;
            var matchingWebSocket = new Mock<WebSocket>().Object;
            var nonMatchingWebSocket = new Mock<WebSocket>().Object;

            var callerConnection = new WrappedWebSocket { WebSocket = callerWebSocket, UserId = 123 };
            var matchingConnection = new WrappedWebSocket { WebSocket = matchingWebSocket, UserId = 124 };
            var nonMatchingConnection = new WrappedWebSocket { WebSocket = nonMatchingWebSocket, UserId = 125 };

            var connections = new List<WrappedWebSocket> { callerConnection, matchingConnection, nonMatchingConnection };

            _mockConnectionManager.Setup(m => m.GetGroupId(It.IsAny<WrappedWebSocket>())).Returns(groupId);
            _mockConnectionManager.Setup(m => m.GetConnections(groupId)).Returns(connections);
            _mockConnectionManager.Setup(m => m.GetPropertyValue(callerConnection, propertyName)).Returns("Admin");
            _mockConnectionManager.Setup(m => m.GetPropertyValue(matchingConnection, propertyName)).Returns("Admin");
            _mockConnectionManager.Setup(m => m.GetPropertyValue(nonMatchingConnection, propertyName)).Returns("User");

            await _task.RunTask(callerConnection, request, CancellationToken.None);

            _mockSendMessageTask.Verify(m => m.RunTask(It.Is<WrappedWebSocket>(w => w.UserId == 124), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockSendMessageTask.Verify(m => m.RunTask(It.Is<WrappedWebSocket>(w => w.UserId == 125), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RunTask_Should_Not_Send_Response_If_Caller_Not_In_Group()
        {
            var groupId = 1;
            var propertyName = "Role";
            var response = new WebSocketResponse { TaskName = "TestTask", Data = "Response data" };
            var request = new WebSocketRequest
            {
                ApiName = "TestApi",
                Endpoint = "/test",
                Method = "POST",
                Data = JsonConvert.SerializeObject((propertyName, response))
            };

            var callerWebSocket = new Mock<WebSocket>().Object;
            var nonMatchingWebSocket = new Mock<WebSocket>().Object;

            var callerConnection = new WrappedWebSocket { WebSocket = callerWebSocket, UserId = 123 };
            var nonMatchingConnection = new WrappedWebSocket { WebSocket = nonMatchingWebSocket, UserId = 125 };

            var connections = new List<WrappedWebSocket> { nonMatchingConnection };

            _mockConnectionManager.Setup(m => m.GetGroupId(It.IsAny<WrappedWebSocket>())).Returns(groupId);
            _mockConnectionManager.Setup(m => m.GetConnections(groupId)).Returns(connections);

            await _task.RunTask(callerConnection, request, CancellationToken.None);

            _mockSendMessageTask.Verify(m => m.RunTask(It.IsAny<WrappedWebSocket>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
