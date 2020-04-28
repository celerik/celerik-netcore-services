using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Celerik.NetCore.Services.Test
{
    [TestClass]
    public class DummyConfigurationTest : ServiceBaseTest
    {
        [TestMethod]
        public void GetAndSet()
        {
            using var config = new DummyConfiguration();
            config["Almuerzo"] = "Bandeja Paisa";

            Assert.AreEqual("Bandeja Paisa", config["Almuerzo"]);
        }

        [TestMethod]
        public void InvalidKey()
        {
            using var config = new DummyConfiguration();
            Assert.AreEqual(null, config["Cena"]);
        }

        [TestMethod]
        public void GetChildren()
        {
            using var config = new DummyConfiguration();

            config["Desayunos:Lunes:Principal"] = "Calentao";
            config["Desayunos:Lunes:Bebida"] = "Chocolate";
            config["Snack"] = "Cocolatina Jumbo";

            var children = config.GetChildren().ToList();

            Assert.AreEqual(2, children.Count);

            Assert.AreEqual("Desayunos", children[0].Key);
            Assert.AreEqual("Desayunos", children[0].Path);
            Assert.AreEqual(null, children[0].Value);

            Assert.AreEqual("Snack", children[1].Key);
            Assert.AreEqual("Snack", children[1].Path);
            Assert.AreEqual("Cocolatina Jumbo", children[1].Value);
        }

        [TestMethod]
        public void GetReloadToken()
        {
            using var config = new DummyConfiguration();
            var reloadToken = config.GetReloadToken();

            Assert.AreNotEqual(null, reloadToken);
        }

        [TestMethod]
        public void GetSection()
        {
            using var config = new DummyConfiguration();

            config["Desayunos:Martes:Principal"] = "Pastel 3 quesos La Miguería";
            config["Desayunos:Martes:Bebida"] = "Fresco";
            config["Onces"] = "Galleta Wafer Jet";

            var desayunos = config.GetSection("Desayunos");

            Assert.AreEqual("Desayunos", desayunos.Key);
            Assert.AreEqual("Desayunos", desayunos.Path);
            Assert.AreEqual(null, desayunos.Value);

            var desayunoMartes = desayunos.GetSection("Martes");

            Assert.AreEqual("Martes", desayunoMartes.Key);
            Assert.AreEqual("Desayunos:Martes", desayunoMartes.Path);
            Assert.AreEqual(null, desayunoMartes.Value);

            var itemsDesayunoMartes = desayunoMartes.GetChildren()
                .OrderBy(i => i.Key)
                .ToList();

            Assert.AreEqual(2, itemsDesayunoMartes.Count);

            Assert.AreEqual("Bebida", itemsDesayunoMartes[0].Key);
            Assert.AreEqual("Desayunos:Martes:Bebida", itemsDesayunoMartes[0].Path);
            Assert.AreEqual("Fresco", itemsDesayunoMartes[0].Value);

            Assert.AreEqual("Principal", itemsDesayunoMartes[1].Key);
            Assert.AreEqual("Desayunos:Martes:Principal", itemsDesayunoMartes[1].Path);
            Assert.AreEqual("Pastel 3 quesos La Miguería", itemsDesayunoMartes[1].Value);
        }

        [TestMethod]
        public void DoubleDispose()
        {
            using var config = new DummyConfiguration();
            config.Dispose();

            Assert.AreNotEqual(null, config);
        }
    }
}
