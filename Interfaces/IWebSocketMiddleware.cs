namespace WSAPIR.Interfaces
{
    public interface IWebSocketMiddleware
    {
        /// <summary>
        /// Middleware for handling WebSocket connections.
        /// Verifies JWT in the query parameters to establish a secure WebSocket connection.
        /// If the JWT is valid, wraps the WebSocket into a WrappedWebSocket and assigns it to a group.
        /// Unauthorized and invalid requests are logged and rejected with appropriate status codes.
        /// </summary>
        /// <param name="context">The HttpContext for the current request.</param>
        /// <returns></returns>
        Task InvokeAsync(HttpContext context);
    }
}