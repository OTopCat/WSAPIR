using System.Net.WebSockets;
using WSAPIR.Models;

namespace WSAPIR.Tests
{
    public class WrappedWebSocketTests
    {
        [Fact]
        public void WrappedWebSocket_Should_Initialize_With_Required_WebSocket()
        {
            var mockWebSocket = new ClientWebSocket();
            var wrappedWebSocket = new WrappedWebSocket
            {
                WebSocket = mockWebSocket
            };

            Assert.Equal(mockWebSocket, wrappedWebSocket.WebSocket);
            Assert.Null(wrappedWebSocket.Module);
            Assert.Equal(0, wrappedWebSocket.UserId);
        }

        [Fact]
        public void WrappedWebSocket_Should_Allow_Setting_Module()
        {
            var mockWebSocket = new ClientWebSocket();
            var wrappedWebSocket = new WrappedWebSocket
            {
                WebSocket = mockWebSocket,
                Module = "sample_module"
            };

            Assert.Equal("sample_module", wrappedWebSocket.Module);
        }

        [Fact]
        public void WrappedWebSocket_Should_Allow_Setting_UserId()
        {
            var mockWebSocket = new ClientWebSocket();
            var wrappedWebSocket = new WrappedWebSocket
            {
                WebSocket = mockWebSocket,
                UserId = 12345
            };

            Assert.Equal(12345, wrappedWebSocket.UserId);
        }

        [Fact]
        public void WrappedWebSocket_Should_Set_And_Get_Properties_Correctly()
        {
            var mockWebSocket = new ClientWebSocket();
            var wrappedWebSocket = new WrappedWebSocket
            {
                WebSocket = mockWebSocket,
                Module = "sample_module",
                UserId = 12345
            };

            Assert.Equal(mockWebSocket, wrappedWebSocket.WebSocket);
            Assert.Equal("sample_module", wrappedWebSocket.Module);
            Assert.Equal(12345, wrappedWebSocket.UserId);
        }
    }
}
