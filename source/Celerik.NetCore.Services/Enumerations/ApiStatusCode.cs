using System.ComponentModel;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines common status codes that the API can send in the response.
    /// </summary>
    public enum ApiStatusCode
    {
        /// <summary>
        /// The process ran successfully.
        /// </summary>
        [Description("Ok")]
        Ok = 200,

        /// <summary>
        /// Something in the request payload is invalid.
        /// </summary>
        [Description("BadRequest")]
        BadRequest = 400,

        /// <summary>
        /// Some argument in the request payload has an invalid format.
        /// </summary>
        [Description("{0}Format")]
        Format = 460,

        /// <summary>
        /// There was an internal server error.
        /// </summary>
        [Description("Error")]
        Error = 500
    }
}
