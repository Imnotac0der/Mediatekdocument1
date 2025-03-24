
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Categorie (réunit les informations des classes Public, Genre et Rayon)
    /// </summary>
    public class CommandeDocument
    {
        public string Id { get; }
        public int NbExemplaire { get; }
        public string IdLivreDvd { get; }

        public CommandeDocument(string id, int nbExemplaire, string idLivreDvd)
        {
            this.Id = id;
            this.NbExemplaire = nbExemplaire;
            this.IdLivreDvd = idLivreDvd;
        }

    }
}
