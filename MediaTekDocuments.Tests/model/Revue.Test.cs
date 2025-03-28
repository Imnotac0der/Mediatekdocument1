using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class RevueTest
    {
        [TestMethod]
        public void Revue_Constructeur()
        {
            Revue revue = new Revue("00001", "titre", "image", "00001", "TestGenre", "00001", "TestPublic", "00001", "TestRayon", "periodicite", 1);

            Assert.AreEqual("00001", revue.Id);
            Assert.AreEqual("titre", revue.Titre);
            Assert.AreEqual("image", revue.Image);
            Assert.AreEqual("00001", revue.IdGenre);
            Assert.AreEqual("TestGenre", revue.Genre);
            Assert.AreEqual("00001", revue.IdPublic);
            Assert.AreEqual("TestPublic", revue.Public);
            Assert.AreEqual("00001", revue.IdRayon);
            Assert.AreEqual("TestRayon", revue.Rayon);
            Assert.AreEqual("periodicite", revue.Periodicite);
            Assert.AreEqual(1, revue.DelaiMiseADispo);
        }
    }
}
