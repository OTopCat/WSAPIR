namespace WSAPIR.Interfaces
{
    /// <summary>
    /// Factory interface for creating WebSocket tasks.
    /// </summary>
    public interface IWebSocketTaskFactory
    {
        /// <summary>
        /// Gets the task by its name.
        /// </summary>
        /// <param name="taskName">The name of the task.</param>
        /// <returns>The corresponding WebSocket task.</returns>
        IWebSocketTask GetTask(string taskName);
    }
}