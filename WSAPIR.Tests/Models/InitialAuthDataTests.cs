using WSAPIR.Models;

namespace WSAPIR.Tests
{
    public class InitialAuthDataTests
    {
        [Fact]
        public void InitialAuthData_Should_Initialize_With_Default_Values()
        {
            var initialAuthData = new InitialAuthData();

            Assert.False(initialAuthData.IsAuthenticated);
            Assert.Equal(0, initialAuthData.UserId);
            Assert.Equal(0, initialAuthData.CustomerId);
            Assert.Null(initialAuthData.Module);
        }

        [Fact]
        public void InitialAuthData_Should_Allow_Setting_IsAuthenticated()
        {
            var initialAuthData = new InitialAuthData
            {
                IsAuthenticated = true
            };

            Assert.True(initialAuthData.IsAuthenticated);
        }

        [Fact]
        public void InitialAuthData_Should_Allow_Setting_UserId()
        {
            var initialAuthData = new InitialAuthData
            {
                UserId = 12345
            };

            Assert.Equal(12345, initialAuthData.UserId);
        }

        [Fact]
        public void InitialAuthData_Should_Allow_Setting_CustomerId()
        {
            var initialAuthData = new InitialAuthData
            {
                CustomerId = 67890
            };

            Assert.Equal(67890, initialAuthData.CustomerId);
        }

        [Fact]
        public void InitialAuthData_Should_Allow_Setting_Module()
        {
            var initialAuthData = new InitialAuthData
            {
                Module = "sample_module"
            };

            Assert.Equal("sample_module", initialAuthData.Module);
        }

        [Fact]
        public void InitialAuthData_Should_Set_And_Get_Properties_Correctly()
        {
            var initialAuthData = new InitialAuthData
            {
                IsAuthenticated = true,
                UserId = 12345,
                CustomerId = 67890,
                Module = "sample_module"
            };

            Assert.True(initialAuthData.IsAuthenticated);
            Assert.Equal(12345, initialAuthData.UserId);
            Assert.Equal(67890, initialAuthData.CustomerId);
            Assert.Equal("sample_module", initialAuthData.Module);
        }
    }
}
