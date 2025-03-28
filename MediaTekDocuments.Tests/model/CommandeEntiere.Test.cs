using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class CommandeEntiereTest
    {
        [TestMethod]
        public void CommandeEntiere_Constructeur()
        {
            DateTime dateCommande = new DateTime(2025, 3, 28, 15, 41, 25);
            Commande commande = new Commande("00001", dateCommande, 1);
            CommandeDocument commandeDocument = new CommandeDocument("00001", 1, "00001");
            Suivi suivi = new Suivi("00001", "00001", "00001");
            CommandeEntiere commandeEntiere = new CommandeEntiere(commande, commandeDocument, suivi);

            Assert.AreEqual("00001", commande.Id);
            Assert.AreEqual(dateCommande, commande.DateCommande);
            Assert.AreEqual(1, commande.Montant);
            Assert.AreEqual(1, commandeDocument.NbExemplaire);
            Assert.AreEqual("00001", commandeDocument.IdLivreDvd);
            Assert.AreEqual("00001", suivi.Stade);
        }
    }
}
