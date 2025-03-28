using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class CommandeDocumentTest
    {
        [TestMethod]
        public void CommandeDocument_Constructeur()
        {
            CommandeDocument commandeDocument = new CommandeDocument("00001", 1, "00001");

            Assert.AreEqual("00001", commandeDocument.Id);
            Assert.AreEqual(1, commandeDocument.NbExemplaire);
            Assert.AreEqual("00001", commandeDocument.IdLivreDvd);
        }
    }
}
