using System.ComponentModel;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Defines the possible types of sorting.
    /// </summary>
    public enum SortDirectionType
    {
        /// <summary>
        /// Sorting ascending.
        /// </summary>
        [Description("asc")]
        Asc = 1,

        /// <summary>
        /// Sorting descending.
        /// </summary>
        [Description("desc")]
        Desc = 2
    }
}
