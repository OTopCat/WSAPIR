namespace WSAPIR.Models
{
    /// <summary>
    /// Defines the claim mappings for the WebSocket authentication.
    /// </summary>
    public class ClaimMappings
    {
        /// <summary>
        /// Gets or sets the claim type for CustomerId.
        /// </summary>
        public string CustomerIdClaim { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the claim type for UserId.
        /// </summary>
        public string UserIdClaim { get; set; } = string.Empty;
    }
}