using WSAPIR.Models;

namespace WSAPIR.Tests
{
    public class ConnectionRequestTests
    {
        [Fact]
        public void ConnectionRequest_Should_Initialize_With_Null_Properties()
        {
            var connectionRequest = new ConnectionRequest();

            Assert.Null(connectionRequest.JWT);
            Assert.Null(connectionRequest.Module);
        }

        [Fact]
        public void ConnectionRequest_Should_Allow_Setting_JWT()
        {
            var connectionRequest = new ConnectionRequest
            {
                JWT = "sample_jwt"
            };

            Assert.Equal("sample_jwt", connectionRequest.JWT);
        }

        [Fact]
        public void ConnectionRequest_Should_Allow_Setting_Module()
        {
            var connectionRequest = new ConnectionRequest
            {
                Module = "sample_module"
            };

            Assert.Equal("sample_module", connectionRequest.Module);
        }

        [Fact]
        public void ConnectionRequest_Should_Set_And_Get_Properties_Correctly()
        {
            var connectionRequest = new ConnectionRequest
            {
                JWT = "sample_jwt",
                Module = "sample_module"
            };

            Assert.Equal("sample_jwt", connectionRequest.JWT);
            Assert.Equal("sample_module", connectionRequest.Module);
        }
    }
}
