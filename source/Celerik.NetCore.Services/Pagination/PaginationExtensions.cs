using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Celerik.NetCore.Util;
using Microsoft.EntityFrameworkCore;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Adds some extension methods related to the Pagination functionality.
    /// </summary>
    public static class PaginationExtensions
    {
        /// <summary>
        /// Paginates this query according to request params.
        /// </summary>
        /// <typeparam name="TItem">The entity type in the query collection.</typeparam>
        /// <param name="query">Object against we are querying.</param>
        /// <param name="request">Object with the request arguments.</param>
        /// <returns>The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">Request is null.</exception>
        public static async Task<PaginationResult<TItem>> PaginateAsync<TItem>(
            this IQueryable<TItem> query,
            PaginationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(
                    UtilResources.Get("ArgumentCanNotBeNull", nameof(request)));

            int count;

            try
            {
                count = await query.CountAsync();
            }
            catch (InvalidOperationException)
            {
                count = query.Count();
            }

            if (!string.IsNullOrEmpty(request.SortKey))
                query = request.IsAscending
                    ? query.OrderBy(request.SortKey)
                    : query.OrderByDescending(request.SortKey);

            query = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);

            List<TItem> items;

            try
            {
                items = await query.ToListAsync();
            }
            catch (InvalidOperationException)
            {
                items = query.ToList();
            }

            var sorDirection = request.IsAscending
                ? SortDirectionType.Asc.GetDescription()
                : SortDirectionType.Desc.GetDescription();

            var pageCount = (int)Math.Ceiling(count / (double)request.PageSize);

            var result = new PaginationResult<TItem>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                SortKey = request.SortKey,
                SortDirection = sorDirection,
                Items = items,
                RecordCount = count,
                PageCount = pageCount
            };

            return result;
        }
    }
}
