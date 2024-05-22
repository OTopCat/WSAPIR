using Microsoft.Extensions.Options;
using WSAPIR.Interfaces;
using WSAPIR.Main;
using WSAPIR.Models;

namespace WSAPIR.Utilities
{
    public static class Installer
    {
        public static void InstallWebsocket(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionManager, ConnectionManager>();
            services.AddSingleton<IWebSocketHandler, WebSocketHandler>();

            // Configure WebSocketAuthSettings
            services.Configure<WebSocketAuthSettings>(configuration.GetSection("IdentityServerSettings"));

            // Configure ApiUrlsSettings
            services.Configure<ApiUrlsSettings>(configuration.GetSection("ApiUrlsSettings"));

            // Configure ClaimMappings
            services.Configure<ClaimMappings>(configuration.GetSection("ClaimMappings"));

            // Register WebSocketAuth with WebSocketAuthSettings
            services.AddSingleton<IWebSocketAuth>(provider =>
                new WebSocketAuth(
                    Options.Create(provider.GetRequiredService<IOptions<WebSocketAuthSettings>>().Value),
                    provider.GetRequiredService<ILogger<WebSocketAuth>>(),
                    provider.GetRequiredService<IHostApplicationLifetime>()
                    )
                );

            services.AddHostedService<AuthInit>();



            // Register all implementations of IWebSocketTask as transient
            var taskTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IWebSocketTask).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

            foreach (var taskType in taskTypes)
            {
                services.AddTransient(typeof(IWebSocketTask), taskType);
            }

            services.AddSingleton<IWebSocketTaskFactory, WebSocketTaskFactory>();
        }
    }
}
