using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class CategorieTest
    {
        [TestMethod]
        public void Categorie_Constructeur()
        {
            Categorie categorie = new Categorie("00001", "neuf");

            Assert.AreEqual("00001", categorie.Id);
            Assert.AreEqual("neuf", categorie.Libelle);
        }
    }
}
