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

        /// <summary>
        /// Guetter sur la liste des abonnements
        /// </summary>
        /// <returns></returns>
        public List<Abonnement> GetAllAbonnements()
        {
            return access.GetAllAbonnements();
        }

        /// <summary>
        /// Guetter sur la liste des abonnements qui expirent dans moins de 30 jours
        /// </summary>
        /// <returns></returns>
        public List<Abonnement> GetAllAbonnements30days()
        {
            return access.GetAllAbonnements30days();
        }

        /// <summary>
        /// Guetter sur la liste des commandes de dvd
        /// </summary>
        /// <returns></returns>
        public List<CommandeDocument> GetAllCommandesDocumentDvd()
        {
            return access.GetAllCommandesDocumentDvd();
        }

        /// <summary>
        /// Guetter sur une commande par son id
        /// </summary>
        /// <returns></returns>
        public Commande GetCommandeById(string id)
        {
            return access.GetCommandeById(id);
        }
        
        /// <summary>
        /// Guetter sur un suivi par son id
        /// </summary>
        /// <returns></returns>
        public Suivi GetSuiviById(string id)
        {
            return access.GetSuiviById(id);
        }

        /// <summary>
        /// Guetter sur un exemplaire par son id
        /// </summary>
        /// <returns></returns>
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
        /// <param name="idDocument">id de la revue concernée</param>F
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

        /// <summary>
        /// Guetter sur un id par l'id d'un rayon
        /// </summary>
        /// <returns></returns>
        public string GetIdByNameOfRayon(string idRayon)
        {
            return access.GetIdByNameOfRayon(idRayon);
        }

        /// <summary>
        /// Guetter sur un id par l'id d'un public
        /// </summary>
        /// <returns></returns>
        public string GetIdByNameOfPublic(string idPublic)
        {
            return access.GetIdByNameOfPublic(idPublic);
        }

        /// <summary>
        /// Guetter sur un id par l'id d'un genre
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Ajoute un exemplaire
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns></returns>
        public bool AjouterExemplaire(Exemplaire exemplaire)
        {
            return access.AjouterExemplaire(exemplaire);
        }

        /// <summary>
        /// Modifie un suivi
        /// </summary>
        /// <param name="suivi"></param>
        /// <returns></returns>
        public bool ModifierSuivi(Suivi suivi)
        {
            return access.ModifierSuivi(suivi);
        }

        /// <summary>
        /// Modifie un abonnement
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool UpdateAbonnement(Abonnement abonnement)
        {
            return access.UpdateAbonnement(abonnement);
        }

        /// <summary>
        /// Modifie le montant d'une commande
        /// </summary>
        /// <param name="commande"></param>
        /// <returns></returns>
        public bool UpdateMontantCommande(Commande commande)
        {
            return access.UpdateMontantCommande(commande);
        }

        /// <summary>
        /// Ajoute une commande
        /// </summary>
        /// <param name="commande"></param>
        /// <returns></returns>
        public bool AjouterCommande(Commande commande)
        {
            return access.AjouterCommande(commande);
        }

        /// <summary>
        /// Ajoute un abonnement
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool AjouterAbonnement(Abonnement abonnement)
        {
            return access.AjouterAbonnement(abonnement);
        }

        /// <summary>
        /// Ajoute une CommandeDocument
        /// </summary>
        /// <param name="commandeDocument"></param>
        /// <returns></returns>
        public bool AjouterCommandeDocument(CommandeDocument commandeDocument)
        {
            return access.AjouterCommandeDocument(commandeDocument);
        }

        /// <summary>
        /// Ajoute un suivi
        /// </summary>
        /// <param name="suivi"></param>
        /// <returns></returns>
        public bool AjouterSuivi(Suivi suivi)
        {
            return access.AjouterSuivi(suivi);
        }

        /// <summary>
        /// Modifie un Livre
        /// </summary>
        /// <param name="livre"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool ModifierLivre(Livre livre, Document document)
        {
            access.ModifierDocument(document);
            access.ModifierLivre_DvD(livre);
            return access.ModifierLivre(livre);
        }

        /// <summary>
        /// Modifie un Dvd
        /// </summary>
        /// <param name="dvd"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool ModifierDvd(Dvd dvd, Document document)
        {
            access.ModifierDocument(document);
            access.ModifierLivre_DvD(dvd);
            return access.ModifierDvd(dvd);
        }

        /// <summary>
        /// Modifie une Revue
        /// </summary>
        /// <param name="revue"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool ModifierRevue(Revue revue, Document document)
        {
            access.ModifierDocument(document);
            return access.ModifierRevue(revue);
        }

        /// <summary>
        /// Guetter sur les dictionnaires de Genre, Public et Rayon
        /// </summary>
        public void GetAllDictionnaries()
        {
            access.DictionnaireGenre();
            access.DictionnairePublic();
            access.DictionnaireRayon();
        }

        /// <summary>
        /// Supprime un Livre
        /// </summary>
        /// <param name="livre"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool SupprimerLivre(Livre livre, Document document)
        {
            access.SupprimerLivre(livre);
            access.SupprimerLivre_DvD(livre);
            return access.SupprimerDocument(document);
        }

        /// <summary>
        /// Supprime une Revue
        /// </summary>
        /// <param name="revue"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool SupprimerRevue(Revue revue, Document document)
        {
            access.SupprimerRevue(revue);
            return access.SupprimerDocument(document);
        }

        /// <summary>
        /// Supprime un Dvd
        /// </summary>
        /// <param name="dvd"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool SupprimerDvd(Dvd dvd, Document document)
        {
            access.SupprimerDvd(dvd);
            access.SupprimerLivre_DvD(dvd);
            return access.SupprimerDocument(document);
        }

        /// <summary>
        /// Supprime une Commande
        /// </summary>
        /// <param name="commande"></param>
        /// <param name="commandeDocument"></param>
        /// <param name="suivi"></param>
        /// <returns></returns>
        public bool SupprimerCommande(Commande commande, CommandeDocument commandeDocument, Suivi suivi)
        {
            access.SupprimerSuivi(suivi);
            access.SupprimerCommandeDocument(commandeDocument);
            return access.SupprimerCommande(commande);
        }

        /// <summary>
        /// Supprime un Abonnement
        /// </summary>
        /// <param name="commande"></param>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool SupprimerAbonnement(Commande commande, Abonnement abonnement)
        {
            access.SupprimerAbonnement(abonnement);
            return access.SupprimerCommande(commande);
        }

        /// <summary>
        /// Vérifie un Abonnement par son idRevue
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckAbonnementByIdRevue(string id)
        {
            return access.CheckAbonnementByIdRevue(id);
        }

        /// <summary>
        /// Vérifie la parution
        /// </summary>
        /// <param name="datecommande"></param>
        /// <param name="dateFin"></param>
        /// <param name="dateParution"></param>
        /// <returns></returns>
        public bool ParutionDansAbonnement(DateTime datecommande, DateTime dateFin, DateTime? dateParution)
        {
            return access.ParutionDansAbonnement(datecommande, dateFin, dateParution);
        }

        /// <summary>
        /// Guetter sur Etat
        /// </summary>
        /// <returns></returns>
        public List<Etat> GetAllEtats()
        {
            return access.GetAllEtats();
        }

        /// <summary>
        /// Modifie l'Etat d'un Exemplaire
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <param name="etat"></param>
        /// <returns></returns>
        public bool ModifierEtatExemplaire(Exemplaire exemplaire, Etat etat)
        {
            return access.ModifierEtatExemplaire(exemplaire, etat);
        }

        /// <summary>
        /// Supprime un Exemplaire
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns></returns>
        public bool SupprimerExemplaire(Exemplaire exemplaire)
        {
            return access.SupprimerExemplaire(exemplaire);
        }

        /// <summary>
        /// Connexion d'un utilisateur
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool IsConnected(string login, string pwd)
        {
            return access.IsConnected(login, pwd);
        }

        /// <summary>
        /// Guetter d'un utilisateur par son login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Utilisateur GetUtilisateur(string login)
        {
            return access.GetUtilisateur(login);
        }

    }
}
