using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using System;

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
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public Livre GetLivre(string id)
        {
            return access.GetLivre(id);
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public Revue GetRevue(string id)
        {
            return access.GetRevue(id);
        }

        /// <summary>
        /// getter sur la liste des dvd
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public Dvd GetDvd(string id)
        {
            return access.GetDvd(id);
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
        /// getter sur la liste des commandes de livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<CommandeDocument> GetAllCommandesDocumentLivres()
        {
            return access.GetAllCommandesDocumentLivres();
        }

        public List<Abonnement> GetAllAbonnements()
        {
            return access.GetAllAbonnements();
        }

        public List<Abonnement> GetAllAbonnements30days()
        {
            return access.GetAllAbonnements30days();
        }

        public List<CommandeDocument> GetAllCommandesDocumentDvd()
        {
            return access.GetAllCommandesDocumentDvd();
        }

        public Commande GetCommandeById(string id)
        {
            return access.GetCommandeById(id);
        }

        public Suivi GetSuiviById(string id)
        {
            return access.GetSuiviById(id);
        }

        public Exemplaire GetExemplaireById(string idRevue)
        {
            return access.GetExemplaireById(idRevue);
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
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            return access.GetAllExemplaires(idDocument);
        }

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesLivre(string idDocument)
        {
            return access.GetAllExemplaires(idDocument);
        }

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesDvd(string idDocument)
        {
            return access.GetAllExemplaires(idDocument);
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
        }
        public bool AjouterExemplaire(Exemplaire exemplaire)
        {
            return access.AjouterExemplaire(exemplaire);
        }

        public bool ModifierSuivi(Suivi suivi)
        {
            return access.ModifierSuivi(suivi);
        }

        public bool UpdateAbonnement(Abonnement abonnement)
        {
            return access.UpdateAbonnement(abonnement);
        }

        public bool UpdateMontantCommande(Commande commande)
        {
            return access.UpdateMontantCommande(commande);
        }

        public bool AjouterCommande(Commande commande)
        {
            return access.AjouterCommande(commande);
        }

        public bool AjouterAbonnement(Abonnement abonnement)
        {
            return access.AjouterAbonnement(abonnement);
        }

        public bool AjouterCommandeDocument(CommandeDocument commandeDocument)
        {
            return access.AjouterCommandeDocument(commandeDocument);
        }

        public bool AjouterSuivi(Suivi suivi)
        {
            return access.AjouterSuivi(suivi);
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

        public bool SupprimerCommande(Commande commande, CommandeDocument commandeDocument, Suivi suivi)
        {
            access.SupprimerSuivi(suivi);
            access.SupprimerCommandeDocument(commandeDocument);
            return access.SupprimerCommande(commande);
        }

        public bool SupprimerAbonnement(Commande commande, Abonnement abonnement)
        {
            access.SupprimerAbonnement(abonnement);
            return access.SupprimerCommande(commande);
        }

        public bool CheckAbonnementByIdRevue(string id)
        {
            return access.CheckAbonnementByIdRevue(id);
        }

        public bool ParutionDansAbonnement(DateTime datecommande, DateTime dateFin, DateTime? dateParution)
        {
            return access.ParutionDansAbonnement(datecommande, dateFin, dateParution);
        }

        public List<Etat> GetAllEtats()
        {
            return access.GetAllEtats();
        }

        public bool ModifierEtatExemplaire(Exemplaire exemplaire, Etat etat)
        {
            return access.ModifierEtatExemplaire(exemplaire, etat);
        }

        public bool SupprimerExemplaire(Exemplaire exemplaire)
        {
            return access.SupprimerExemplaire(exemplaire);
        }

    }
}
