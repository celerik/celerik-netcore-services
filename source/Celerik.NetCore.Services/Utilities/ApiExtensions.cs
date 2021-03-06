﻿using System;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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
        /// Adds core services to the passed-in service collection.
        /// </summary>
        /// <typeparam name="TLoggerCategory">The type who's name is used
        /// for the logger category name.</typeparam>
        /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
        /// <param name="services">The IServiceCollection to add core services
        /// to.</param>
        /// <param name="config">Reference to the current IConfiguration
        /// instance.</param>
        /// <param name="apiConfig">Defines the configuration needed to
        /// add core services.</param>
        /// <exception cref="ConfigException">If there was defined a
        /// SqlServerConnectionStringKey but the connection string name is not
        /// found either in the environment variables or in the config file.
        /// </exception>
        public static ApiBuilder<TLoggerCategory, TDbContext> AddCoreServices<TLoggerCategory, TDbContext>(
            this IServiceCollection services,
            IConfiguration config,
            ApiConfig apiConfig = null)
                where TDbContext : DbContext
        {
            apiConfig ??= new ApiConfig();

            return new ApiBuilder<TLoggerCategory, TDbContext>(services, config)
                .AddLocalization(apiConfig.LocalizationOptions)
                .AddLogging(apiConfig.ConsoleLoggerOptions)
                .AddSqlServer()
                .AddIdentity(apiConfig.IdentityOptions)
                .AddCosmosDb();
        }

        /// <summary>
        /// Adds core services to the passed-in service collection.
        /// </summary>
        /// <typeparam name="TLoggerCategory">The type who's name is used
        /// for the logger category name.</typeparam>
        /// <param name="services">The IServiceCollection to add core services
        /// to.</param>
        /// <param name="config">Reference to the current IConfiguration
        /// instance.</param>
        /// <param name="apiConfig">Defines the configuration needed to
        /// add core services.</param>
        public static ApiBuilder<TLoggerCategory, DbContext> AddCoreServices<TLoggerCategory>(
            this IServiceCollection services,
            IConfiguration config,
            ApiConfig apiConfig = null)
        {
            apiConfig ??= new ApiConfig();

            return new ApiBuilder<TLoggerCategory, DbContext>(services, config)
                .AddLocalization(apiConfig.LocalizationOptions)
                .AddLogging(apiConfig.ConsoleLoggerOptions);
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
        /// <exception cref="ArgumentNullException">Config is null.</exception>
        /// <exception cref="ConfigException">If the ServiceType is not
        /// present in the config object, or if the value is invalid.</exception>
        public static ApiServiceType GetServiceType(this IConfiguration config)
            => config.ReadEnum<ApiServiceType>(ApiConfigKeys.ServiceType);

        /// <summary>
        /// Adds a Fluent Validator to this service collection.
        /// </summary>
        /// <typeparam name="TPayload">The type of the payload to
        /// be validated.</typeparam>
        /// <typeparam name="TValidator">The type of the validator.
        /// </typeparam>
        /// <param name="services">The services where add the validator
        /// to.</param>
        public static void AddValidator<TPayload, TValidator>(
            this IServiceCollection services)
                where TPayload : class
                where TValidator : AbstractValidator<TPayload>
        {
            services.AddSingleton<IValidator<TPayload>, TValidator>();
        }
        /*
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
        /// <exception cref="ArgumentNullException">Pagination is null.</exception>
        public static TEntity FirstOrDefault<TEntity>(this ApiResponse<PaginationResult<TEntity>> pagination)
        {
            if (pagination == null)
                throw new ArgumentNullException(
                    UtilResources.Get("ArgumentCanNotBeNull", nameof(pagination)));

            var first = pagination.Data.RecordCount > 0
                ? pagination.Data.Items.ElementAt(0)
                : default;

            return first;
        }*/
    }
}
