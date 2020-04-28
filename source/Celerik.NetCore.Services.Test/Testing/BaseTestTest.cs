using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Celerik.NetCore.Services.Test
{
    [TestClass]
    public class BaseTestTest : BaseTest
    {
        [TestMethod]
        public void SetUserClaims()
        {
            var claimKey = "UserName";
            var claimValue = "René Higuita";
            SetUserClaims(claimKey, claimValue);

            var httpContextAccesor = GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccesor.HttpContext;
            var claimObj = httpContext.User.Claims.FirstOrDefault(c => c.Type == claimKey);
            var claimStr = JsonConvert.DeserializeObject<string>(claimObj.Value);

            Assert.AreEqual(claimValue, claimStr);
        }

        [TestMethod]
        public void GetIConfiguration()
        {
            var configSvc = GetService<IConfiguration>();
            var isDummyConfiguration = configSvc is DummyConfiguration;

            Assert.AreEqual(true, isDummyConfiguration);
        }
    }
}
