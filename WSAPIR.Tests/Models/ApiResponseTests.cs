using WSAPIR.Models;

namespace WSAPIR.Tests
{
    public class ApiResponseTests
    {
        [Fact]
        public void ApiResponse_Should_Initialize_With_Required_Properties()
        {
            var apiResponse = new ApiResponse
            {
                TaskName = "TestTask",
                Sender = "TestSender"
            };

            Assert.Equal("TestTask", apiResponse.TaskName);
            Assert.Equal("TestSender", apiResponse.Sender);
        }

        [Fact]
        public void ApiResponse_Should_Allow_Optional_Data()
        {
            var apiResponse = new ApiResponse
            {
                TaskName = "TestTask",
                Sender = "TestSender",
                Data = "SomeData"
            };

            Assert.NotNull(apiResponse.Data);
            Assert.Equal("SomeData", apiResponse.Data);
        }

        [Fact]
        public void ApiResponse_Should_Allow_Null_Data()
        {
            var apiResponse = new ApiResponse
            {
                TaskName = "TestTask",
                Sender = "TestSender",
                Data = null
            };

            Assert.Null(apiResponse.Data);
        }

        [Fact]
        public void ApiResponse_Should_Allow_GroupId()
        {
            var apiResponse = new ApiResponse
            {
                TaskName = "TestTask",
                Sender = "TestSender",
                GroupId = 123
            };

            Assert.Equal(123, apiResponse.GroupId);
        }

        [Fact]
        public void ApiResponse_Should_Set_And_Get_Properties_Correctly()
        {

            var apiResponse = new ApiResponse
            {
                TaskName = "TestTask",
                Sender = "TestSender",
                Data = "SomeData",
                GroupId = 123
            };

            Assert.Equal("TestTask", apiResponse.TaskName);
            Assert.Equal("TestSender", apiResponse.Sender);
            Assert.Equal("SomeData", apiResponse.Data);
            Assert.Equal(123, apiResponse.GroupId);
        }
    }
}
