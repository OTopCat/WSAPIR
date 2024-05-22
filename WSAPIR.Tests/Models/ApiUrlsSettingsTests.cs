using WSAPIR.Models;

namespace WSAPIR.Tests
{
    public class ApiUrlsSettingsTests
    {
        [Fact]
        public void ApiUrlsSettings_Should_Initialize_With_Empty_Dictionary()
        {
            var apiUrlsSettings = new ApiUrlsSettings();

            Assert.NotNull(apiUrlsSettings.Urls);
            Assert.Empty(apiUrlsSettings.Urls);
        }

        [Fact]
        public void ApiUrlsSettings_Should_Allow_Adding_Urls()
        {
            var apiUrlsSettings = new ApiUrlsSettings();

            apiUrlsSettings.Urls.Add("Api1", "https://api1.example.com");
            apiUrlsSettings.Urls.Add("Api2", "https://api2.example.com");

            Assert.Equal(2, apiUrlsSettings.Urls.Count);
            Assert.Equal("https://api1.example.com", apiUrlsSettings.Urls["Api1"]);
            Assert.Equal("https://api2.example.com", apiUrlsSettings.Urls["Api2"]);
        }

        [Fact]
        public void ApiUrlsSettings_Should_Allow_Updating_Urls()
        {
            var apiUrlsSettings = new ApiUrlsSettings();
            apiUrlsSettings.Urls.Add("Api1", "https://api1.example.com");

            apiUrlsSettings.Urls["Api1"] = "https://updatedapi1.example.com";

            Assert.Equal("https://updatedapi1.example.com", apiUrlsSettings.Urls["Api1"]);
        }

        [Fact]
        public void ApiUrlsSettings_Should_Allow_Removing_Urls()
        {
            var apiUrlsSettings = new ApiUrlsSettings();
            apiUrlsSettings.Urls.Add("Api1", "https://api1.example.com");

            apiUrlsSettings.Urls.Remove("Api1");

            Assert.Empty(apiUrlsSettings.Urls);
        }

        [Fact]
        public void ApiUrlsSettings_Should_Throw_Exception_When_Adding_Duplicate_Keys()
        {
            var apiUrlsSettings = new ApiUrlsSettings();
            apiUrlsSettings.Urls.Add("Api1", "https://api1.example.com");

            Assert.Throws<ArgumentException>(() => apiUrlsSettings.Urls.Add("Api1", "https://duplicateapi1.example.com"));
        }
    }
}
