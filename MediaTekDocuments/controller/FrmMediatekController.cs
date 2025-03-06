using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }


        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        public string GetIdByNameOfRayon(string idRayon)
        {
            return access.GetIdByNameOfRayon(idRayon);
        }

        public string GetIdByNameOfPublic(string idPublic)
        {
            return access.GetIdByNameOfPublic(idPublic);
        }

        public string GetIdByNameOfGenre(string idGenre)
        {
            return access.GetIdByNameOfGenre(idGenre);
        }

        /// <summary>
        /// Ajoute un document (livre, DVD, etc.) dans la base de données via l'API.
        /// </summary>
        /// <param name="document">L'objet document à ajouter</param>
        /// <returns>True si l'ajout a réussi, False sinon</returns>
        public bool AjouterLivre(Livre livre)
        {
            access.AjouterDocument(livre);
            access.AjouterLivreDvD(livre);
            return access.AjouterLivre(livre);

            //mettre à jour la page d'accueil
        }

        /// <summary>
        /// Ajoute un document (livre, DVD, etc.) dans la base de données via l'API.
        /// </summary>
        /// <param name="document">L'objet document à ajouter</param>
        /// <returns>True si l'ajout a réussi, False sinon</returns>
        public bool AjouterRevue(Revue revue)
        {
            access.AjouterDocument(revue);
            return access.AjouterRevue(revue);

            //mettre à jour la page d'accueil
        }

        /// <summary>
        /// Ajoute un document (livre, DVD, etc.) dans la base de données via l'API.
        /// </summary>
        /// <param name="document">L'objet document à ajouter</param>
        /// <returns>True si l'ajout a réussi, False sinon</returns>
        public bool AjouterDvd(Dvd dvd)
        {
            access.AjouterDocument(dvd);
            access.AjouterLivreDvD(dvd);
            return access.AjouterDvd(dvd);

            //mettre à jour la page d'accueil
        }

        public bool ModifierLivre(Livre livre, Document document)
        {
            access.ModifierDocument(document);
            access.ModifierLivre_DvD(livre);
            return access.ModifierLivre(livre);
        }

        public bool ModifierDvd(Dvd dvd, Document document)
        {
            access.ModifierDocument(document);
            access.ModifierLivre_DvD(dvd);
            return access.ModifierDvd(dvd);
        }

        public bool ModifierRevue(Revue revue, Document document)
        {
            access.ModifierDocument(document);
            return access.ModifierRevue(revue);
        }

        public void GetAllDictionnaries()
        {
            access.DictionnaireGenre();
            access.DictionnairePublic();
            access.DictionnaireRayon();
        }

        public bool SupprimerLivre(Livre livre, Document document)
        {
            access.SupprimerLivre(livre);
            access.SupprimerLivre_DvD(livre);
            return access.SupprimerDocument(document);
        }

        public bool SupprimerRevue(Revue revue, Document document)
        {
            access.SupprimerRevue(revue);
            return access.SupprimerDocument(document);
        }

        public bool SupprimerDvd(Dvd dvd, Document document)
        {
            access.SupprimerDvd(dvd);
            access.SupprimerLivre_DvD(dvd);
            return access.SupprimerDocument(document);
        }
    }
}
