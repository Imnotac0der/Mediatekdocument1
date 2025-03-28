using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class ExemplaireTest
    {
        [TestMethod]
        public void Exemplaire_Constructeur()
        {
            DateTime dateAchat = new DateTime(2025, 3, 28, 15, 41, 25);
            Exemplaire exemplaire = new Exemplaire(1, dateAchat, "photo", "00001", "00001");

            Assert.AreEqual(1, exemplaire.Numero);
            Assert.AreEqual(dateAchat, exemplaire.DateAchat);
            Assert.AreEqual("photo", exemplaire.Photo);
            Assert.AreEqual("00001", exemplaire.IdEtat);
            Assert.AreEqual("00001", exemplaire.Id);
        }
    }
}
