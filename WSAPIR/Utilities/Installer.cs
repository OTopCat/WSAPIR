using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WSAPIR.Interfaces;
using WSAPIR.Main;
using WSAPIR.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using WSAPIR.Utilities;

public static class Installer
{
    public static void InstallWebsocket(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddSingleton<WebSocketHandler>();

        // Configure WebSocketAuthSettings
        services.Configure<WebSocketAuthSettings>(configuration.GetSection("IdentityServerSettings"));

        // Configure ApiUrlsSettings
        services.Configure<ApiUrlsSettings>(configuration.GetSection("ApiUrlsSettings"));

        // Configure ClaimMappings
        services.Configure<ClaimMappings>(configuration.GetSection("ClaimMappings"));

        // Register WebSocketAuth with WebSocketAuthSettings
        services.AddSingleton<WebSocketAuth>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<WebSocketAuthSettings>>().Value;
            var logger = provider.GetRequiredService<ILogger<WebSocketAuth>>();
            var hostApplicationLifetime = provider.GetRequiredService<IHostApplicationLifetime>();
            return new WebSocketAuth(Options.Create(settings), logger, hostApplicationLifetime);
        });

        services.AddSingleton<IWebSocketAuth>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<WebSocketAuthSettings>>().Value;
            var logger = provider.GetRequiredService<ILogger<WebSocketAuth>>();
            var hostApplicationLifetime = provider.GetRequiredService<IHostApplicationLifetime>();
            return new WebSocketAuth(Options.Create(settings), logger, hostApplicationLifetime);
        });

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
