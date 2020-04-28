namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines key conventions for the current configuration.
    /// </summary>
    public static class ApiConfigKeys
    {
        /// <summary>
        /// Key where the Service Type is defined.
        /// </summary>
        public const string ServiceType = "ServiceType";

        /// <summary>
        /// Key where the Logging Configuration is defined.
        /// </summary>
        public const string Logging = "Logging";

        /// <summary>
        /// Key where the Logging Provider is defined.
        /// </summary>
        public const string LoggingProvider = "Logging:Provider";

        /// <summary>
        /// Name of the SqlServer connection string.
        /// </summary>
        public const string SqlServerConnectionStringName = "SqlServer";

        /// <summary>
        /// Name of the AspNetIdentity connection string.
        /// </summary>
        public const string AspNetIdentityConnectionStringName = "AspNetIdentity";
    }
}
