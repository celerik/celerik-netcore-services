using System.ComponentModel;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines the type of change a entity will undergo when changes
    /// are submitted to the database.
    /// </summary>
    public enum ApiChangeAction
    {
        /// <summary>
        /// The entity will be inserted.
        /// </summary>
        [Description("Insert")]
        Insert = 1,

        /// <summary>
        /// The entity will be updated.
        /// </summary>
        [Description("Update")]
        Update = 2,

        /// <summary>
        /// The entity will be deleted.
        /// </summary>
        [Description("Delete")]
        Delete = 3,
    }
}
