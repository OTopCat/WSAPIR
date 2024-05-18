using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using WSAPIR.Interfaces;
using WSAPIR.Models;

namespace WSAPIR.Main
{
    /// <summary>
    /// WebSocket authentication implementation.
    /// </summary>
    public class WebSocketAuth : IWebSocketAuth
    {
        private readonly string _authorityEndpoint;
        private readonly string _apiName;
        private readonly ClaimMappings _claimMappings;
        private TokenValidationParameters? _validationParams;
        private bool _initialized;
        private readonly ILogger<WebSocketAuth> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketAuth"/> class.
        /// </summary>
        /// <param name="settings">The authentication settings.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="hostApplicationLifetime">The host application lifetime.</param>
        public WebSocketAuth(IOptions<WebSocketAuthSettings> settings, ILogger<WebSocketAuth> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            var authSettings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _authorityEndpoint = authSettings.AuthorityEndpoint ?? throw new ArgumentNullException(nameof(authSettings.AuthorityEndpoint));
            _apiName = authSettings.ApiName ?? throw new ArgumentNullException(nameof(authSettings.ApiName));
            _claimMappings = authSettings.ClaimMappings ?? throw new ArgumentNullException(nameof(authSettings.ClaimMappings));
        }

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            try
            {
                _validationParams = await GetValidationParametersAsync();
                _initialized = true;
                _logger.LogInformation("WebSocketAuth initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Exception thrown while attempting to initialize Auth service and get OpenID configuration from Identity. Shutting down application...");
                _hostApplicationLifetime.StopApplication();
            }
        }

        /// <inheritdoc />
        public bool IsInitialized => _initialized;

        /// <inheritdoc />
        public ClaimsIdentity GetIdentityFromJWT(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                _logger.LogWarning("JWT token is null or empty.");
                return new ClaimsIdentity();
            }

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var claimsPrincipal = handler.ValidateToken(jwt, _validationParams, out _);
                _logger.LogInformation("JWT successfully validated and ClaimsIdentity retrieved.");
                return claimsPrincipal?.Identity as ClaimsIdentity ?? new ClaimsIdentity();
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning(ex, "JWT has expired.");
                return new ClaimsIdentity();
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Invalid JWT.");
                return new ClaimsIdentity();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Exception thrown while attempting to get ClaimsIdentity from JWT.");
                return new ClaimsIdentity();
            }
        }

        /// <inheritdoc />
        public async Task<ClaimsIdentity> GetIdentityFromJWTAsync(string jwt)
        {
            return await Task.Run(() => GetIdentityFromJWT(jwt));
        }

        /// <inheritdoc />
        public async Task<InitialAuthData> InitialAuthAsync(string jwt)
        {
            var authData = new InitialAuthData { IsAuthenticated = false };
            try
            {
                var identity = await GetIdentityFromJWTAsync(jwt);
                if (identity.IsAuthenticated)
                {
                    authData.UserId = GetClaimValueAsInt(identity, _claimMappings.UserIdClaim);
                    authData.CustomerId = GetClaimValueAsInt(identity, _claimMappings.CustomerIdClaim);
                    authData.IsAuthenticated = true;
                    _logger.LogInformation("Initial authentication successful for UserId {UserId}.", authData.UserId);
                }
                else
                {
                    _logger.LogError("WS connection not authenticated.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Exception thrown while attempting to read token.");
            }
            return authData;
        }

        /// <inheritdoc />
        public async Task<bool> IsAuthenticatedAsync(WebSocketRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.JWT))
            {
                _logger.LogWarning("WebSocket request or JWT is null or empty.");
                return false;
            }

            try
            {
                var identity = await GetIdentityFromJWTAsync(request.JWT);
                if (identity.IsAuthenticated)
                {
                    _logger.LogInformation("Request is authenticated.");
                    return true;
                }
                else
                {
                    _logger.LogError("JWT validation failed.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while attempting to check if request is authenticated.");
                return false;
            }
        }

        private async Task<TokenValidationParameters> GetValidationParametersAsync()
        {
            try
            {
                var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{_authorityEndpoint}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever());

                var openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
                _logger.LogInformation("OpenID configuration retrieved successfully.");

                return new TokenValidationParameters
                {
                    ValidIssuer = _authorityEndpoint,
                    ValidAudiences = new[] { $"{_authorityEndpoint}/resources", _apiName, "CsmConfig" },
                    IssuerSigningKeys = openIdConfig.SigningKeys
                };
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Exception thrown while retrieving validation parameters. Shutting Down");
                throw;
            }
        }

        /// <inheritdoc />
        public int GetClaimValueAsInt(ClaimsIdentity identity, string claimType)
        {
            var claimValue = identity.FindFirst(claimType)?.Value;
            return int.TryParse(claimValue, out var result) ? result : 0;
        }
    }
}
