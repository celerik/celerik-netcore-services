using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Celerik.NetCore.Services.Test
{
    [TestClass]
    public class ServiceResourcesTest : ServiceBaseTest
    {
        [TestMethod]
        public void Get()
        {
            var name = "ApiBuilder.MethodAlreadyCalled";
            var resource = ServiceResources.Get(name, nameof(Get));

            Assert.AreEqual("This method was already called: 'Get'", resource);
        }

        [TestMethod]
        public void GetWithArgs()
        {
            var name = "The {0} database couldn´t be deleted!";
            var args = "ChuckNorrisFacts";
            var resource = ServiceResources.Get(name, args);

            Assert.AreEqual("The ChuckNorrisFacts database couldn´t be deleted!", resource);
        }
    }
}
