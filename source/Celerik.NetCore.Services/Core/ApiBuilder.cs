using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Celerik.NetCore.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Builder to add core services to a service collection.
    /// </summary>
    /// <typeparam name="TLoggerCategory">The type who's name is used
    /// for the logger category name.</typeparam>
    /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
    public class ApiBuilder<TLoggerCategory, TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Reference to the current IServiceCollection instance.
        /// </summary>
        private readonly IServiceCollection _services;

        /// <summary>
        /// Reference to the current IConfiguration instance.
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// List containing the names of the methods already executed.
        /// </summary>
        private readonly List<string> _invokedMethods;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="services">Reference to the current IServiceCollection
        /// instance.</param>
        /// <param name="config">Reference to the current IConfiguration
        /// instance.</param>
        public ApiBuilder(IServiceCollection services, IConfiguration config)
        {
            _services = services;
            _config = config;
            _invokedMethods = new List<string>();
        }

        /// <summary>
        /// Gets the current service type.
        /// </summary>
        public ApiServiceType ServiceType => _config.GetServiceType();

        /// <summary>
        /// Adds services required for application localization.
        /// </summary>
        /// <returns>Reference to the current ApiBuilder.</returns>
        /// <param name="options">Provides programmatic configuration
        /// for localization.</param>
        /// <exception cref="InvalidOperationException">If this method
        /// was already called.</exception>
        internal ApiBuilder<TLoggerCategory, TDbContext> AddLocalization(
            LocalizationOptions options = null)
        {
            if (IsInvoked(nameof(AddLocalization)))
                throw new InvalidOperationException(
                    ServiceResources.Get("ApiBuilder.MethodAlreadyCalled", nameof(AddLocalization))
                );

            var jsonStringLocalizer = new JsonStringLocalizerFactory(options.ResourcesPath);
            _services.AddSingleton<IStringLocalizerFactory>(jsonStringLocalizer);

            _invokedMethods.Add(nameof(AddLocalization));
            return this;
        }

        /// <summary>
        /// Adds logging services to the current service collection.
        /// </summary>
        /// <param name="options">Provides programmatic configuration
        /// for the Console logger.</param>
        /// <returns>Reference to the current ApiBuilder.</returns>
        /// <exception cref="InvalidOperationException">If this method
        /// was already called.</exception>
        internal ApiBuilder<TLoggerCategory, TDbContext> AddLogging(
            ConsoleLoggerOptions options = null)
        {
            if (IsInvoked(nameof(AddLogging)))
                throw new InvalidOperationException(
                    ServiceResources.Get("ApiBuilder.MethodAlreadyCalled", nameof(AddLogging))
                );

            _services.AddLogging(builder =>
            {
                builder.ClearProviders();

                var logConfig = _config.GetSection(ApiConfigKeys.Logging);
                builder.AddConfiguration(logConfig);

                var providerString = _config[ApiConfigKeys.LoggingProvider]
                    ?? string.Empty;
                var providerTokens = providerString.Split(
                    ",", StringSplitOptions.RemoveEmptyEntries);
                var providerList = providerTokens.Select(
                    provider => provider.ToLowerInvariant().Trim());

                if (providerList.Contains(ApiLogProvider.Console.GetDescription()))
                    builder.AddConsole(opts =>
                    {
                        if (options != null)
                        {
                            opts.IncludeScopes = options.IncludeScopes;
                            opts.DisableColors = options.DisableColors;
                            opts.Format = options.Format;
                            opts.LogToStandardErrorThreshold = options.LogToStandardErrorThreshold;
                            opts.TimestampFormat = options.TimestampFormat;
                        }
                    });

                if (providerList.Contains(ApiLogProvider.Debug.GetDescription()))
                    builder.AddDebug();
                if (providerList.Contains(ApiLogProvider.EventSource.GetDescription()))
                    builder.AddEventSourceLogger();
            });

            _invokedMethods.Add(nameof(AddLogging));
            return this;
        }

        /// <summary>
        /// Adds a SqlServer DbContext to the current service collection
        /// only if the the SqlServer connection string or the AspNetIdentity
        /// connection string is defined in the configuration and the service
        /// type is ApiServiceType.ServiceEF.
        /// </summary>
        /// <exception cref="InvalidOperationException">If this method
        /// was already called.</exception>
        internal ApiBuilder<TLoggerCategory, TDbContext> AddSqlServer()
        {
            if (IsInvoked(nameof(AddSqlServer)))
                throw new InvalidOperationException(
                    ServiceResources.Get("ApiBuilder.MethodAlreadyCalled", nameof(AddSqlServer))
                );

            if (ServiceType == ApiServiceType.ServiceEF)
            {
                var sqlServerConnectionString = _config.GetConnectionString(
                    ApiConfigKeys.SqlServerConnectionStringName);
                var aspNetIdentityConnectionString = _config.GetConnectionString(
                    ApiConfigKeys.AspNetIdentityConnectionStringName);
                var connectionString = !string.IsNullOrEmpty(sqlServerConnectionString)
                    ? sqlServerConnectionString
                    : aspNetIdentityConnectionString;

                if (!string.IsNullOrEmpty(connectionString))
                    _services.AddDbContext<TDbContext>(
                        opts => opts.UseSqlServer(connectionString)
                    );
            }

            _invokedMethods.Add(nameof(AddSqlServer));
            return this;
        }

        /// <summary>
        /// Adds a Cosmos DbContext to the current service collection
        /// only if the the Cosmos connection string
        /// is defined in the configuration and the service
        /// type is ApiServiceType.ServiceEF.
        /// </summary>
        /// <exception cref="InvalidOperationException">If this method
        /// was already called.</exception>
        internal ApiBuilder<TLoggerCategory, TDbContext> AddCosmosDb()
        {
            if (IsInvoked(nameof(AddCosmosDb)))
                throw new InvalidOperationException(
                    ServiceResources.Get("ApiBuilder.MethodAlreadyCalled", nameof(AddCosmosDb))
                );

            if (ServiceType == ApiServiceType.ServiceEF)
            {
                var connectionString = _config.GetConnectionString(
                    ApiConfigKeys.CosmosDbConnectionStringName);

                var cosmosConectionString = new CosmosDBConnectionString(connectionString);
                var databaseName = _config.GetValue<string>(ApiConfigKeys.CosmosDbDatabaseName);

                if (!string.IsNullOrEmpty(connectionString))
                    _services.AddDbContext<TDbContext>(
                        opts => opts.UseCosmos(cosmosConectionString.ServiceEndpoint, cosmosConectionString.AuthKey, databaseName)
                    );
            }

            _invokedMethods.Add(nameof(AddCosmosDb));
            return this;
        }

        /// <summary>
        /// Adds AspNetIdentity services to the current service collection
        /// only if the AspNetIdentity connection string is defined in the
        /// configuration and the service type is ApiServiceType.ServiceEF.
        /// </summary>
        /// <param name="options">Represents all the options you can use to
        /// configure the identity system.</param>
        /// <returns>Reference to the current ApiBuilder.</returns>
        /// <exception cref="InvalidOperationException">If this method
        /// was already called.</exception>
        public ApiBuilder<TLoggerCategory, TDbContext> AddIdentity(
            IdentityOptions options = null)
        {
            if (IsInvoked(nameof(AddIdentity)))
                throw new InvalidOperationException(
                    ServiceResources.Get("ApiBuilder.MethodAlreadyCalled", nameof(AddIdentity))
                );

            var aspNetIdentityConnectionString = _config.GetConnectionString(
                ApiConfigKeys.AspNetIdentityConnectionStringName);
            var shouldAddIdenity = ServiceType == ApiServiceType.ServiceEF
                && !string.IsNullOrEmpty(aspNetIdentityConnectionString);

            if (shouldAddIdenity)
            {
                _services.AddDefaultIdentity<IdentityUser>(opts => {
                    if (options?.ClaimsIdentity != null)
                    {
                        opts.ClaimsIdentity.RoleClaimType = options.ClaimsIdentity.RoleClaimType;
                        opts.ClaimsIdentity.UserNameClaimType = options.ClaimsIdentity.UserNameClaimType;
                        opts.ClaimsIdentity.UserIdClaimType = options.ClaimsIdentity.UserIdClaimType;
                        opts.ClaimsIdentity.SecurityStampClaimType = options.ClaimsIdentity.SecurityStampClaimType;
                    }
                    if (options?.User != null)
                    {
                        opts.User.AllowedUserNameCharacters = options.User.AllowedUserNameCharacters;
                        opts.User.RequireUniqueEmail = options.User.RequireUniqueEmail;
                    }
                    if (options?.Password != null)
                    {
                        opts.Password.RequiredLength = options.Password.RequiredLength;
                        opts.Password.RequiredUniqueChars = options.Password.RequiredUniqueChars;
                        opts.Password.RequireNonAlphanumeric = options.Password.RequireNonAlphanumeric;
                        opts.Password.RequireLowercase = options.Password.RequireLowercase;
                        opts.Password.RequireUppercase = options.Password.RequireUppercase;
                        opts.Password.RequireDigit = options.Password.RequireDigit;
                    }
                    if (options?.Lockout != null)
                    {
                        opts.Lockout.AllowedForNewUsers = options.Lockout.AllowedForNewUsers;
                        opts.Lockout.MaxFailedAccessAttempts = options.Lockout.MaxFailedAccessAttempts;
                        opts.Lockout.DefaultLockoutTimeSpan = options.Lockout.DefaultLockoutTimeSpan;
                    }
                    if (options?.SignIn != null)
                    {
                        opts.SignIn.RequireConfirmedEmail = options.SignIn.RequireConfirmedEmail;
                        opts.SignIn.RequireConfirmedPhoneNumber = options.SignIn.RequireConfirmedPhoneNumber;
                        opts.SignIn.RequireConfirmedAccount = options.SignIn.RequireConfirmedAccount;
                    }
                    if (options?.Tokens != null)
                    {
                        opts.Tokens.ProviderMap = options.Tokens.ProviderMap;
                        opts.Tokens.EmailConfirmationTokenProvider = options.Tokens.EmailConfirmationTokenProvider;
                        opts.Tokens.PasswordResetTokenProvider = options.Tokens.PasswordResetTokenProvider;
                        opts.Tokens.ChangeEmailTokenProvider = options.Tokens.ChangeEmailTokenProvider;
                        opts.Tokens.ChangePhoneNumberTokenProvider = options.Tokens.ChangePhoneNumberTokenProvider;
                        opts.Tokens.AuthenticatorTokenProvider = options.Tokens.AuthenticatorTokenProvider;
                        opts.Tokens.AuthenticatorIssuer = options.Tokens.AuthenticatorIssuer;
                    }
                    if (options?.Stores != null)
                    {
                        opts.Stores.MaxLengthForKeys = options.Stores.MaxLengthForKeys;
                        opts.Stores.ProtectPersonalData = options.Stores.ProtectPersonalData;
                    }
                })
                .AddEntityFrameworkStores<TDbContext>();
            }

            _invokedMethods.Add(nameof(AddIdentity));
            return this;
        }

        /// <summary>
        /// Adds automapper to the current service collection.
        /// </summary>
        /// <param name="configure">The configure callback.</param>
        /// <returns>Reference to the current ApiBuilder.</returns>
        /// <exception cref="InvalidOperationException">If this method
        /// was already called.</exception>
        public ApiBuilder<TLoggerCategory, TDbContext> AddAutomapper(
            Action<IMapperConfigurationExpression> configure = null)
        {
            if (IsInvoked(nameof(AddAutomapper)))
                throw new InvalidOperationException(
                    ServiceResources.Get("ApiBuilder.MethodAlreadyCalled", nameof(AddAutomapper))
                );

            var config = new MapperConfiguration(options =>
            {
                configure?.Invoke(options);
            });

            var mapper = config.CreateMapper();
            _services.AddSingleton(mapper);

            _invokedMethods.Add(nameof(AddAutomapper));
            return this;
        }

        /// <summary>
        /// Adds fluent validation to the current service collection.
        /// </summary>
        /// <param name="configure">The configure callback.</param>
        /// <returns>Reference to the current ApiBuilder.</returns>
        /// <exception cref="InvalidOperationException">If this method
        /// was already called.</exception>
        /// <exception cref="ArgumentNullException">Configure is null.
        /// </exception>
        public ApiBuilder<TLoggerCategory, TDbContext> AddValidators(
            Action configure)
        {
            if (IsInvoked(nameof(AddValidators)))
                throw new InvalidOperationException(
                    ServiceResources.Get("ApiBuilder.MethodAlreadyCalled", nameof(AddValidators))
                );

            if (configure == null)
                throw new ArgumentNullException(
                    UtilResources.Get("ArgumentCanNotBeNull", nameof(configure))
                );

            _services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            configure.Invoke();

            _invokedMethods.Add(nameof(AddValidators));
            return this;
        }

        /// <summary>
        /// Adds business services to the current service collection.
        /// </summary>
        /// <param name="configure">The configure callback.</param>
        /// <returns>Reference to the current ApiBuilder.</returns>
        /// <exception cref="InvalidOperationException">If this method
        /// was already called.</exception>
        /// <exception cref="ArgumentNullException">Configure is null.
        /// </exception>
        public ApiBuilder<TLoggerCategory, TDbContext> AddBusinesServices(
            Action<IConfiguration> configure)
        {
            if (IsInvoked(nameof(AddBusinesServices)))
                throw new InvalidOperationException(
                    ServiceResources.Get("ApiBuilder.MethodAlreadyCalled", nameof(AddBusinesServices))
                );

            if (configure == null)
                throw new ArgumentNullException(
                    UtilResources.Get("ArgumentCanNotBeNull", nameof(configure))
                );

            switch (ServiceType)
            {
                case ApiServiceType.ServiceHttp:
                case ApiServiceType.ServiceMock:
                    _services.AddTransient<ApiServiceArgs<TLoggerCategory>>();
                    break;
                case ApiServiceType.ServiceEF:
                    _services.AddTransient<ApiServiceArgsEF<TLoggerCategory, TDbContext>>();
                    break;
            }

            configure.Invoke(_config);

            _invokedMethods.Add(nameof(AddBusinesServices));
            return this;
        }

        /// <summary>
        /// Indicates whether the passed-in method name was already
        /// executed in this builder.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <returns>True if the passed-in method name was already
        /// executed in this builder.</returns>
        private bool IsInvoked(string methodName)
            => _invokedMethods.Contains(methodName);
    }
}
