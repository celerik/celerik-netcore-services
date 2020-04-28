using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Celerik.NetCore.Services
{
    /// <summary>
    /// Implementation of the IHttpContextAccessor interface for testing purposes. With
    /// this implementation we get a DefaultHttpContext.
    /// </summary>
    public class DummyHttpContextAccessor : IHttpContextAccessor
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="userClaimKey">The user claim key.</param>
        /// <param name="userClaimValue">The user claim value.</param>
        public DummyHttpContextAccessor(string userClaimKey = null, object userClaimValue = null)
        {
            if (userClaimKey != null)
            {
                var userJson = JsonConvert.SerializeObject(userClaimValue);
                var userClaims = new List<Claim> { new Claim(userClaimKey, userJson) };
                var userIdentity = new ClaimsIdentity(userClaims);

                HttpContext.User.AddIdentity(userIdentity);
            }

        }

        /// <summary>
        /// Gets or sets a reference to the current HttpContext.
        /// </summary>
        public HttpContext HttpContext { get; set; } = new DefaultHttpContext();
    }
}
