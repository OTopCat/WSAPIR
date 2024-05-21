using System.Net.WebSockets;
using WSAPIR.Interfaces;
using WSAPIR.Main;
using WSAPIR.Models;

public class WebSocketMiddleware : IWebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebSocketHandler _webSocketHandler;
    private readonly WebSocketAuth _webSocketAuth;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<WebSocketMiddleware> _logger;

    public WebSocketMiddleware(
        RequestDelegate next,
        WebSocketHandler webSocketHandler,
        WebSocketAuth webSocketAuth,
        ILogger<WebSocketMiddleware> logger,
        IConnectionManager connectionManager)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _webSocketHandler = webSocketHandler ?? throw new ArgumentNullException(nameof(webSocketHandler));
        _webSocketAuth = webSocketAuth ?? throw new ArgumentNullException(nameof(webSocketAuth));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
    }

    /// <summary>
    /// Middleware for handling WebSocket connections.
    /// Verifies JWT in the query parameters to establish a secure WebSocket connection.
    /// If the JWT is valid, wraps the WebSocket into a WrappedWebSocket and assigns it to a group.
    /// Unauthorized and invalid requests are logged and rejected with appropriate status codes.
    /// </summary>
    /// <param name="context">The HttpContext for the current request.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.WebSockets.IsWebSocketRequest)
        {
            await HandleWebSocketRequestAsync(context);
        }
        else
        {
            await _next(context);
        }
    }

    /// <summary>
    /// Handles WebSocket requests by validating the JWT and establishing the WebSocket connection.
    /// </summary>
    /// <param name="context">The HttpContext for the current request.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task HandleWebSocketRequestAsync(HttpContext context)
    {
        if (TryGetJwtFromQuery(context.Request.Query, out var jwt))
        {
            var authData = await _webSocketAuth.InitialAuthAsync(jwt);
            if (authData.IsAuthenticated)
            {
                await EstablishWebSocketConnectionAsync(context, authData);
            }
            else
            {
                _logger.LogWarning("Unauthorized WebSocket connection attempt.");
                SetResponseStatusCode(context, StatusCodes.Status401Unauthorized, "Unauthorized");
            }
        }
        else
        {
            _logger.LogWarning("WebSocket connection attempt without JWT.");
            SetResponseStatusCode(context, StatusCodes.Status400BadRequest, "Bad Request: JWT not provided");
        }
    }

    /// <summary>
    /// Establishes a WebSocket connection and adds it to the appropriate group.
    /// </summary>
    /// <param name="context">The HttpContext for the current request.</param>
    /// <param name="authData">The authentication data extracted from the JWT.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task EstablishWebSocketConnectionAsync(HttpContext context, InitialAuthData authData)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        try
        {
            var wws = new WrappedWebSocket
            {
                WebSocket = webSocket,
                UserId = authData.UserId
            };

            await _connectionManager.AddToGroupAsync(authData.CustomerId, wws);
            await _webSocketHandler.HandleWebSocketAsync(wws);
        }
        finally
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            webSocket.Dispose();
        }
    }

    /// <summary>
    /// Checks if a JWT is provided as a query parameter of the WebSocket connection request.
    /// </summary>
    /// <param name="queryCollection">The query collection to check.</param>
    /// <param name="jwt">The extracted JWT, if present.</param>
    /// <returns>True if a JWT is provided; otherwise, false.</returns>
    private bool TryGetJwtFromQuery(IQueryCollection queryCollection, out string jwt)
    {
        if (queryCollection == null)
        {
            throw new ArgumentNullException(nameof(queryCollection));
        }

        if (queryCollection.TryGetValue("jwt", out var values) && values.Count > 0)
        {
            jwt = values.FirstOrDefault() ?? string.Empty; // Ensure jwt is not null
            return true;
        }

        _logger.LogError("WebSocketMiddleware - Error retrieving initial connection data: JWT not found.");
        jwt = string.Empty; // Assign an empty string instead of null
        return false;
    }

    /// <summary>
    /// Sets the response status code and writes an error message to the response.
    /// </summary>
    /// <param name="context">The HttpContext for the current request.</param>
    /// <param name="statusCode">The status code to set.</param>
    /// <param name="message">The error message to write to the response.</param>
    private void SetResponseStatusCode(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var errorResponse = new { error = message };
        context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
    }
}
