namespace WSAPIR.Models
{
    /// <summary>
    /// Settings for WebSocket authentication.
    /// </summary>
    public class WebSocketAuthSettings
    {
        /// <summary>
        /// Gets or sets the authority endpoint.
        /// </summary>
        public string? AuthorityEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the API name.
        /// </summary>
        public string? ApiName { get; set; }

        /// <summary>
        /// Gets or sets the claim mappings.
        /// </summary>
        public ClaimMappings ClaimMappings { get; set; } = new ClaimMappings();
    }
}
