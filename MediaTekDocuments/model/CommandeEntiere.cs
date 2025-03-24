using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class CommandeEntiere
    {
        public string Id { get; }
        public DateTime DateCommande { get; }
        public double Montant { get; }
        public int NbExemplaire { get; }
        public string IdLivreDvd { get; }
        public string stade { get; }
        public CommandeEntiere(Commande commande, CommandeDocument commandeDocument, Suivi suivi)
        {
            this.Id = commande.Id;
            this.DateCommande = commande.DateCommande;
            this.Montant = commande.Montant;
            this.NbExemplaire = commandeDocument.NbExemplaire;
            this.IdLivreDvd = commandeDocument.IdLivreDvd;
            this.stade = suivi.Stade;
        }
    }
}
