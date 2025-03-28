using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class GenreTest
    {
        [TestMethod]
        public void Genre_Constructeur()
        {
            DateTime dateAchat = new DateTime(2025, 3, 28, 15, 41, 25);
            Genre genre = new Genre("00001", "Test");

            Assert.AreEqual("00001", genre.Id);
            Assert.AreEqual("Test", genre.Libelle);
        }
    }
}
