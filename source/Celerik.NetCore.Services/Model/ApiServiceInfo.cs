namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Provides service information.
    /// </summary>
    public class ApiServiceInfo
    {
        /// <summary>
        /// The service name. E.g: "MasterDataSVC".
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The service version. E.g: "0.1.1"
        /// </summary>
        public string Version { get; set; }
    }
}
