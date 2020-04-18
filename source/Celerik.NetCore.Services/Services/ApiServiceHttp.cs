using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Base class for all services using a HttpClient.
    /// </summary>
    public abstract class ApiServiceHttp : IDisposable
    {
        /// <summary>
        /// Indicates wheter Dispose() was already called.
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// The name of the controller this service points to.
        /// </summary>
        private readonly string _controller = null;

        /// <summary>
        /// Object for sending HTTP requests and receiving HTTP
        /// responses.
        /// </summary>
        private readonly HttpClient _httpClient = null;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="baseAddress">Base address of Uniform
        /// Resource Identifier (URI) of the Internet resource used
        /// when sending requests.</param>
        /// <param name="controller">The name of the controller this
        /// service points to.</param>
        public ApiServiceHttp(string baseAddress, string controller)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
            _controller = controller;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates whether it is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                _httpClient.Dispose();

            _isDisposed = true;
        }

        /// <summary>
        /// Calls an endpint with the passed-in Method and Payload.
        /// </summary>
        /// <typeparam name="TOutput">The type of the Output response.
        /// </typeparam>
        /// <param name="method">The Http Method.</param>
        /// <param name="endpoint">The endpoint name.</param>
        /// <param name="payload">Arguments of the service.</param>
        /// <returns>Service response casted as a TOutput.</returns>
        protected async Task<ApiResponse<TOutput>> Call<TOutput>(
            HttpMethod method, string endpoint, object payload = null)
        {
            var url = GetUrl(method, endpoint, payload);
            using var request = new HttpRequestMessage(method, url);

            if (method != HttpMethod.Get &&
                method != HttpMethod.Delete &&
                payload != null)
            {
                var json = JsonConvert.SerializeObject(payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var obj = JsonConvert.DeserializeObject<ApiResponse<TOutput>>(content);
                return obj;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (IsInvalidModelState(content))
                    content = GetFirstMessage(content);

                return new ApiResponse<TOutput>
                {
                    Data = default,
                    Message = content,
                    MessageType = ApiMessageType.Error,
                    Success = false
                };
            }

            throw new HttpRequestException(
                ServiceResources.Get("ApiServiceHttp.Call.Error",
                request.RequestUri.ToString(),
                response.StatusCode,
                content)
            );
        }

        /// <summary>
        /// Builds the Url for calling a service based on the Method, Endpoint
        /// and Payload received as argument.
        /// </summary>
        /// <param name="method">The Http Method.</param>
        /// <param name="endpoint">The endpoint name.</param>
        /// <param name="payload">Arguments of the service.</param>
        /// <returns>Url to call the service based on the Method, Endpoint
        /// and Payload received as argument.</returns>
        private string GetUrl(
            HttpMethod method, string endpoint, object payload = null)
        {
            var url = $"{_controller}/{endpoint}";

            if ((method == HttpMethod.Get || method == HttpMethod.Delete) &&
                payload != null)
            {
                var builder = HttpUtility.ParseQueryString(string.Empty);
                var props = payload.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in props)
                {
                    var value = prop.GetValue(payload);
                    if (value != null)
                        builder[prop.Name] = value.ToString();
                }

                var query = builder.ToString();
                url = $"{url}?{query}";
            }

            return url;
        }

        /// <summary>
        /// Indicates if a service response is related to an Invalid
        /// Model State.
        /// </summary>
        /// <param name="response">Response received from the service.
        /// </param>
        /// <returns>True if the service response is related to an
        /// Invalid Model State.</returns>
        private static bool IsInvalidModelState(string response)
        {
            return !string.IsNullOrEmpty(response) &&
                response.Contains("\"errors\"") &&
                response.Contains("\"type\"") &&
                response.Contains("\"title\"") &&
                response.Contains("\"status\"") &&
                response.Contains("\"traceId\"");
        }


        /// <summary>
        /// Gets the first error message contained into the service
        /// response received as argument.
        /// </summary>
        /// <param name="response">Response received from the service.
        /// </param>
        /// <returns>First error message contained into the service
        /// response received as argument.</returns>
        private static string GetFirstMessage(string response)
        {
            try
            {
                var firstMessage = response.Substring(
                    response.IndexOf("[\"", StringComparison.InvariantCulture) + 2);
                firstMessage = firstMessage.Substring(0,
                    firstMessage.IndexOf("\"", StringComparison.InvariantCulture) - 1);

                return firstMessage;
            }
            catch (ArgumentOutOfRangeException)
            {
                return response;
            }
        }
    }
}
