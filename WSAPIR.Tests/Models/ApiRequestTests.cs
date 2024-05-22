using WSAPIR.Models;

namespace WSAPIR.Tests
{
    public class ApiRequestTests
    {
        [Fact]
        public void Constructor_Should_Set_Properties_Correctly()
        {
            var apiRequest = new ApiRequest
            {
                ApiName = "TestApi",
                Endpoint = "/test",
                Method = "GET"
            };

            Assert.Equal("TestApi", apiRequest.ApiName);
            Assert.Equal("/test", apiRequest.Endpoint);
            Assert.Equal("GET", apiRequest.Method);
            Assert.Null(apiRequest.Data);
            Assert.Null(apiRequest.CallbackTask);
        }

        [Fact]
        public void Can_Set_Optional_Properties()
        {
            var apiRequest = new ApiRequest
            {
                ApiName = "TestApi",
                Endpoint = "/test",
                Method = "GET",
                Data = "SampleData",
                CallbackTask = "SampleCallback"
            };

            Assert.Equal("SampleData", apiRequest.Data);
            Assert.Equal("SampleCallback", apiRequest.CallbackTask);
        }
    }
}
