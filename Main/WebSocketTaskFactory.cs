using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WSAPIR.Interfaces;

namespace WSAPIR.Main
{
    /// <summary>
    /// Factory for creating WebSocket tasks.
    /// </summary>
    public class WebSocketTaskFactory : IWebSocketTaskFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<Dictionary<string, IWebSocketTask>> _tasks;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketTaskFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public WebSocketTaskFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _tasks = new Lazy<Dictionary<string, IWebSocketTask>>(GetTasks);
        }

        /// <summary>
        /// Gets the WebSocket task by its name.
        /// </summary>
        /// <param name="taskName">The name of the task.</param>
        /// <returns>The corresponding WebSocket task.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the task is not found.</exception>
        public IWebSocketTask GetTask(string taskName)
        {
            if (_tasks.Value.TryGetValue(taskName, out var task))
            {
                return task;
            }
            throw new KeyNotFoundException($"Task with name '{taskName}' not found.");
        }

        /// <summary>
        /// Retrieves all WebSocket tasks from the service provider.
        /// </summary>
        /// <returns>A dictionary of task names and their corresponding tasks.</returns>
        private Dictionary<string, IWebSocketTask> GetTasks()
        {
            return _serviceProvider.GetServices<IWebSocketTask>()
                                   .ToDictionary(task => task.TaskName, task => task);
        }
    }
}
