namespace WSAPIR.Models
{
    public class InitialAuthData
    {
        public bool IsAuthenticated { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string? Module { get; set; }
    }
}

