using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using MediaTekDocuments.bddmanager;
using Serilog;



namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = "http://localhost/rest_mediatekdocuments/";
        /// <summary>
        /// nom de connexion à la bdd
        /// </summary>
        private static readonly string connectionName = "MediaTekDocuments.Properties.Settings.mediatek86ConnectionString";
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// Getter sur l'objet d'accès aux données
        /// </summary>
        public BddManager Manager { get; }
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// 
        private const string PUT = "PUT";
        private const string DELETE = "DELETE";


        private readonly Dictionary<string, Genre> classeurGenres = new Dictionary<string, Genre>();
        private readonly Dictionary<string, Public> classeurPublics = new Dictionary<string, Public>();
        private readonly Dictionary<string, Rayon> classeurRayons = new Dictionary<string, Rayon>();

        /// <summary>
        /// Récupération de la chaîne de connexion
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string GetConnectionStringByName(string name)
        {
            string returnValue = null;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];
            if (settings != null)
                returnValue = settings.ConnectionString;
            return returnValue;
        }

        /// <summary>
        /// Création unique de l'objet de type BddManager
        /// Arrête le programme si l'accès à la BDD a échoué
        /// </summary>
        private Access()
        {
            String connectionString = null;
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt")
                    .CreateLogger();

                connectionString = GetConnectionStringByName(connectionName);
                Manager = BddManager.GetInstance(connectionString);

                string authenticationString = "admin:adminpwd";
                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Log.Fatal("Access.Access catch connectionString={0} erreur={1}", connectionString, e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création d'une seule instance de la classe
        /// </summary>
        /// <returns></returns>
        public static Access GetInstance()
        {
            if (instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les états à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Etat</returns>
        public List<Etat> GetAllEtats()
        {
            IEnumerable<Etat> lesEtats = TraitementRecup<Etat>(GET, "etat", null);
            return new List<Etat>(lesEtats);
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<CommandeDocument> GetAllCommandesDocumentLivres()
        {
            List<CommandeDocument> lesCommandesDocument = TraitementRecup<CommandeDocument>(GET, "commandedocument/livre", null);
            return lesCommandesDocument;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<CommandeDocument> GetAllCommandesDocumentDvd()
        {
            List<CommandeDocument> lesCommandesDocument = TraitementRecup<CommandeDocument>(GET, "commandedocument/dvd", null);
            return lesCommandesDocument;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Abonnement> GetAllAbonnements()
        {
            List<Abonnement> lesAbonnements = TraitementRecup<Abonnement>(GET, "abonnement", null);

            foreach (Abonnement abonnement in lesAbonnements)
            {
                Commande commande = GetCommandeById(abonnement.Id);

                if (commande != null)
                {
                    abonnement.CompleterAvecCommande(commande);
                }
            }

            return lesAbonnements;
        }

        /// <summary>
        /// Retourne les abonnements dont la date de fin est dans moins de 30 jours.
        /// </summary>
        /// <returns>Liste filtrée d'abonnements</returns>
        public List<Abonnement> GetAllAbonnements30days()
        {
            List<Abonnement> lesAbonnements = TraitementRecup<Abonnement>(GET, "abonnement", null);
            List<Abonnement> abonnementsAlerte = new List<Abonnement>();

            foreach (Abonnement abonnement in lesAbonnements)
            {
                Commande commande = GetCommandeById(abonnement.Id);

                if (commande != null)
                {
                    abonnement.CompleterAvecCommande(commande);

                    // Filtrer si la date de fin est dans moins de 30 jours
                    if ((abonnement.DateFinAbonnement - DateTime.Now).TotalDays <= 30)
                    {
                        abonnementsAlerte.Add(abonnement);
                    }
                }
            }

            return abonnementsAlerte;
        }

        /// <summary>
        /// Retourne une commande spécifique à partir de son ID.
        /// </summary>
        /// <param name="id">ID de la commande à récupérer</param>
        /// <returns>Objet Commande correspondant à l'ID</returns>
        public Commande GetCommandeById(string id)
        {
            // Création de l'objet JSON à envoyer
            var champs = new Dictionary<string, string> { { "id", id } };

            // Sérialisation en format JSON
            string jsonPayload = JsonConvert.SerializeObject(champs, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            // Encodage en form-urlencoded
            string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

            // Récupération de la commande spécifique
            var result = TraitementRecup<Commande>(GET, "commande", formEncodedPayload);

            // Vérifier le retour de l'API et filtrer l'ID
            return result?.Find(cmd => cmd.Id == id);
        }

        /// <summary>
        /// Vérifie si un abonnement existe pour une revue donnée.
        /// </summary>
        /// <param name="id">ID de la revue à vérifier</param>
        /// <returns>True si un abonnement est trouvé, sinon False</returns>
        public bool CheckAbonnementByIdRevue(string id)
        {
            // Création de l'objet JSON à envoyer
            var champs = new Dictionary<string, string> { { "idRevue", id } };

            // Sérialisation en format JSON
            string jsonPayload = JsonConvert.SerializeObject(champs, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            // Encodage en form-urlencoded
            string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

            // Récupération des abonnements qui correspondent
            var result = TraitementRecup<Abonnement>(GET, "abonnement", formEncodedPayload);

            // Vérifie si la liste n'est pas vide et contient au moins un abonnement avec l'ID spécifié
            return result != null && result.Exists(cmd => cmd.IdRevue == id);
        }

        /// <summary>
        /// Retourne vrai si la Date de Paution est inférieure à Date de Commande et inférieure ou égale à Date de Fin
        /// </summary>
        /// <param name="dateCommande"></param>
        /// <param name="dateFin"></param>
        /// <param name="dateParution"></param>
        /// <returns></returns>
        public bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFin, DateTime? dateParution)
        {
            if (dateParution < dateCommande && dateParution <= dateFin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Retourne une commande spécifique à partir de son ID.
        /// </summary>
        /// <param name="id">ID de la commande à récupérer</param>
        /// <returns>Objet Commande correspondant à l'ID</returns>
        public Exemplaire GetExemplaireById(string id)
        {
            try
            {

                // 🔹 Conversion en JSON pour l'URL (comme Postman)
                string jsonId = Uri.EscapeDataString(JsonConvert.SerializeObject(new { id }));

                // 🔹 Création de l'URL avec le JSON directement
                string url = $"exemplaire/{jsonId}";

                // 🔹 Appel de l'API
                var result = TraitementRecup<Exemplaire>(GET, url, ""); // Pas de body

                // 🔹 Vérification et retour de l'exemplaire trouvé
                return result?.Find(ex => ex.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Retourne une commande spécifique à partir de son ID.
        /// </summary>
        /// <param name="id">ID de la commande à récupérer</param>
        /// <returns>Objet Commande correspondant à l'ID</returns>
        public Suivi GetSuiviById(string id)
        {
            // Création de l'objet JSON à envoyer
            var champs = new Dictionary<string, string> { { "id", id } };

            // Sérialisation en format JSON
            string jsonPayload = JsonConvert.SerializeObject(champs, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            // Encodage en form-urlencoded
            string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

            // Récupération de la commande spécifique
            var result = TraitementRecup<Suivi>(GET, "suivi", formEncodedPayload);

            // Vérifier le retour de l'API et filtrer l'ID
            return result?.Find(cmd => cmd.Id == id);
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);
            return lesRevues;
        }

        /// <summary>
        /// Ajoute un LivreDvd
        /// </summary>
        /// <param name="livreDvd"></param>
        /// <returns></returns>
        public bool AjouterLivreDvD(object livreDvd)
        {
            //effectuer une requete API de POST dans la table livredvd avec id
            try
            {
                // Sélectionner uniquement les champs requis
                var docDict = JObject.FromObject(livreDvd);

                var livreDvdFiltré = new
                {
                    id = docDict.ContainsKey("Id") ? docDict["Id"]?.ToString() : null,
                };


                string jsonPayload = JsonConvert.SerializeObject(
             livreDvdFiltré,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                }
                );


                // Encapsuler dans "champs=" et encoder en application/x-www-form-urlencoded
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Envoyer la requête en POST avec application/x-www-form-urlencoded
                List<Document> liste = TraitementRecup<Document>(POST, "livres_dvd", formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }


                return (liste != null);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Ajoute un abonnement
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool AjouterAbonnement(Abonnement abonnement)
        {
            try
            {
                // Vérification et extraction des données
                var docDict = JObject.Parse(JsonConvert.SerializeObject(abonnement));

                // Extraction et conversion des champs
                var abonnementTriage = new
                {
                    id = docDict["Id"]?.ToString() ?? "MISSING_ID",
                    dateFinAbonnement = docDict["DateFinAbonnement"] != null && DateTime.TryParse(docDict["DateFinAbonnement"].ToString(), out DateTime date)
                ? date.ToString("yyyy-MM-dd") // Formatage pour MySQL
                : null,
                    idRevue = docDict["IdRevue"]?.ToString() ?? "MISSING_IDREVUE"
                };

                // Vérification avant envoi
                if (string.IsNullOrEmpty(abonnementTriage.id) || abonnementTriage.dateFinAbonnement == null)
                {
                    return false;
                }

                // Sérialisation en JSON
                string jsonPayload = JsonConvert.SerializeObject(
                    abonnementTriage,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    }
                );

                // Encapsuler dans "champs=" et encoder en application/x-www-form-urlencoded
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Envoi de la requête POST
                List<Abonnement> liste = TraitementRecup<Abonnement>(POST, "abonnement", formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return liste != null;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Ajoute un exemplaire
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns></returns>
        public bool AjouterExemplaire(Exemplaire exemplaire)
        {
            try
            {
                var docDict = JObject.Parse(JsonConvert.SerializeObject(exemplaire));
                dynamic exemplaireTriage;

                if (exemplaire.Numero == 0) // ou == null si autorisé
                {
                    exemplaireTriage = new
                    {
                        id = docDict["Id"]?.ToString() ?? "MISSING_ID",
                        dateAchat = docDict["DateAchat"] != null && DateTime.TryParse(docDict["DateAchat"].ToString(), out DateTime date)
                            ? date.ToString("yyyy-MM-dd")
                            : null,
                        photo = docDict["Photo"]?.ToString() ?? "",
                        idEtat = docDict["IdEtat"]?.ToString() ?? "MISSING_IDETAT"
                    };
                }
                else
                {
                    exemplaireTriage = new
                    {
                        id = docDict["Id"]?.ToString() ?? "MISSING_ID",
                        numero = docDict.ContainsKey("Numero") && int.TryParse(docDict["Numero"]?.ToString(), out int result) ? result : 0,
                        dateAchat = docDict["DateAchat"] != null && DateTime.TryParse(docDict["DateAchat"].ToString(), out DateTime date)
                            ? date.ToString("yyyy-MM-dd")
                            : null,
                        photo = docDict["Photo"]?.ToString() ?? "",
                        idEtat = docDict["IdEtat"]?.ToString() ?? "MISSING_IDETAT"
                    };
                }

                // Vérification
                if (string.IsNullOrEmpty(exemplaireTriage.id) || string.IsNullOrEmpty(exemplaireTriage.idEtat))
                {
                    return false;
                }

                string jsonPayload = JsonConvert.SerializeObject(exemplaireTriage,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    });

                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                List<Document> liste = TraitementRecup<Document>(POST, "exemplaire", formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Ajoute une commande
        /// </summary>
        /// <param name="commande"></param>
        /// <returns></returns>
        public bool AjouterCommande(Commande commande)
        {
            try
            {
                // Vérification et extraction des données
                var docDict = JObject.Parse(JsonConvert.SerializeObject(commande));

                // Extraction et conversion des champs
                var commandeTriage = new
                {
                    id = docDict["Id"]?.ToString() ?? "MISSING_ID",
                    dateCommande = docDict["DateCommande"] != null && DateTime.TryParse(docDict["DateCommande"].ToString(), out DateTime date)
                ? date.ToString("yyyy-MM-dd") // Formatage pour MySQL
                : null,
                    montant = docDict["Montant"]?.ToString() ?? ""
                };

                // Sérialisation en JSON
                string jsonPayload = JsonConvert.SerializeObject(
                    commandeTriage,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    }
                );

                // Encapsuler dans "champs=" et encoder en application/x-www-form-urlencoded
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Envoi de la requête POST
                List<Document> liste = TraitementRecup<Document>(POST, "commande", formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return liste != null;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Ajoute une CommandeDocument
        /// </summary>
        /// <param name="commandeDocument"></param>
        /// <returns></returns>
        public bool AjouterCommandeDocument(CommandeDocument commandeDocument)
        {
            try
            {
                // Vérification et extraction des données
                var docDict = JObject.Parse(JsonConvert.SerializeObject(commandeDocument));

                // Extraction et conversion des champs
                var commandeTriage = new
                {
                    id = docDict["Id"]?.ToString() ?? "MISSING_ID",
                    nbExemplaire = docDict.ContainsKey("NbExemplaire") && int.TryParse(docDict["NbExemplaire"]?.ToString(), out int result) ? result : 0,
                    idLivreDvd = docDict["IdLivreDvd"]?.ToString() ?? ""
                };

                // Sérialisation en JSON
                string jsonPayload = JsonConvert.SerializeObject(
                    commandeTriage,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    }
                );

                // Encapsuler dans "champs=" et encoder en application/x-www-form-urlencoded
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Envoi de la requête POST
                List<Document> liste = TraitementRecup<Document>(POST, "commandedocument", formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return liste != null;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Ajoute un Suivi
        /// </summary>
        /// <param name="suivi"></param>
        /// <returns></returns>
        public bool AjouterSuivi(Suivi suivi)
        {
            try
            {
                // Vérification et extraction des données
                var docDict = JObject.Parse(JsonConvert.SerializeObject(suivi));

                // Extraction et conversion des champs
                var commandeTriage = new
                {
                    id = docDict["Id"]?.ToString() ?? "MISSING_ID",
                    stade = docDict["Stade"]?.ToString() ?? "",
                    idLivreDvd = docDict["IdLivreDvd"]?.ToString() ?? ""
                };

                // Sérialisation en JSON
                string jsonPayload = JsonConvert.SerializeObject(
                    commandeTriage,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    }
                );

                // Encapsuler dans "champs=" et encoder en application/x-www-form-urlencoded
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Envoi de la requête POST
                List<Document> liste = TraitementRecup<Document>(POST, "suivi", formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return liste != null;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Ajoute une Revue
        /// </summary>
        /// <param name="revue"></param>
        /// <returns></returns>
        public bool AjouterRevue(object revue)
        {
            //effectuer une requete API de POST dans la table livre avec id, isbn, auteur et collection

            try
            {
                // Sélectionner uniquement les champs requis
                var docDict = JObject.FromObject(revue);

                var revueFiltré = new
                {
                    id = docDict.ContainsKey("Id") ? docDict["Id"]?.ToString() : null,
                    periodicite = docDict.ContainsKey("Periodicite") ? docDict["Periodicite"]?.ToString() : null, // MAJUSCULE CORRIGÉE
                    delaiMiseADispo = docDict.ContainsKey("DelaiMiseADispo") && int.TryParse(docDict["DelaiMiseADispo"]?.ToString(), out int result) ? result : 0 // MAJUSCULE CORRIGÉE

                };


                string jsonPayload = JsonConvert.SerializeObject(
             revueFiltré,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                }
                );


                // Encapsuler dans "champs=" et encoder en application/x-www-form-urlencoded
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Envoyer la requête en POST avec application/x-www-form-urlencoded
                List<Document> liste = TraitementRecup<Document>(POST, "revue", formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return (liste != null);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Ajoute un Livre
        /// </summary>
        /// <param name="livre"></param>
        /// <returns></returns>
        public bool AjouterLivre(object livre)
        {
            //effectuer une requete API de POST dans la table livre avec id, isbn, auteur et collection

            try
            {
                // Sélectionner uniquement les champs requis
                var docDict = JObject.FromObject(livre);

                var livreFiltré = new
                {
                    id = docDict.ContainsKey("Id") ? docDict["Id"]?.ToString() : null,
                    isbn = docDict.ContainsKey("Isbn") ? docDict["Isbn"]?.ToString() : null,
                    auteur = docDict.ContainsKey("Auteur") ? docDict["Auteur"]?.ToString() : null,
                    collection = docDict.ContainsKey("Collection") ? docDict["Collection"]?.ToString() : null
                };


                string jsonPayload = JsonConvert.SerializeObject(
             livreFiltré,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                }
                );


                // Encapsuler dans "champs=" et encoder en application/x-www-form-urlencoded
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Envoyer la requête en POST avec application/x-www-form-urlencoded
                List<Document> liste = TraitementRecup<Document>(POST, "livre", formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return (liste != null);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Ajoute un Document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool AjouterDocument(object document)
        {
            try
            {
                // Sélectionner uniquement les champs requis
                var docDict = JObject.FromObject(document);

                var documentFiltré = new
                {
                    id = docDict.ContainsKey("Id") ? docDict["Id"]?.ToString() : null,
                    titre = docDict.ContainsKey("Titre") ? docDict["Titre"]?.ToString() : null,
                    idRayon = docDict.ContainsKey("IdRayon") ? docDict["IdRayon"]?.ToString() : null,
                    idPublic = docDict.ContainsKey("IdPublic") ? docDict["IdPublic"]?.ToString() : null,
                    idGenre = docDict.ContainsKey("IdGenre") ? docDict["IdGenre"]?.ToString() : null,
                    image = docDict.ContainsKey("Image") ? docDict["Image"]?.ToString() : null
                };


                string jsonPayload = JsonConvert.SerializeObject(
             documentFiltré,
                new Newtonsoft.Json.JsonSerializerSettings
                        {
                     ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                 }
                );


                // Encapsuler dans "champs=" et encoder en application/x-www-form-urlencoded
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Envoyer la requête en POST avec application/x-www-form-urlencoded
                List<Document> liste = TraitementRecup<Document>(POST, "document", formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return (liste != null);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Ajoute un Dvd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns></returns>
        public bool AjouterDvd(object dvd)
        {
            //effectuer une requete API de POST dans la table livre avec id, isbn, auteur et collection

            try
            {
                // Sélectionner uniquement les champs requis
                var docDict = JObject.FromObject(dvd);

                var dvdFiltre = new
                {
                    id = docDict.ContainsKey("Id") ? docDict["Id"]?.ToString() : null,
                    synopsis = docDict.ContainsKey("Synopsis") ? docDict["Synopsis"]?.ToString() : null,
                    realisateur = docDict.ContainsKey("Realisateur") ? docDict["Realisateur"]?.ToString() : null,
                    duree = docDict.ContainsKey("Duree") ? docDict["Duree"]?.ToString() : null
                };


                string jsonPayload = JsonConvert.SerializeObject(
             dvdFiltre,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                }
                );


                // Encapsuler dans "champs=" et encoder en application/x-www-form-urlencoded
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Envoyer la requête en POST avec application/x-www-form-urlencoded
                List<Document> liste = TraitementRecup<Document>(POST, "dvd", formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return (liste != null);
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Guetter d'un id par le nom d'un Rayon
        /// </summary>
        /// <param name="nomRayon"></param>
        /// <returns></returns>
        public string GetIdByNameOfRayon(string nomRayon)
        {
            if (classeurRayons != null && classeurRayons.Count > 0)
            {
                // Cherche l'ID correspondant au libellé
                var rayon = classeurRayons.FirstOrDefault(x => x.Value.Libelle == nomRayon);

                if (!string.IsNullOrEmpty(rayon.Key))
                {
                    return rayon.Key; // Retourne l'ID du rayon trouvé
                }
            }

            return null; // Retourne null si aucun rayon correspondant n'est trouvé
        }

        /// <summary>
        /// Guetter d'un id par le nom d'un Public
        /// </summary>
        /// <param name="nomPublic"></param>
        /// <returns></returns>
        public string GetIdByNameOfPublic(string nomPublic)
        {
            if (classeurPublics != null && classeurPublics.Count > 0)
            {
                // Cherche l'ID correspondant au libellé
                var pub = classeurPublics.FirstOrDefault(x => x.Value.Libelle == nomPublic);

                if (!string.IsNullOrEmpty(pub.Key))
                {
                    return pub.Key; // Retourne l'ID du public trouvé
                }
            }

            return null; // Retourne null si aucun public correspondant n'est trouvé
        }

        /// <summary>
        /// Guetter d'un id par le nom d'un Genre
        /// </summary>
        /// <param name="nomGenre"></param>
        /// <returns></returns>
        public string GetIdByNameOfGenre(string nomGenre)
        {
            if (classeurGenres != null && classeurGenres.Count > 0)
            {
                // Cherche l'ID correspondant au libellé
                var genre = classeurGenres.FirstOrDefault(x => x.Value.Libelle == nomGenre);

                if (!string.IsNullOrEmpty(genre.Key))
                {
                    return genre.Key; // Retourne l'ID du genre trouvé
                }
            }

            return null; // Retourne null si aucun genre correspondant n'est trouvé
        }

        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetAllExemplaires(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// Met à jour le dictionnaire
        /// </summary>
        public void DictionnaireGenre()
        {
            List<Categorie> genres = GetAllGenres();

            foreach (var genre in genres)
            {
                classeurGenres[genre.Id] = (Genre)genre; // Conversion Categorie -> Genre si nécessaire
            }
        }

        /// <summary>
        /// Met à jour le dictionnaire
        /// </summary>
        public void DictionnairePublic()
        {
            List<Categorie> publics = GetAllPublics();

            foreach (var pub in publics)
            {
                classeurPublics[pub.Id] = (Public)pub; // Conversion Categorie -> Public si nécessaire
            }

        }

        /// <summary>
        /// Met à jour le dictionnaire
        /// </summary>
        public void DictionnaireRayon()
        {
            List<Categorie> rayons = GetAllRayons();

            foreach (var rayon in rayons)
            {
                classeurRayons[rayon.Id] = (Rayon)rayon; // Conversion Categorie -> Public si nécessaire
            }

        }

        /// <summary>
        /// Modifie l'etat d'un exemplaire
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <param name="etat"></param>
        /// <returns></returns>
        public bool ModifierEtatExemplaire(Exemplaire exemplaire, Etat etat)
        {
            try
            {
                // Construire l'objet JSON avec seulement `isbn` et `auteur`
                var etatFiltre = new
                {
                    numero = exemplaire.Numero,
                    idEtat = etat.Id
                };

                // Convertir l'objet en JSON
                string jsonPayload = JsonConvert.SerializeObject(etatFiltre);

                // 🔹 Encapsuler le JSON dans `x-www-form-urlencoded`
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // URL API avec l'ID du livre
                string url = $"exemplaire/numero";

                // Envoyer la requête PUT
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(PUT, url, formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie une Revue
        /// </summary>
        /// <param name="revue"></param>
        /// <returns></returns>
        public bool ModifierRevue(Revue revue)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(revue.Id))
                {
                    return false;
                }

                // Construire l'objet JSON avec seulement `isbn` et `auteur`
                var revueFiltre = new
                {
                    periodicite = revue.Periodicite,
                    delaiMiseADispo = revue.DelaiMiseADispo

                };

                // Convertir l'objet en JSON
                string jsonPayload = JsonConvert.SerializeObject(revueFiltre);

                // 🔹 Encapsuler le JSON dans `x-www-form-urlencoded`
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // URL API avec l'ID du livre
                string url = $"revue/{revue.Id}";

                // Envoyer la requête PUT
                List<Document> liste = TraitementRecup<Document>(PUT, url, formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie un abonnement
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool UpdateAbonnement(Abonnement abonnement)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(abonnement.Id))
                {
                    return false;
                }

                // Construire l'objet JSON avec seulement `isbn` et `auteur`
                var revueFiltre = new
                {
                    dateFinAbonnement = abonnement.DateFinAbonnement
                };

                // Convertir l'objet en JSON
                string jsonPayload = JsonConvert.SerializeObject(revueFiltre);

                // 🔹 Encapsuler le JSON dans `x-www-form-urlencoded`
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // URL API avec l'ID du livre
                string url = $"abonnement/{abonnement.Id}";

                // Envoyer la requête PUT
                List<Abonnement> liste = TraitementRecup<Abonnement>(PUT, url, formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie le montant d'une Commande
        /// </summary>
        /// <param name="commande"></param>
        /// <returns></returns>
        public bool UpdateMontantCommande(Commande commande)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(commande.Id))
                {
                    return false;
                }

                // Construire l'objet JSON avec seulement `isbn` et `auteur`
                var revueFiltre = new
                {
                    montant = commande.Montant
                };

                // Convertir l'objet en JSON
                string jsonPayload = JsonConvert.SerializeObject(revueFiltre);

                // 🔹 Encapsuler le JSON dans `x-www-form-urlencoded`
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // URL API avec l'ID du livre
                string url = $"commande/{commande.Id}";

                // Envoyer la requête PUT
                List<Abonnement> liste = TraitementRecup<Abonnement>(PUT, url, formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie un Suivi
        /// </summary>
        /// <param name="suivi"></param>
        /// <returns></returns>
        public bool ModifierSuivi(Suivi suivi)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(suivi.Id))
                {
                    return false;
                }

                // Construire l'objet JSON avec seulement `isbn` et `auteur`
                var revueFiltre = new
                {
                    id = suivi.Id,
                    stade = suivi.Stade,
                    idLivreDvd = suivi.IdLivreDvd

                };

                // Convertir l'objet en JSON
                string jsonPayload = JsonConvert.SerializeObject(revueFiltre);

                // 🔹 Encapsuler le JSON dans `x-www-form-urlencoded`
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // URL API avec l'ID du livre
                string url = $"suivi/{suivi.Id}";

                // Envoyer la requête PUT
                List<Document> liste = TraitementRecup<Document>(PUT, url, formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie un Dvd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns></returns>
        public bool ModifierDvd(Dvd dvd)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(dvd.Id))
                {
                    return false;
                }

                // Construire l'objet JSON avec seulement `isbn` et `auteur`
                var dvdFiltre = new
                {
                    synopsis = dvd.Synopsis,
                    realisateur = dvd.Realisateur,
                    duree = dvd.Duree.ToString()


                };

                // Convertir l'objet en JSON
                string jsonPayload = JsonConvert.SerializeObject(dvdFiltre);

                // 🔹 Encapsuler le JSON dans `x-www-form-urlencoded`
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // URL API avec l'ID du livre
                string url = $"dvd/{dvd.Id}";

                // Envoyer la requête PUT
                List<Document> liste = TraitementRecup<Document>(PUT, url, formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie un Livre
        /// </summary>
        /// <param name="livre"></param>
        /// <returns></returns>
        public bool ModifierLivre(Livre livre)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(livre.Id))
                {
                    return false;
                }

                // Construire l'objet JSON avec seulement `isbn` et `auteur`
                var livreFiltré = new
                {
                    isbn = livre.Isbn,
                    auteur = livre.Auteur,
                    collection = livre.Collection
                    
                };

                // Convertir l'objet en JSON
                string jsonPayload = JsonConvert.SerializeObject(livreFiltré);

                // 🔹 Encapsuler le JSON dans `x-www-form-urlencoded`
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // URL API avec l'ID du livre
                string url = $"livre/{livre.Id}";

                // Envoyer la requête PUT
                List<Document> liste = TraitementRecup<Document>(PUT, url, formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie un Document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool ModifierDocument(Document document)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(document.Id))
                {
                    return false;
                }

                // Construire l'objet JSON avec seulement `isbn` et `auteur`
                var documentFiltre = new
                {
                    titre = document.Titre,
                    image = document.Image,
                    idRayon = document.IdRayon,
                    idGenre = document.IdGenre,
                    idPublic = document.IdPublic
            };

                // Convertir l'objet en JSON
                string jsonPayload = JsonConvert.SerializeObject(documentFiltre);

                // 🔹 Encapsuler le JSON dans `x-www-form-urlencoded`
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // URL API avec l'ID du livre
                string url = $"document/{document.Id}";

                // Envoyer la requête PUT
                List<Document> liste = TraitementRecup<Document>(PUT, url, formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie un LivreDvd
        /// </summary>
        /// <param name="livredvd"></param>
        /// <returns></returns>
        public bool ModifierLivre_DvD(LivreDvd livredvd)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(livredvd.Id))
                {
                    return false;
                }

                // Construire l'objet JSON avec seulement `isbn` et `auteur`
                var LivreDvDFiltre = new
                {
                    id = livredvd.Titre
                };

                // Convertir l'objet en JSON
                string jsonPayload = JsonConvert.SerializeObject(LivreDvDFiltre);

                // 🔹 Encapsuler le JSON dans `x-www-form-urlencoded`
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // URL API avec l'ID du livre
                string url = $"document/{livredvd.Id}";

                // Envoyer la requête PUT
                List<Document> liste = TraitementRecup<Document>(PUT, url, formEncodedPayload);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public Livre GetLivre(string id)
        {
            // Récupérer tous les livres
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);

            // Vérifier si des livres ont été trouvés
            if (lesLivres == null || lesLivres.Count == 0)
            {
                return null;
            }

            // Filtrer pour trouver le livre correspondant à l'ID
            Livre livreTrouve = lesLivres.Find(livre => livre.Id == id);

            return livreTrouve;
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public Revue GetRevue(string id)
        {
            // Récupérer tous les livres
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);

            // Vérifier si des livres ont été trouvés
            if (lesRevues == null || lesRevues.Count == 0)
            {
                return null;
            }

            // Filtrer pour trouver le livre correspondant à l'ID
            Revue revueTrouve = lesRevues.Find(revue => revue.Id == id);

            return revueTrouve;
        }

        /// <summary>
        /// getter sur la liste des dvd
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public Dvd GetDvd(string id)
        {
            // Récupérer tous les livres
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);

            // Vérifier si des livres ont été trouvés
            if (lesDvd == null || lesDvd.Count == 0)
            {
                return null;
            }

            // Filtrer pour trouver le livre correspondant à l'ID
            Dvd dvdTrouve = lesDvd.Find(dvd => dvd.Id == id);

            return dvdTrouve;
        }

        /// <summary>
        /// Supprime un Livre
        /// </summary>
        /// <param name="livre"></param>
        /// <returns></returns>
        public bool SupprimerLivre(Livre livre)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(livre.Id))
                {
                    return false;
                }

                // Construire l'URL avec l'ID du livre au format JSON
                string jsonId = Uri.EscapeDataString($"{{\"id\":\"{livre.Id}\"}}");
                string url = $"livre/{jsonId}";

                // Envoyer la requête DELETE avec TraitementRecup
                List<Livre> liste = TraitementRecup<Livre>(DELETE, url, null);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime un Exemplairee
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns></returns>
        public bool SupprimerExemplaire(Exemplaire exemplaire)
        {
            try
            {
                // Construire l'URL avec l'ID du livre au format JSON
                string jsonId = Uri.EscapeDataString($"{{\"numero\":\"{exemplaire.Numero}\"}}");
                string url = $"exemplaire/{jsonId}";

                // Envoyer la requête DELETE avec TraitementRecup
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(DELETE, url, null);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime une Commande
        /// </summary>
        /// <param name="commande"></param>
        /// <returns></returns>
        public bool SupprimerCommande(Commande commande)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(commande.Id))
                {
                    return false;
                }

                // Construire l'URL avec l'ID du livre au format JSON
                string jsonId = Uri.EscapeDataString($"{{\"id\":\"{commande.Id}\"}}");
                string url = $"commande/{jsonId}";

                // Envoyer la requête DELETE avec TraitementRecup
                List<Commande> liste = TraitementRecup<Commande>(DELETE, url, null);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime un Abonnement
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool SupprimerAbonnement(Abonnement abonnement)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(abonnement.Id))
                {
                    return false;
                }

                // Construire l'URL avec l'ID du livre au format JSON
                string jsonId = Uri.EscapeDataString($"{{\"id\":\"{abonnement.Id}\"}}");
                string url = $"abonnement/{jsonId}";

                // Envoyer la requête DELETE avec TraitementRecup
                List<Abonnement> liste = TraitementRecup<Abonnement>(DELETE, url, null);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime une CommandeDocument
        /// </summary>
        /// <param name="commandeDocument"></param>
        /// <returns></returns>
        public bool SupprimerCommandeDocument(CommandeDocument commandeDocument)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(commandeDocument.Id))
                {
                    return false;
                }

                // Construire l'URL avec l'ID du livre au format JSON
                string jsonId = Uri.EscapeDataString($"{{\"id\":\"{commandeDocument.Id}\"}}");
                string url = $"commandedocument/{jsonId}";

                // Envoyer la requête DELETE avec TraitementRecup
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(DELETE, url, null);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime un Suivi
        /// </summary>
        /// <param name="suivi"></param>
        /// <returns></returns>
        public bool SupprimerSuivi(Suivi suivi)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(suivi.Id))
                {
                    return false;
                }

                // Construire l'URL avec l'ID du livre au format JSON
                string jsonId = Uri.EscapeDataString($"{{\"id\":\"{suivi.Id}\"}}");
                string url = $"suivi/{jsonId}";

                // Envoyer la requête DELETE avec TraitementRecup
                List<Suivi> liste = TraitementRecup<Suivi>(DELETE, url, null);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime un Dvd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns></returns>
        public bool SupprimerDvd(Dvd dvd)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(dvd.Id))
                {
                    return false;
                }

                // Construire l'URL avec l'ID du livre au format JSON
                string jsonId = Uri.EscapeDataString($"{{\"id\":\"{dvd.Id}\"}}");
                string url = $"dvd/{jsonId}";

                // Envoyer la requête DELETE avec TraitementRecup
                List<Livre> liste = TraitementRecup<Livre>(DELETE, url, null);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime une Revue
        /// </summary>
        /// <param name="revue"></param>
        /// <returns></returns>
        public bool SupprimerRevue(Revue revue)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(revue.Id))
                {
                    return false;
                }

                // Construire l'URL avec l'ID du livre au format JSON
                string jsonId = Uri.EscapeDataString($"{{\"id\":\"{revue.Id}\"}}");
                string url = $"revue/{jsonId}";

                // Envoyer la requête DELETE avec TraitementRecup
                List<Livre> liste = TraitementRecup<Livre>(DELETE, url, null);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime un LivreDvd
        /// </summary>
        /// <param name="livre"></param>
        /// <returns></returns>
        public bool SupprimerLivre_DvD(LivreDvd livre)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(livre.Id))
                {
                    return false;
                }

                // Construire l'URL avec l'ID du livre au format JSON
                string jsonId = Uri.EscapeDataString($"{{\"id\":\"{livre.Id}\"}}");
                string url = $"livres_dvd/{jsonId}";

                // Envoyer la requête DELETE avec TraitementRecup
                List<Livre> liste = TraitementRecup<Livre>(DELETE, url, null);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime un Document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool SupprimerDocument(Document document)
        {
            try
            {
                // Vérifier que l'ID du livre est présent
                if (string.IsNullOrEmpty(document.Id))
                {
                    return false;
                }

                // Construire l'URL avec l'ID du livre au format JSON
                string jsonId = Uri.EscapeDataString($"{{\"id\":\"{document.Id}\"}}");
                string url = $"document/{jsonId}";

                // Envoyer la requête DELETE avec TraitementRecup
                List<Livre> liste = TraitementRecup<Livre>(DELETE, url, null);

                if (liste == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T> (String methode, String message, String parametres)
        {
            // Initialisation des logs
            bddmanager.LoggerHelper.Initialize();

            List<T> liste = new List<T>();
            try
            {
                Log.Information("Méthode utilisée : {methode}, Message : {message}, Paramètres : {parametres}", methode, message, parametres);
                Log.Debug("Méthode: {methode}", methode);
                Log.Debug("Message: {message}", message);
                Log.Debug("Paramètres envoyés: {parametres}", parametres);
                
                JObject retour = api.RecupDistant(methode, message, parametres);
                Log.Debug("Retour API brut : {retour}", retour?.ToString());

                // extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());

                        Log.Information("Contenu brut de result : {result}", resultString);

                    }
                    // dans le cas du POST (insert), récupération de la liste d'objets
                    if (methode.Equals(POST))
                    {
                        Log.Information("Payload envoyé : {parametres}", parametres);
                        Log.Information("Réponse brute de l'API : {retour}", retour.ToString());
                    }
                }
                else
                {
                    Log.Warning("Code erreur = {code}, message = {message}", code, (string)retour["message"]);
                }
            
            }catch(Exception e)
            {
                Log.Fatal(e, "Erreur lors de l'accès à l'API");
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        /// <summary>
        /// Guetter d'un Utilisateur par son Login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Utilisateur GetUtilisateur(string login)
        {
            // Création de l'objet JSON à envoyer
            var champs = new Dictionary<string, string> { { "login", login } };

            // Sérialisation en format JSON
            string jsonPayload = JsonConvert.SerializeObject(champs, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            // Encodage en form-urlencoded
            string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

            // Appel API
            var result = TraitementRecup<Utilisateur>(GET, "utilisateur", formEncodedPayload);

            return result?.Find(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Vérifie la connexion
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsConnected(string login, string password)
        {
            try
            {
                // Requête vers l’API pour récupérer l’utilisateur avec ce login
                var champs = new Dictionary<string, string> { { "login", login } };
                string jsonPayload = JsonConvert.SerializeObject(champs);
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Appel API
                List<Utilisateur> utilisateurs = TraitementRecup<Utilisateur>(GET, "utilisateur", formEncodedPayload);
                if (utilisateurs == null || utilisateurs.Count == 0) return false;

                var user = utilisateurs.FirstOrDefault();
                if (user == null) return false;

                // Hash du mot de passe entré
                string pwdHash = HashPassword(password);

                // Comparaison
                return user.Password == pwdHash;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Permet d'hasher un mot de passe
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
