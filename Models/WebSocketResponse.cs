using System.Text;
using Newtonsoft.Json;

namespace WSAPIR.Models
{
	public class WebSocketResponse
	{
		// sender API
		public string SourceAPI { get; set; } = string.Empty;
		// source task to process response on front-end
		public required string TaskName { get; set; }
		// Data json string
		public object? Data { get; set; }

		public ArraySegment<byte> ToBuffer()
		{
			return new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
		}

	}
}
