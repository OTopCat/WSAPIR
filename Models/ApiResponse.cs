namespace WSAPIR.Models
{
    /// <summary>
    /// Represents an API response.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Gets or sets the data of the response.
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// Gets or sets the group ID associated with the response.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the name of the task to perform.
        /// </summary>
        public required string TaskName { get; set; }

        /// <summary>
        /// Gets or sets the sender of the response.
        /// </summary>
        public required string Sender { get; set; }
    }
}
