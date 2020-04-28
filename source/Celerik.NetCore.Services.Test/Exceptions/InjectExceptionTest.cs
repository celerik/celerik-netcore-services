using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Celerik.NetCore.Services.Test
{
    [TestClass]
    public class InjectExceptionTest
    {
        [TestMethod]
        public void ConstructorEmpty()
        {
            var exception = new InjectException();
            Assert.AreEqual("Exception of type 'Celerik.NetCore.Services.InjectException' was thrown.", exception.Message);
        }

        [TestMethod]
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "We are just testing")]
        public void ConstructorWithMessage()
        {
            var message = "Error...";
            var exception = new InjectException(message);

            Assert.AreEqual(message, exception.Message);
        }

        [TestMethod]
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "We are just testing")]
        public void ConstructorWithMessageAndException()
        {
            var message = "Error...";
            var inner = new Exception();
            var exception = new InjectException(message, inner);

            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(inner, exception.InnerException);
        }
    }
}
