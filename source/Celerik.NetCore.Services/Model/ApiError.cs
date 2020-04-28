using System;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines the standard response for all errors.
    /// </summary>
    /// <typeparam name="TStatusCode">Type of the StatusCode property.
    /// </typeparam>
    public class ApiError<TStatusCode>
         where TStatusCode : struct, IConvertible
    {
        /// <summary>
        /// A localized string describing the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The status code related to this error (enumeration).
        /// </summary>
        public TStatusCode StatusCode { get; set; }
    }
}
