using System.ComponentModel;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines the possible types of messages that the API can
    /// send in the response.
    /// </summary>
    public enum ApiMessageType
    {
        /// <summary>
        /// The process ran successfully and some contextual
        /// information is sent.
        /// </summary>
        [Description("info")]
        Info = 1,

        /// <summary>
        /// The process ran successfully.
        /// </summary>
        [Description("success")]
        Success = 2,

        /// <summary>
        /// The process ran successfully but there is a warning
        /// message.
        /// </summary>
        [Description("warning")]
        Warning = 3,

        /// <summary>
        /// There was an error executing the process.
        /// </summary>
        [Description("error")]
        Error = 4
    }

    /// <summary>
    /// Defines the possible types of operation that the API can execute.
    /// </summary>
    public enum ApiOperationType
    {
        /// <summary>
        /// A read operation that does not alter any data.
        /// </summary>
        [Description("Read")]
        Read = 1,

        /// <summary>
        /// An operation that inserts a single record.
        /// </summary>
        [Description("Insert")]
        Insert = 2,

        /// <summary>
        /// An operation that inserts several records.
        /// </summary>
        [Description("Bulk Insert")]
        BulkInsert = 3,

        /// <summary>
        /// An operation that updates a single record.
        /// </summary>
        [Description("Update")]
        Update = 4,

        /// <summary>
        /// An operation that updates several records.
        /// </summary>
        [Description("Bulk Update")]
        BulkUpdate = 5,

        /// <summary>
        /// An operation that deletes a single record.
        /// </summary>
        [Description("Delete")]
        Delete = 6,

        /// <summary>
        /// An operation that deletes several records.
        /// </summary>
        [Description("Bulk Delete")]
        BulkDelete = 7
    }

    /// <summary>
    /// Defines possible implementations of the service.
    /// </summary>
    public enum ApiServiceType
    {
        /// <summary>
        /// The service is implemented using Entity Framework against a
        /// datasource.
        /// </summary>
        [Description("ServiceEF")]
        ServiceEF = 1,

        /// <summary>
        /// The service is implemented using mock data.
        /// </summary>
        [Description("ServiceMock")]
        ServiceMock = 2
    }
}
