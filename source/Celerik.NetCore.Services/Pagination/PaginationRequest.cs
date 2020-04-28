using Celerik.NetCore.Util;
using Newtonsoft.Json;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Payload base class for all requests implementing pagination.
    /// </summary>
    public class PaginationRequest
    {
        /// <summary>
        /// The page number, starting from 1 for the first page. By default: 1.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of records per page. By default: int.MaxValue.
        /// </summary>
        public int PageSize { get; set; } = int.MaxValue;

        /// <summary>
        /// The sort key.
        /// </summary>
        public string SortKey { get; set; }

        /// <summary>
        /// The sort direction, specify "asc" or "desc".
        /// </summary>
        public string SortDirection { get; set; }

        /// <summary>
        /// Indicates whether sort is ascending.
        /// </summary>
        [JsonIgnore]
        public bool IsAscending =>
            SortDirection?.ToLowerInvariant() !=
            SortDirectionType.Desc.GetDescription().ToLowerInvariant();
    }

    /// <summary>
    /// Payload base class for all requests implementing pagination.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity against we are
    /// sorting.</typeparam>
    public class PaginationRequest<TEntity> : PaginationRequest
    {
    }
}
