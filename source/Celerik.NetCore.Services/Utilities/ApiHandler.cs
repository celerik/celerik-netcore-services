using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Handler to configure the ApiService. Specifically, we configure
    /// the following:
    ///     - Localization
    ///     - Logging
    ///     - Service Arguments
    /// </summary>
    /// <example>
    ///     <code>
    ///         // Call this handler from the Startup.cs class:
    ///         public void ConfigureServices(IServiceCollection services)
    ///         {
    ///             ...
    ///             services.AddServiceArguments&lt;TDbContext&gt;();
    ///         }
    ///     </code>
    /// </example>
    public static class ApiHandler
    {
        /// <summary>
        /// Adds base services to the passed-in service collection, including
        /// Localization and Login.
        /// </summary>
        /// <param name="services">The IServiceCollection to add base services
        /// to.</param>
        public static void AddBaseServices(this IServiceCollection services)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddConsole(console =>
                {
                    console.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                });
                configure.AddDebug();
            });
        }

        /// <summary>
        /// Adds service arguments to this service collection.
        /// </summary>
        /// <typeparam name="TLoggerCategory">The type who's name is used
        /// for the logger category name.</typeparam>
        /// <typeparam name="TDbContext">The type of DbContext</typeparam>
        /// <param name="services">The IServiceCollection to add service
        /// arguments to.</param>
        public static void AddServiceArguments<TLoggerCategory, TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            var serviceType = services.GetServiceType();

            switch (serviceType)
            {
                case ApiServiceType.ServiceEF:
                    services.AddTransient<ApiServiceArgsEF<TLoggerCategory, TDbContext>>();
                    break;
                case ApiServiceType.ServiceMock:
                    services.AddTransient<ApiServiceArgs<TLoggerCategory>>();
                    break;
                default:
                    return;
            }
        }
    }
}
