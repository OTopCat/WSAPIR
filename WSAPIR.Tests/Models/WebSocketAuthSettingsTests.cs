using WSAPIR.Models;

namespace WSAPIR.Tests
{
    public class WebSocketAuthSettingsTests
    {
        [Fact]
        public void WebSocketAuthSettings_Should_Initialize_With_Default_Values()
        {
            var webSocketAuthSettings = new WebSocketAuthSettings();

            Assert.Null(webSocketAuthSettings.AuthorityEndpoint);
            Assert.Null(webSocketAuthSettings.ApiName);
            Assert.NotNull(webSocketAuthSettings.ClaimMappings);
        }

        [Fact]
        public void WebSocketAuthSettings_Should_Allow_Setting_AuthorityEndpoint()
        {
            var webSocketAuthSettings = new WebSocketAuthSettings
            {
                AuthorityEndpoint = "https://authority.example.com"
            };

            Assert.Equal("https://authority.example.com", webSocketAuthSettings.AuthorityEndpoint);
        }

        [Fact]
        public void WebSocketAuthSettings_Should_Allow_Setting_ApiName()
        {
            var webSocketAuthSettings = new WebSocketAuthSettings
            {
                ApiName = "sample_api"
            };

            Assert.Equal("sample_api", webSocketAuthSettings.ApiName);
        }

        [Fact]
        public void WebSocketAuthSettings_Should_Allow_Setting_ClaimMappings()
        {
            var webSocketAuthSettings = new WebSocketAuthSettings();
            var claimMappings = new ClaimMappings
            {
                CustomerIdClaim = "customer_id",
                UserIdClaim = "user_id"
            };

            webSocketAuthSettings.ClaimMappings = claimMappings;

            Assert.Equal("customer_id", webSocketAuthSettings.ClaimMappings.CustomerIdClaim);
            Assert.Equal("user_id", webSocketAuthSettings.ClaimMappings.UserIdClaim);
        }

        [Fact]
        public void WebSocketAuthSettings_Should_Set_And_Get_Properties_Correctly()
        {
            var claimMappings = new ClaimMappings
            {
                CustomerIdClaim = "customer_id",
                UserIdClaim = "user_id"
            };

            var webSocketAuthSettings = new WebSocketAuthSettings
            {
                AuthorityEndpoint = "https://authority.example.com",
                ApiName = "sample_api",
                ClaimMappings = claimMappings
            };

            Assert.Equal("https://authority.example.com", webSocketAuthSettings.AuthorityEndpoint);
            Assert.Equal("sample_api", webSocketAuthSettings.ApiName);
            Assert.Equal("customer_id", webSocketAuthSettings.ClaimMappings.CustomerIdClaim);
            Assert.Equal("user_id", webSocketAuthSettings.ClaimMappings.UserIdClaim);
        }
    }
}
