using WSAPIR.Models;

namespace WSAPIR.Tests
{
    public class ClaimMappingsTests
    {
        [Fact]
        public void ClaimMappings_Should_Initialize_With_Empty_Strings()
        {
            var claimMappings = new ClaimMappings();

            Assert.NotNull(claimMappings.CustomerIdClaim);
            Assert.Equal(string.Empty, claimMappings.CustomerIdClaim);
            Assert.NotNull(claimMappings.UserIdClaim);
            Assert.Equal(string.Empty, claimMappings.UserIdClaim);
        }

        [Fact]
        public void ClaimMappings_Should_Allow_Setting_CustomerIdClaim()
        {
            var claimMappings = new ClaimMappings
            {
                CustomerIdClaim = "customer_id_claim_type"
            };

            Assert.Equal("customer_id_claim_type", claimMappings.CustomerIdClaim);
        }

        [Fact]
        public void ClaimMappings_Should_Allow_Setting_UserIdClaim()
        {
            var claimMappings = new ClaimMappings
            {
                UserIdClaim = "user_id_claim_type"
            };

            Assert.Equal("user_id_claim_type", claimMappings.UserIdClaim);
        }

        [Fact]
        public void ClaimMappings_Should_Set_And_Get_Properties_Correctly()
        {
            var claimMappings = new ClaimMappings
            {
                CustomerIdClaim = "customer_id_claim_type",
                UserIdClaim = "user_id_claim_type"
            };

            Assert.Equal("customer_id_claim_type", claimMappings.CustomerIdClaim);
            Assert.Equal("user_id_claim_type", claimMappings.UserIdClaim);
        }
    }
}
