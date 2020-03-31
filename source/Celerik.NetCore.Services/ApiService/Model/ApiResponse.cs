using Celerik.NetCore.Util;
using Newtonsoft.Json;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines the standarized response for all services.
    /// </summary>
    /// <typeparam name="TData">Type of the Data property.</typeparam>
    public class ApiResponse<TData>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="data">Data sent as the response.</param>
        public ApiResponse(TData data = default)
        {
            Data = data;
        }

        /// <summary>
        /// Data sent as the response.
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// Optional message in case something happened during the service execution.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Describes the type of message.
        /// </summary>
        [JsonConverter(typeof(EnumDescriptionJsonConverter))]
        public ApiMessageType? MessageType { get; set; }

        /// <summary>
        /// Indicates whether the service was successfully executed.
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Returns a JSON string that represents the current object.
        /// </summary>
        /// <returns>JSON string that represents the current object.</returns>
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
