
namespace MediaTekDocuments.model
{
    using System;
    /// <summary>
    /// Classe métier Categorie (réunit les informations des classes Public, Genre et Rayon)
    /// </summary>
    public class Commande
    {
        public string Id { get; }
        public DateTime DateCommande { get; }
        public double Montant { get; }

        public Commande(string id, DateTime dateCommande, double montant)
        {
            this.Id = id;
            this.DateCommande = dateCommande;
            this.Montant = montant;
        }

    }
}
