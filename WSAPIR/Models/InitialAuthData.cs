namespace WSAPIR.Models
{
    /// <summary>
    /// Object to store data from claims while we check Authentication on connect. this is then used to create WrappedWebSocket obj
    /// </summary>
    public class InitialAuthData
    {
        /// <summary>
        /// as name suggests
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// user id from claims
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// customer id for multitenancy
        /// </summary>
        public int CustomerId { get; set; }


        /// <summary>
        /// module for multi API env.
        /// </summary>
        public string? Module { get; set; }
    }
}

