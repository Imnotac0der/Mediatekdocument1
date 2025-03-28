using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class DocumentTest
    {
        [TestMethod]
        public void Document_Constructeur()
        {
            Document document = new Document("00001", "Test", "Test", "00001", "TestGenre", "00001", "TestPublic", "00001", "TestRayon" );

            Assert.AreEqual("00001", document.Id);
            Assert.AreEqual("Test", document.Titre);
            Assert.AreEqual("Test", document.Image);
            Assert.AreEqual("00001", document.IdGenre);
            Assert.AreEqual("TestGenre", document.Genre);
            Assert.AreEqual("00001", document.IdPublic);
            Assert.AreEqual("TestPublic", document.Public);
            Assert.AreEqual("00001", document.IdRayon);
            Assert.AreEqual("TestRayon", document.Rayon);
        }
    }
}
