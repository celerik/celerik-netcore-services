using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines the standard response for all services.
    /// </summary>
    /// <typeparam name="TData">Type of the Data property.
    /// </typeparam>
    /// <typeparam name="TStatusCode">Type of the StatusCode poperty.
    /// </typeparam>
    public class ApiResponse<TData, TStatusCode>
        where TData : class
        where TStatusCode : struct, IConvertible
    {
        /// <summary>
        /// Data sent in the response.
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// Indicates whether the service ran successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// An optional localized message describing the execution
        /// of the service.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Describes the type of message, null if there is no message.
        /// </summary>
        public ApiMessageType? MessageType { get; set; }

        /// <summary>
        /// The status code related to service execution (enumeration).
        /// </summary>
        public TStatusCode StatusCode { get; set; }

        /// <summary>
        /// Returns a JSON string that represents the current object.
        /// </summary>
        /// <returns>JSON string that represents the current object.</returns>
        public override string ToString()
            => JsonConvert.SerializeObject(this);

        /// <summary>
        /// Converts an ApiError&lt;TStatusCode&gt; into an
        /// ApiResponse&lt;TData, TStatusCode&gt;.
        /// </summary>
        /// <param name="error">The object to cast.</param>
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Implicit operators should not throw exceptions")]
        public static implicit operator ApiResponse<TData, TStatusCode>(ApiError<TStatusCode> error)
            => new ApiResponse<TData, TStatusCode>
            {
                Data = default,
                Message = error.Message,
                MessageType = ApiMessageType.Error,
                Success = false,
                StatusCode = error.StatusCode
            };
    }
}
