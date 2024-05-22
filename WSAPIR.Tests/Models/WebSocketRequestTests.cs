using WSAPIR.Models;
using Xunit;

namespace WSAPIR.Tests
{
    public class WebSocketRequestTests
    {
        [Fact]
        public void WebSocketRequest_Should_Initialize_With_Default_Values()
        {
            var webSocketRequest = new WebSocketRequest
            {
                ApiName = "defaultApi",
                Endpoint = "defaultEndpoint",
                Method = "GET"
            };

            Assert.Null(webSocketRequest.JWT);
            Assert.Equal("defaultApi", webSocketRequest.ApiName);
            Assert.Equal("defaultEndpoint", webSocketRequest.Endpoint);
            Assert.Equal("GET", webSocketRequest.Method);
            Assert.Equal(string.Empty, webSocketRequest.Data);
            Assert.Null(webSocketRequest.CallbackTask);
        }

        [Fact]
        public void WebSocketRequest_Should_Allow_Setting_JWT()
        {
            var webSocketRequest = new WebSocketRequest
            {
                ApiName = "defaultApi",
                Endpoint = "defaultEndpoint",
                Method = "GET",
                JWT = "sample_jwt"
            };

            Assert.Equal("sample_jwt", webSocketRequest.JWT);
        }

        [Fact]
        public void WebSocketRequest_Should_Allow_Setting_ApiName()
        {
            var webSocketRequest = new WebSocketRequest
            {
                ApiName = "sample_api",
                Endpoint = "defaultEndpoint",
                Method = "GET"
            };

            Assert.Equal("sample_api", webSocketRequest.ApiName);
        }

        [Fact]
        public void WebSocketRequest_Should_Allow_Setting_Endpoint()
        {
            var webSocketRequest = new WebSocketRequest
            {
                ApiName = "defaultApi",
                Endpoint = "sample_endpoint",
                Method = "GET"
            };

            Assert.Equal("sample_endpoint", webSocketRequest.Endpoint);
        }

        [Fact]
        public void WebSocketRequest_Should_Allow_Setting_Method()
        {
            var webSocketRequest = new WebSocketRequest
            {
                ApiName = "defaultApi",
                Endpoint = "defaultEndpoint",
                Method = "POST"
            };

            Assert.Equal("POST", webSocketRequest.Method);
        }

        [Fact]
        public void WebSocketRequest_Should_Allow_Setting_Data()
        {
            var webSocketRequest = new WebSocketRequest
            {
                ApiName = "defaultApi",
                Endpoint = "defaultEndpoint",
                Method = "GET",
                Data = "sample_data"
            };

            Assert.Equal("sample_data", webSocketRequest.Data);
        }

        [Fact]
        public void WebSocketRequest_Should_Allow_Setting_CallbackTask()
        {
            var webSocketRequest = new WebSocketRequest
            {
                ApiName = "defaultApi",
                Endpoint = "defaultEndpoint",
                Method = "GET",
                CallbackTask = "sample_callback"
            };

            Assert.Equal("sample_callback", webSocketRequest.CallbackTask);
        }

        [Fact]
        public void WebSocketRequest_Should_Set_And_Get_Properties_Correctly()
        {
            var webSocketRequest = new WebSocketRequest
            {
                ApiName = "sample_api",
                Endpoint = "sample_endpoint",
                Method = "POST",
                JWT = "sample_jwt",
                Data = "sample_data",
                CallbackTask = "sample_callback"
            };

            Assert.Equal("sample_jwt", webSocketRequest.JWT);
            Assert.Equal("sample_api", webSocketRequest.ApiName);
            Assert.Equal("sample_endpoint", webSocketRequest.Endpoint);
            Assert.Equal("POST", webSocketRequest.Method);
            Assert.Equal("sample_data", webSocketRequest.Data);
            Assert.Equal("sample_callback", webSocketRequest.CallbackTask);
        }

        [Fact]
        public void WebSocketRequest_Should_Not_Throw_Exception_If_Required_Properties_Are_Set()
        {
            var webSocketRequest = new WebSocketRequest
            {
                ApiName = "sample_api",
                Endpoint = "sample_endpoint",
                Method = "GET"
            };

            Assert.Equal("sample_api", webSocketRequest.ApiName);
            Assert.Equal("sample_endpoint", webSocketRequest.Endpoint);
            Assert.Equal("GET", webSocketRequest.Method);
        }
    }
}
