using System.Security.Claims;
using WSAPIR.Models;

namespace WSAPIR.Interfaces
{
    /// <summary>
    /// Interface for WebSocket authentication.
    /// </summary>
    public interface IWebSocketAuth
    {
        /// <summary>
        /// Initializes the WebSocket authentication.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Gets a value indicating whether the authentication is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the identity from a JWT token.
        /// </summary>
        /// <param name="jwt">The JWT token.</param>
        /// <returns>The ClaimsIdentity.</returns>
        ClaimsIdentity GetIdentityFromJWT(string jwt);

        /// <summary>
        /// Gets the identity from a JWT token asynchronously.
        /// </summary>
        /// <param name="jwt">The JWT token.</param>
        /// <returns>The task result contains the ClaimsIdentity.</returns>
        Task<ClaimsIdentity> GetIdentityFromJWTAsync(string jwt);

        /// <summary>
        /// Performs initial authentication using a JWT token asynchronously.
        /// </summary>
        /// <param name="jwt">The JWT token.</param>
        /// <returns>The task result contains the InitialAuthData.</returns>
        Task<InitialAuthData> InitialAuthAsync(string jwt);

        /// <summary>
        /// Checks whether the request is authenticated asynchronously.
        /// </summary>
        /// <param name="request">The WebSocket request.</param>
        /// <returns>The task result contains a boolean indicating whether the request is authenticated.</returns>
        Task<bool> IsAuthenticatedAsync(WebSocketRequest request);
    }
}
