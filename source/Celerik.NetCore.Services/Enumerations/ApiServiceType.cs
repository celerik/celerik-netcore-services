using System.ComponentModel;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines possible implementations of the API.
    /// </summary>
    public enum ApiServiceType
    {
        /// <summary>
        /// The API is implemented using Entity Framework against a
        /// datasource.
        /// </summary>
        [Description("ServiceEF")]
        ServiceEF = 1,

        /// <summary>
        /// The API is implemented using a HttpClient.
        /// </summary>
        [Description("ServiceHttp")]
        ServiceHttp = 2,

        /// <summary>
        /// The API is implemented using mock data.
        /// </summary>
        [Description("ServiceMock")]
        ServiceMock = 3
    }
}
