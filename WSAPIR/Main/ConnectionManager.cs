using System.Collections.Concurrent;
using WSAPIR.Interfaces;
using WSAPIR.Models;

namespace WSAPIR.Main
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<int, ConcurrentBag<WrappedWebSocket>> _clientConnections = new();
        private readonly ILogger<ConnectionManager> _logger;

        public ConnectionManager(ILogger<ConnectionManager> logger)
        {
            _logger = logger;
        }

        public Task AddToGroupAsync(int groupId, WrappedWebSocket caller)
        {
            _clientConnections.AddOrUpdate(groupId, new ConcurrentBag<WrappedWebSocket> { caller }, (key, oldConnections) =>
            {
                oldConnections.Add(caller);
                return oldConnections;
            });

            _logger.LogInformation("Connection Manager: Connection added to group {GroupId}. Group now has {Count} connections.", groupId, GetConnections(groupId).Count());
            return Task.CompletedTask;
        }

        public void RemoveFromGroup(WrappedWebSocket caller)
        {
            int groupId = GetGroupId(caller);

            if (groupId == -1)
            {
                _logger.LogError("Connection Manager: Error retrieving connection group for removal.");
                return;
            }

            if (_clientConnections.TryGetValue(groupId, out var connections))
            {
                var updatedConnections = new ConcurrentBag<WrappedWebSocket>(connections.Except(new[] { caller }));
                if (updatedConnections.IsEmpty)
                {
                    _clientConnections.TryRemove(groupId, out _);
                }
                else
                {
                    _clientConnections[groupId] = updatedConnections;
                }

                _logger.LogInformation("Connection Manager: Connection removed from group {GroupId}. Group now has {Count} connection(s).", groupId, updatedConnections.Count);
            }
        }

        public IEnumerable<WrappedWebSocket> GetConnections(int groupId)
        {
            return _clientConnections.TryGetValue(groupId, out var connections) ? connections : Enumerable.Empty<WrappedWebSocket>();
        }

        public int GetGroupId(WrappedWebSocket caller)
        {
            foreach (var (groupId, connections) in _clientConnections)
            {
                if (connections.Contains(caller))
                {
                    _logger.LogInformation("Connection Manager: Retrieved groupId {GroupId}.", groupId);
                    return groupId;
                }
            }

            _logger.LogError("Connection Manager: Error retrieving connection group.");
            return -1;
        }

        public object? GetPropertyValue(WrappedWebSocket caller, string propertyName)
        {
            if (caller == null)
            {
                _logger.LogError("Connection Manager: Caller is null.");
                return null;
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                _logger.LogError("Connection Manager: Property name is null or empty.");
                return null;
            }

            try
            {
                var property = caller.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    _logger.LogError("Connection Manager: Property '{PropertyName}' not found on caller.", propertyName);
                    return null;
                }

                return property.GetValue(caller);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection Manager: Error retrieving caller property.");
                return null;
            }
        }
    }
}
