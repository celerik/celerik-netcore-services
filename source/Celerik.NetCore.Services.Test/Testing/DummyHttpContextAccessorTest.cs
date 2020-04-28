using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Celerik.NetCore.Services.Test
{
    [TestClass]
    public class DummyHttpContextAccessorTest : ServiceBaseTest
    {
        [TestMethod]
        public void ConstructorGetAndSet()
        {
            var claimKey = "User";
            var claimValue = "El Conde Pátula";

            var httpContextAccesor = new DummyHttpContextAccessor(
                claimKey, claimValue
            );

            var httpContext = httpContextAccesor.HttpContext;
            var claimObj = httpContext.User.Claims.FirstOrDefault(c => c.Type == claimKey);
            var claimStr = JsonConvert.DeserializeObject<string>(claimObj.Value);

            Assert.AreEqual(claimValue, claimStr);

            httpContextAccesor.HttpContext = null;
            Assert.AreEqual(null, httpContextAccesor.HttpContext);
        }
    }
}
