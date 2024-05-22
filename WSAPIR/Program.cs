using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;
using WSAPIR.Main;
using System;
using System.IO;
using WSAPIR.Interfaces;
using WSAPIR.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

// Register IHttpClientFactory
builder.Services.AddHttpClient();

// Custom service registrations
Directory.CreateDirectory("error-logs");
builder.Services.InstallWebsocket(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(builder.Configuration.GetSection("WSSettings").GetValue<int>("KeepAliveTimeInSeconds"))
};
app.UseWebSockets(webSocketOptions);
app.UseMiddleware<WebSocketMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// Log registered services
var logger = app.Services.GetRequiredService<ILogger<Program>>();
foreach (var service in app.Services.GetServices<IWebSocketTask>())
{
    logger.LogInformation($"Registered service: {service.GetType().Name}");
}
logger.LogInformation("Finished logging services.");

try
{
    app.Run();
}
catch (TaskCanceledException ex)
{
    logger.LogError(ex, "Application startup was canceled.");
}
catch (Exception ex)
{
    logger.LogError(ex, "An unexpected error occurred during application startup.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
