using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class CommandeTest
    {
        [TestMethod]
        public void Commande_Constructeur()
        {
            DateTime dateCommande = new DateTime(2025, 3, 28, 15, 41, 25);
            Commande commande = new Commande("00001", dateCommande, 1);

            Assert.AreEqual("00001", commande.Id);
            Assert.AreEqual(dateCommande, commande.DateCommande);
            Assert.AreEqual(1, commande.Montant);
        }
    }
}
