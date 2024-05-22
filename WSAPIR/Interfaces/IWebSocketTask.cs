using WSAPIR.Models;

namespace WSAPIR.Interfaces
{
    /// <summary>
    /// Represents a WebSocket task.
    /// </summary>
    public interface IWebSocketTask
    {
        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        string TaskName { get; }

        /// <summary>
        /// Executes the WebSocket task.
        /// </summary>
        /// <param name="wws">The wrapped WebSocket.</param>
        /// <param name="request">Required WebSocketRequest object</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns></returns>
        Task RunTask(WrappedWebSocket wws, WebSocketRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the WebSocket task with response
        /// </summary>
        /// <param name="wws">The wrapped WebSocket.</param>
        /// <param name="request">Required WebSocketRequest object</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns></returns>
        Task RunTask(WrappedWebSocket wws, WebSocketResponse response, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the WebSocket task.
        /// </summary>
        /// <param name="wws">The wrapped WebSocket.</param>
        /// <param name="data">Optional data for the task.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns></returns>
        Task RunTask(WrappedWebSocket wws, string? data, CancellationToken cancellationToken);
    }
}
