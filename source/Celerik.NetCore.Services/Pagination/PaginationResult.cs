using System.Collections.Generic;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Response base class for all requests implementing pagination.
    /// </summary>
    /// <typeparam name="TItem">The entity type in the "Items" collection.</typeparam>
    public class PaginationResult<TItem> : PaginationRequest
    {
        /// <summary>
        /// The total record count.
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// The total page count.
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// List of items including in the result according to the page size.
        /// </summary>
        public IEnumerable<TItem> Items { get; set; }
    }
}
