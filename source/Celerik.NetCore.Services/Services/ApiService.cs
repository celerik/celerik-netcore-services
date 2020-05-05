using System;
using System.Diagnostics;
using System.Globalization;
using AutoMapper;
using Celerik.NetCore.Util;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Base class for all services.
    /// </summary>
    /// <typeparam name="TLoggerCategory">The type who's name is used
    /// for the logger category name.</typeparam>
    /// <typeparam name="TResources">The type used to localize string
    /// resources.</typeparam>
    public abstract class ApiService<TLoggerCategory, TResources> : IDisposable
    {
        /// <summary>
        /// Indicates wheter Dispose() was already called.
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// Object to measure time executions.
        /// </summary>
        private Stopwatch _stopWatch = null;

        /// <summary>
        /// Reference to te current IHttpContextAccessor instance.
        /// </summary>
        private IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Reference to the current IStringLocalizer instance.
        /// </summary>
        private IStringLocalizer _stringLocalizer;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="args">Encapsulates the properties to initialize a new
        /// ApiServiceArgs&lt;TLoggerCategory&gt;.</param>
        /// <exception cref="ArgumentNullException">Args is null.</exception>
        public ApiService(ApiServiceArgs<TLoggerCategory> args)
        {
            if (args == null)
                throw new ArgumentNullException(
                    UtilResources.Get("ArgumentCanNotBeNull", nameof(args)));

            _stopWatch = new Stopwatch();
            _httpContextAccessor = args.HttpContextAccessor;

            ServiceProvider = args.ServiceProvider;
            Config = args.Config;
            Logger = args.Logger;
            Mapper = args.Mapper;

            UtilResources.Initialize(args.StringLocalizerFactory);
            _stringLocalizer = UtilResources.Factory.Create(typeof(TResources));
        }

        /// <summary>
        /// Reference to the current IServiceProvider instance.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Reference to the current IConfiguration instance.
        /// </summary>
        protected IConfiguration Config { get; private set; }

        /// <summary>
        /// Reference to the current ILogger instance.
        /// </summary>
        protected ILogger<TLoggerCategory> Logger { get; private set; }

        /// <summary>
        /// Reference to the current IMapper instance.
        /// </summary>
        protected IMapper Mapper { get; private set; }

        /// <summary>
        /// Gets a reference to the current HttpContext.
        /// </summary>
        protected HttpContext HttpContext => _httpContextAccessor?.HttpContext;

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates whether it is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            var typeName = GetType().Name;
            Logger.LogDebug(LocalizeString("ApiService.Dispose.Disposing", typeName));

            if (disposing)
            {
                _stopWatch = null;
                _httpContextAccessor = null;

                ServiceProvider = null;
                Config = null;
                Logger = null;
                Mapper = null;
            }

            _isDisposed = true;
        }

        /// <summary>
        /// Starts logging an action.
        /// </summary>
        protected void StartLog()
        {
            _stopWatch.Start();
            var callerMethodName = new StackTrace(1).GetMethodName();
            Logger.LogDebug(LocalizeString("ApiService.StartLog.Start", callerMethodName));
        }

        /// <summary>
        /// Stops logging an action.
        /// </summary>
        /// <param name="message">Optional message to be logged.</param>
        protected void EndLog(string message = null)
        {
            _stopWatch.Stop();

            var callerMethodName = new StackTrace(1).GetMethodName();
            var totalSeconds = _stopWatch.Elapsed.TotalSeconds;

            Logger.LogDebug(LocalizeString("ApiService.EndLog.End", callerMethodName));
            Logger.LogDebug(LocalizeString("ApiService.EndLog.TotalSeconds", totalSeconds));

            if (!string.IsNullOrEmpty(message))
                Logger.LogDebug(LocalizeString(message));
        }

        /// <summary>
        /// Localizes the string resource with the given name.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <returns>Localized string resource.</returns>
        protected string LocalizeString(string name)
            => _stringLocalizer?[name].Value
                ?? ServiceResources.Get(name);

        /// <summary>
        /// Localizes the string resource with the given name and formatted with
        /// the supplied arguments.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <param name="arguments">The values to format the string with.</param>
        /// <returns>The formatted string resource.</returns>
        public string LocalizeString(string name, params object[] arguments)
            => _stringLocalizer?[name, arguments].Value
                ?? ServiceResources.Get(name, arguments);

        /// <summary>
        /// Validates the passed-in payload using FluentValidation.
        /// </summary>
        /// <typeparam name="TPayload">The type of the payload object.
        /// </typeparam>
        /// <param name="payload">The object to be validated.</param>
        /// <param name="message">Message describing error, null if
        /// the payload is valid.</param>
        /// <param name="property">Name of the invalid property, null if
        /// the payload is valid or if the payload is null.</param>
        /// <returns>True if the payload is valid.</returns>
        protected bool Validate<TPayload>(
            TPayload payload, out string message, out string property)
        {
            message = null;
            property = null;

            if (payload == null)
                message = LocalizeString("ArgumentCanNotBeNull", nameof(payload));
            else
            {
                var validator = ServiceProvider.GetRequiredService<IValidator<TPayload>>();
                var result = validator.Validate(payload);

                if (!result.IsValid)
                {
                    var firstError = result.Errors[0];
                    message = firstError.ErrorMessage;
                    property = firstError.PropertyName;
                }
            }

            return message == null;
        }

        /// <summary>
        /// Creates an Error response based on the passed-in localized
        /// Message and Property.
        /// </summary>
        /// <typeparam name="TStatusCode">Type of the StatusCode property
        /// in the ApiError object.</typeparam>
        /// <param name="message">Localized Message describing the error.
        /// </param>
        /// <param name="property">Name of the invalid property, null if
        /// the payload is null.</param>
        /// <returns>Error response based on the passed-in localized
        /// Message and Property.</returns>
        protected ApiError<TStatusCode> Error<TStatusCode>(
            string message, string property)
            where TStatusCode : struct, IConvertible
        {
            var statusCode = property == null
                ? ApiStatusCode.BadRequest.ToString()
                : string.Format(CultureInfo.InvariantCulture, ApiStatusCode.Format.GetDescription(), property);

            var apiError = new ApiError<TStatusCode>
            {
                Message = LocalizeString(message),
                StatusCode = EnumUtility.GetValueFromDescription<TStatusCode>(statusCode)
            };

            Logger.LogWarning(apiError.Message);

            return apiError;
        }

        /// <summary>
        /// Creates an Error response based on the passed-in statusCode.
        /// </summary>
        /// <typeparam name="TStatusCode">Type of the StatusCode property
        /// in the ApiError object.</typeparam>
        /// <param name="statusCode">The status code</param>
        /// <returns>Error response based on the passed-in statusCode.</returns>
        protected ApiError<TStatusCode> Error<TStatusCode>(TStatusCode statusCode)
            where TStatusCode : struct, IConvertible
        {
            var apiError = new ApiError<TStatusCode>
            {
                Message = LocalizeString(statusCode.ToString()),
                StatusCode = statusCode
            };

            Logger.LogWarning(apiError.Message);

            return apiError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TStatusCode"></typeparam>
        /// <param name="data"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected ApiResponse<TData, TStatusCode> Ok<TData, TStatusCode>(object data, TStatusCode statusCode)
            where TData : class
            where TStatusCode : struct, IConvertible
        {
            var response = new ApiResponse<TData, TStatusCode>
            {
                Data = Mapper.Map<TData>(data),
                Success = true,
                Message = LocalizeString(statusCode.ToString()),
                MessageType = ApiMessageType.Success,
                StatusCode = statusCode
            };

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TStatusCode"></typeparam>
        /// <param name="response"></param>
        /// <param name="successMessageCode"></param>
        protected void ProcessResponse<TData, TStatusCode>(
            ApiResponse<TData, TStatusCode> response,
            TStatusCode successMessageCode)
            where TData : class
            where TStatusCode : struct, IConvertible
        {
            if (response?.Data != null)
            {
                response.Success = true;
                response.Message = LocalizeString(successMessageCode.ToString());
                response.MessageType = ApiMessageType.Success;
                response.StatusCode = successMessageCode;
            }
            else
            {
                response.Success = false;
                response.MessageType = ApiMessageType.Error;

                if (response.Message == null)
                    response.Message = LocalizeString(response.StatusCode.ToString());
            }
        }
        /*
        /// <summary>
        /// Paginates this query according to the passed-in request params.
        /// </summary>
        /// <typeparam name="TRequest">The entity request type.</typeparam>
        /// <typeparam name="TResult">The entity result type.</typeparam>
        /// <param name="query">Object against we are querying.</param>
        /// <param name="request">Object with request arguments.</param>
        /// <returns>The task object representing the asynchronous operation.
        /// </returns>
        public async Task<ApiResponse<PaginationResult<TResult>>> PaginateAsync<TRequest, TResult>(
            IQueryable<TRequest> query,
            PaginationRequest request)
        {
            var pagination = await query.PaginateAsync(request);

            var castedPagination = new PaginationResult<TResult>
            {
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                SortKey = pagination.SortKey,
                SortDirection = pagination.SortDirection,
                Items = Mapper.Map<IEnumerable<TResult>>(pagination.Items),
                RecordCount = pagination.RecordCount,
                PageCount = pagination.PageCount
            };

            ApiResponse<PaginationResult<TResult>> response;

            if (castedPagination.RecordCount == 0)
                response = new ApiResponse<PaginationResult<TResult>>
                {
                    Data = castedPagination,
                    Message = ServiceResources.Get("Common.NoRecordsFound"),
                    MessageType = ApiMessageType.Info,
                    Success = true
                };
            else
                response = Ok(castedPagination);

            return response;
        }*/
    }
}
