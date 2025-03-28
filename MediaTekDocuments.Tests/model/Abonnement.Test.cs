using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class AbonnementTest
    {
        [TestMethod]
        public void Abonnement_Constructeur()
        {
            DateTime dateCommande = new DateTime(2025, 3, 28, 15, 41, 25);
            Commande commande = new Commande("0001", dateCommande, 1);
            Abonnement abonnement = new Abonnement("00001", dateCommande, "00001", commande);

            abonnement.CompleterAvecCommande(commande);

            Assert.AreEqual("00001", abonnement.Id);
            Assert.AreEqual(dateCommande, abonnement.DateFinAbonnement);
            Assert.AreEqual("00001", abonnement.IdRevue);
            Assert.AreEqual("0001", abonnement.IdCommande);
            Assert.AreEqual(dateCommande, abonnement.DateCommande);
            Assert.AreEqual(1, abonnement.Montant);
        }
    }
}
