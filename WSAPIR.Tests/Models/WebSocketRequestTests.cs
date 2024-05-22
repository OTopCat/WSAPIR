using WSAPIR.Models;

namespace WSAPIR.Tests
{
    public class WebSocketRequestTests
    {
        [Fact]
        public void WebSocketRequest_Should_Initialize_With_Default_Values()
        {
            var webSocketRequest = new WebSocketRequest();

            Assert.Null(webSocketRequest.JWT);
            Assert.Equal(string.Empty, webSocketRequest.TaskName);
            Assert.Equal(string.Empty, webSocketRequest.Data);
        }

        [Fact]
        public void WebSocketRequest_Should_Allow_Setting_JWT()
        {
            var webSocketRequest = new WebSocketRequest
            {
                JWT = "sample_jwt"
            };

            Assert.Equal("sample_jwt", webSocketRequest.JWT);
        }

        [Fact]
        public void WebSocketRequest_Should_Allow_Setting_TaskName()
        {
            var webSocketRequest = new WebSocketRequest
            {
                TaskName = "sample_task"
            };

            Assert.Equal("sample_task", webSocketRequest.TaskName);
        }

        [Fact]
        public void WebSocketRequest_Should_Allow_Setting_Data()
        {
            var webSocketRequest = new WebSocketRequest
            {
                Data = "sample_data"
            };

            Assert.Equal("sample_data", webSocketRequest.Data);
        }

        [Fact]
        public void WebSocketRequest_Should_Set_And_Get_Properties_Correctly()
        {
            var webSocketRequest = new WebSocketRequest
            {
                JWT = "sample_jwt",
                TaskName = "sample_task",
                Data = "sample_data"
            };

            Assert.Equal("sample_jwt", webSocketRequest.JWT);
            Assert.Equal("sample_task", webSocketRequest.TaskName);
            Assert.Equal("sample_data", webSocketRequest.Data);
        }
    }
}
