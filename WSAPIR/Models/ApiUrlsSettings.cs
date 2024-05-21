namespace WSAPIR.Models
{
    /// <summary>
    /// Settings for API URLs.
    /// </summary>
    public class ApiUrlsSettings
    {
        /// <summary>
        /// Gets or sets the dictionary of API URLs.
        /// </summary>
        public Dictionary<string, string> Urls { get; set; } = new Dictionary<string, string>();
    }
}
