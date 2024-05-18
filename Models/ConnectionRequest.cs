using System;
namespace WSAPIR.Models
{
    public class ConnectionRequest
    {
        public string? JWT { get; set; }
        public string? Module { get; set; }
    }
}