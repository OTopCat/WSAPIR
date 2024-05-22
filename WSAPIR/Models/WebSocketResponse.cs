using System.Text;
using Newtonsoft.Json;

namespace WSAPIR.Models
{

    /// <summary>
    /// response obj
    /// </summary>
    public class WebSocketResponse
    {
        /// <summary>
        /// sender API
        /// </summary>
        public string SourceAPI { get; set; } = string.Empty;

        /// <summary>
        /// source task to process response on front-end
        /// </summary>
        public required string TaskName { get; set; }

        /// <summary>
        /// Data json string
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// transformation to byte array for transfer over WS
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> ToBuffer()
        {
            return new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
        }

    }
}
