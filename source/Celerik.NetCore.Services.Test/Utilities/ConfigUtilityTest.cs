using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Celerik.NetCore.Services.Test
{
    public class DeveloperSettigs
    {
        public string ScreenSize { get; set; }
        public string CoffeeBrand { get; set; }
        public string PreferredMusic { get; set; }
        public string Pet { get; set; }
    }

    public enum GuitarBrandType
    {
        Gibson = 1,
        Cheapson = 2
    }

    [TestClass]
    public class ConfigUtilityTest : ServiceBaseTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadObjectFromNullConfiguration()
        {
            IConfiguration config = null;
            config.ReadObject<DeveloperSettigs>("DeveloperSettigs");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void ReadObjectKeyNotFound()
        {
            var config = GetService<IConfiguration>();
            config.ReadObject<DeveloperSettigs>("DeveloperSettigs");
        }

        [TestMethod]
        public void ReadObjectOk()
        {
            var config = GetService<IConfiguration>();
            config["DeveloperSettigs:ScreenSize"] = "100 inches";
            config["DeveloperSettigs:CoffeeBrand"] = "Café La Bastilla";
            config["DeveloperSettigs:PreferredMusic"] = "Ultra super dark satanic bloody metal";
            config["DeveloperSettigs:Pet"] = "Kitten";

            var settings = config.ReadObject<DeveloperSettigs>("DeveloperSettigs");

            Assert.AreEqual("100 inches", settings.ScreenSize);
            Assert.AreEqual("Café La Bastilla", settings.CoffeeBrand);
            Assert.AreEqual("Ultra super dark satanic bloody metal", settings.PreferredMusic);
            Assert.AreEqual("Kitten", settings.Pet);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadEnumFromNullConfiguration()
        {
            IConfiguration config = null;
            config.ReadEnum<GuitarBrandType>("GuitaSettings:Brand");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void ReadEnumKeyNotFound()
        {
            var config = GetService<IConfiguration>();
            config.ReadEnum<GuitarBrandType>("GuitaSettings:Brand");
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void ReadEnumInvalidValue()
        {
            var config = GetService<IConfiguration>();
            config["GuitaSettings:Brand"] = "Invalid";
            config.ReadEnum<GuitarBrandType>("GuitaSettings:Brand");
        }

        [TestMethod]
        public void ReadEnumOk()
        {
            var config = GetService<IConfiguration>();
            config["GuitaSettings:Brand"] = "Cheapson";
            var brand = config.ReadEnum<GuitarBrandType>("GuitaSettings:Brand");

            Assert.AreEqual(GuitarBrandType.Cheapson, brand);
        }
    }
}
