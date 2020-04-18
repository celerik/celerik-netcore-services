using System;
using System.Linq;
using Celerik.NetCore.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Adds some extension methods related to the ApiService functionality.
    /// </summary>
    public static class ApiExtensions
    {
        /// <summary>
        /// Gets the first element contained into the Data.Items property of the passed-in
        /// PaginationResult object.
        /// </summary>
        /// <typeparam name="TEntity">The type of each element into the Data.Items collection.
        /// </typeparam>
        /// <param name="pagination">The pagination object where we get the first element.
        /// </param>
        /// <returns>First element contained into the Data.Items property of the passed-in
        /// PaginationResult object, or null if the collection doesn´t have elements.</returns>
        public static TEntity FirstOrDefault<TEntity>(this ApiResponse<PaginationResult<TEntity>> pagination)
        {
            if (pagination == null)
                throw new ArgumentException(
                    UtilResources.Get("Common.ArgumentCanNotBeNull", nameof(pagination)));

            var first = pagination.Data.RecordCount > 0
                ? pagination.Data.Items.ElementAt(0)
                : default;

            return first;
        }

        /// <summary>
        /// Gets the ApiServiceType configured into the passed-in IConfiguration object.
        /// 
        /// The following are the supported service types:
        ///     - ServiceEF: services implementing EntityFramework data access.
        ///     - ServiceHttp: services using a HttpClient.
        ///     - ServiceMock: services implementing Mock data access.
        /// 
        /// By convention, the service type is retrieved from the IConfiguration object
        /// from the property: "ServiceType".
        /// </summary>
        /// <param name="config">The configuration object where we get the ApiServiceType.
        /// </param>
        /// <returns>ApiServiceType stored into the passed-in IConfiguration object.</returns>
        public static ApiServiceType GetServiceType(this IConfiguration config)
        {
            if (config == null)
                throw new ArgumentException(
                    UtilResources.Get("Common.ArgumentCanNotBeNull", nameof(config)));

            var key = "ServiceType";
            var value = config[key];
            var type = EnumUtility.GetValueFromDescription<ApiServiceType>(value);

            if (string.IsNullOrEmpty(value))
                throw new ConfigException(
                    ServiceResources.Get("Common.MissingValue", key));
            if (type == 0)
                throw new ConfigException(
                    ServiceResources.Get("Common.InvalidValue", key, value));

            return type;
        }

        /// <summary>
        /// Gets the ApiServiceType configured into the passed-in IServiceCollection object.
        /// 
        /// The following are the supported service types:
        ///     - ServiceEF: services implementing EntityFramework data access.
        ///     - ServiceHttp: services using a HttpClient.
        ///     - ServiceMock: services implementing Mock data access.
        /// 
        /// By convention, the service type is retrieved from the IConfiguration object
        /// from the property: "ServiceType".
        /// </summary>
        /// <param name="services">The services where we get the ApiServiceType.
        /// </param>
        /// <returns>ApiServiceType stored into the passed-in IServiceCollection object.</returns>
        public static ApiServiceType GetServiceType(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var config = provider.GetRequiredService<IConfiguration>();
            var type = config.GetServiceType();

            return type;
        }
    }
}
