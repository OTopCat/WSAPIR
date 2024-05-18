using WSAPIR.Models;

namespace WSAPIR.Interfaces
{
    /// <summary>
    /// Interface for managing WebSocket connections.
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// Adds a WrappedWebSocket to the specified group.
        /// </summary>
        /// <param name="groupId">The ID of the group to add the connection to.</param>
        /// <param name="caller">The WrappedWebSocket representing the connection to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddToGroupAsync(int groupId, WrappedWebSocket caller);

        /// <summary>
        /// Removes a WrappedWebSocket from its group.
        /// </summary>
        /// <param name="caller">The WrappedWebSocket to remove.</param>
        void RemoveFromGroup(WrappedWebSocket caller);

        /// <summary>
        /// Gets all connections in the specified group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <returns>An IEnumerable of WrappedWebSocket representing the connections in the group.</returns>
        IEnumerable<WrappedWebSocket> GetConnections(int groupId);

        /// <summary>
        /// Retrieves the group ID for the specified connection.
        /// </summary>
        /// <param name="caller">The WrappedWebSocket to find the group ID for.</param>
        /// <returns>The group ID, or -1 if the connection is not found in any group.</returns>
        int GetGroupId(WrappedWebSocket caller);

        /// <summary>
        /// Retrieves the value of the specified property from the WrappedWebSocket.
        /// </summary>
        /// <param name="caller">The WrappedWebSocket to retrieve the property value from.</param>
        /// <param name="propertyName">The property name to retrieve the value for.</param>
        /// <returns>The value of the property, or null if an error occurs.</returns>
        object? GetPropertyValue(WrappedWebSocket caller, string propertyName);
    }
}
