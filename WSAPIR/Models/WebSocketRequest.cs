namespace WSAPIR.Models
{
    /// <summary>
    /// request object coming in WS connection from front-end
    /// </summary>
    public class WebSocketRequest
    {
        /// <summary>
        /// JWT from front-end
        /// </summary>
        public string? JWT { get; set; }

        /// <summary>
        /// Gets or sets the name of the API.
        /// </summary>
        public required string ApiName { get; set; }

        /// <summary>
        /// Gets or sets the endpoint of the API.
        /// </summary>
        public required string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method of the API request (GET, POST, etc.).
        /// </summary>
        public required string Method { get; set; }

        /// <summary>
        /// Gets or sets the data to be sent with the request.
        /// </summary>
        public string? Data { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the callback task to be executed on response data.
        /// </summary>
        public string? CallbackTask { get; set; }
    }
}

