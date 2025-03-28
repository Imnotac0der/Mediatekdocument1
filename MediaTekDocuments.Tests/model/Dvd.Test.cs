using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class DvdTest
    {
        [TestMethod]
        public void Dvd_Constructeur()
        {
            Dvd dvd = new Dvd("00001", "Test", "Test",1, "TestR", "TestS", "00001", "TestGenre", "00001", "TestPublic", "00001", "TestRayon");

            Assert.AreEqual("00001", dvd.Id);
            Assert.AreEqual("Test", dvd.Titre);
            Assert.AreEqual("Test", dvd.Image);
            Assert.AreEqual("TestR", dvd.Realisateur);
            Assert.AreEqual("TestS", dvd.Synopsis);
            Assert.AreEqual("00001", dvd.IdGenre);
            Assert.AreEqual("TestGenre", dvd.Genre);
            Assert.AreEqual("00001", dvd.IdPublic);
            Assert.AreEqual("TestPublic", dvd.Public);
            Assert.AreEqual("00001", dvd.IdRayon);
            Assert.AreEqual("TestRayon", dvd.Rayon);
        }
    }
}
