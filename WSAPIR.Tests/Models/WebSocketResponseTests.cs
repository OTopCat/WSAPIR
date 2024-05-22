using System.Text;
using Newtonsoft.Json;
using WSAPIR.Models;

namespace WSAPIR.Tests
{
    public class WebSocketResponseTests
    {
        [Fact]
        public void WebSocketResponse_Should_Initialize_With_Default_Values()
        {
            var webSocketResponse = new WebSocketResponse
            {
                TaskName = "sample_task"
            };

            Assert.Equal(string.Empty, webSocketResponse.SourceAPI);
            Assert.Equal("sample_task", webSocketResponse.TaskName);
            Assert.Null(webSocketResponse.Data);
        }

        [Fact]
        public void WebSocketResponse_Should_Allow_Setting_SourceAPI()
        {
            var webSocketResponse = new WebSocketResponse
            {
                TaskName = "sample_task",
                SourceAPI = "sample_api"
            };

            Assert.Equal("sample_api", webSocketResponse.SourceAPI);
        }

        [Fact]
        public void WebSocketResponse_Should_Allow_Setting_TaskName()
        {
            var webSocketResponse = new WebSocketResponse
            {
                TaskName = "sample_task"
            };

            webSocketResponse.TaskName = "new_task";

            Assert.Equal("new_task", webSocketResponse.TaskName);
        }

        [Fact]
        public void WebSocketResponse_Should_Allow_Setting_Data()
        {
            var webSocketResponse = new WebSocketResponse
            {
                TaskName = "sample_task"
            };

            var sampleData = new { Property1 = "value1", Property2 = 2 };
            webSocketResponse.Data = sampleData;

            Assert.Equal(sampleData, webSocketResponse.Data);
        }

        [Fact]
        public void WebSocketResponse_Should_Convert_To_Buffer_Correctly()
        {
            var webSocketResponse = new WebSocketResponse
            {
                SourceAPI = "sample_api",
                TaskName = "sample_task",
                Data = new { Property1 = "value1", Property2 = 2 }
            };

            var buffer = webSocketResponse.ToBuffer();
            Assert.NotNull(buffer.Array);

            if (buffer.Array != null)
            {
                var jsonString = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                var expectedJsonString = JsonConvert.SerializeObject(webSocketResponse);

                Assert.Equal(expectedJsonString, jsonString);
            }
        }

        [Fact]
        public void WebSocketResponse_Should_Set_And_Get_Properties_Correctly()
        {
            var sampleData = new { Property1 = "value1", Property2 = 2 };

            var webSocketResponse = new WebSocketResponse
            {
                SourceAPI = "sample_api",
                TaskName = "sample_task",
                Data = sampleData
            };

            Assert.Equal("sample_api", webSocketResponse.SourceAPI);
            Assert.Equal("sample_task", webSocketResponse.TaskName);
            Assert.Equal(sampleData, webSocketResponse.Data);
        }
    }
}
