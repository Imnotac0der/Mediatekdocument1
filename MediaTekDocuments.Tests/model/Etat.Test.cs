using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class EtatTest
    {
        [TestMethod]
        public void Etat_Constructeur()
        {
            Etat etat = new Etat("00001", "neuf");

            Assert.AreEqual("00001", etat.Id);
            Assert.AreEqual("neuf", etat.Libelle);
        }
    }
}
