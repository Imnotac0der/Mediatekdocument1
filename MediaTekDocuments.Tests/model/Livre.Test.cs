using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class LivreTest
    {
        [TestMethod]
        public void Livre_Constructeur()
        {
            Livre livre = new Livre("00001", "titre", "image", "isbn", "auteur", "collection","00001", "TestGenre", "00001", "TestPublic", "00001", "TestRayon");

            Assert.AreEqual("00001", livre.Id);
            Assert.AreEqual("titre", livre.Titre);
            Assert.AreEqual("image", livre.Image);
            Assert.AreEqual("isbn", livre.Isbn);
            Assert.AreEqual("auteur", livre.Auteur);
            Assert.AreEqual("collection", livre.Collection);
            Assert.AreEqual("00001", livre.IdGenre);
            Assert.AreEqual("TestGenre", livre.Genre);
            Assert.AreEqual("00001", livre.IdPublic);
            Assert.AreEqual("TestPublic", livre.Public);
            Assert.AreEqual("00001", livre.IdRayon);
            Assert.AreEqual("TestRayon", livre.Rayon);
        }
    }
}
