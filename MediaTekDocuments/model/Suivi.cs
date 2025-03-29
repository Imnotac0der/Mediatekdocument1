
namespace MediaTekDocuments.model
{
    using System;
    /// <summary>
    /// Classe métier Suivi
    /// </summary>
    public class Suivi
    {
        public string Id { get; }
        public string Stade { get; }
        public string IdLivreDvd { get; }

        public Suivi(string id, string stade, string idLivreDvd)
        {
            this.Id = id;
            this.Stade = stade;
            this.IdLivreDvd = idLivreDvd;
        }

    }
}
