using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class UtilisateurTests
    {
        [TestMethod]
        public void Utilisateur_Constructeur()
        {
            Utilisateur user = new Utilisateur(0, "Jean", "Duchamps", "jeanduchamps", "motdepasse", "00001");

            Assert.AreEqual(0, user.Id);
            Assert.AreEqual("Jean", user.Firstname);
            Assert.AreEqual("Duchamps", user.Name);
            Assert.AreEqual("jeanduchamps", user.Login);
            Assert.AreEqual("motdepasse", user.Password);
            Assert.AreEqual("00001", user.IdService);
        }
    }

}
