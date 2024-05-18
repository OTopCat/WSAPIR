namespace WSAPIR.Models
{
	public class WebSocketRequest
	{
		// JWT from front-end
		public string? JWT { get; set; }
		// Taks to execute on target API
		public string TaskName { get; set; } = string.Empty;
		// Data json string
		public string Data { get; set; } = string.Empty;
	}
}

