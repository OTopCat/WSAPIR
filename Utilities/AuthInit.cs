using WSAPIR.Main;

namespace WSAPIR.Utilities
{
    /// <summary>
    /// Utility class for initializing and stopping WebSocket authentication services.
    /// </summary>
    public class AuthInit : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuthInit> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthInit"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="logger">The logger.</param>
        public AuthInit(IServiceProvider serviceProvider, ILogger<AuthInit> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Starts the WebSocket authentication initialization process.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var authSingleton = scope.ServiceProvider.GetRequiredService<WebSocketAuth>();

            try
            {
                _logger.LogInformation("Initializing WebSocket authentication.");
                await authSingleton.InitializeAsync();
                _logger.LogInformation("WebSocket authentication initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to initialize WebSocket authentication. Shutting down application.");
                throw;  // Rethrow the exception to terminate the application.
            }
        }

        /// <summary>
        /// Stops the WebSocket authentication services.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping WebSocket authentication services.");
            return Task.CompletedTask;
        }
    }
}
