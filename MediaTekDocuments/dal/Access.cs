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
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
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


        private Dictionary<string, Genre> classeurGenres = new Dictionary<string, Genre>();
        private Dictionary<string, Public> classeurPublics = new Dictionary<string, Public>();
        private Dictionary<string, Rayon> classeurRayons = new Dictionary<string, Rayon>();


        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            String authenticationString;
            try
            {
                authenticationString = "admin:adminpwd";
                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if(instance == null)
            {
                instance = new Access();
            }
            return instance;
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
                else
                {
                    Console.WriteLine($"⚠️ Aucune commande trouvée pour l'abonnement {abonnement.Id}");
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
                else
                {
                    Console.WriteLine($"⚠️ Aucune commande trouvée pour l'abonnement {abonnement.Id}");
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
            return result?.FirstOrDefault(cmd => cmd.Id == id);
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
            return result != null && result.Any(cmd => cmd.IdRevue == id);
        }

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
                Console.WriteLine("🔍 [DEBUG] ID reçu: " + id);

                // 🔹 Conversion en JSON pour l'URL (comme Postman)
                string jsonId = Uri.EscapeDataString(JsonConvert.SerializeObject(new { id }));

                // 🔹 Création de l'URL avec le JSON directement
                string url = $"exemplaire/{jsonId}";

                Console.WriteLine("📤 [DEBUG] URL de la requête : " + url);

                // 🔹 Appel de l'API
                var result = TraitementRecup<Exemplaire>(GET, url, ""); // Pas de body

                Console.WriteLine($"📥 [DEBUG] Résultat brut de l'API : {JsonConvert.SerializeObject(result)}");

                // 🔹 Vérification et retour de l'exemplaire trouvé
                return result?.FirstOrDefault(ex => ex.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception : {ex.Message}");
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
            return result?.FirstOrDefault(cmd => cmd.Id == id);
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

        //en construction
        public bool AjouterLivreDvD(object livreDvd)
        {
            //effectuer une requete API de POST dans la table livredvd avec id
            try
            {
                // Sélectionner uniquement les champs requis
                var docDict = JObject.FromObject(livreDvd);

                Console.WriteLine("Contenu de document : " + JsonConvert.SerializeObject(livreDvd, Formatting.Indented));


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

                // Log du format envoyé
                Console.WriteLine("Payload encodé envoyé : " + formEncodedPayload);

                // Envoyer la requête en POST avec application/x-www-form-urlencoded
                List<Document> liste = TraitementRecup<Document>(POST, "livres_dvd", formEncodedPayload);

                if (liste == null)
                {
                    Console.WriteLine("Erreur : La liste retournée par l'API est null.");
                    return false;
                }


                // Log de la réponse API
                Console.WriteLine($"Réponse API: {liste}");

                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'ajout du document : " + ex.Message);
                return false;
            }
        }

        public bool AjouterAbonnement(Abonnement abonnement)
        {
            try
            {
                // Vérification et extraction des données
                var docDict = JObject.Parse(JsonConvert.SerializeObject(abonnement));


                Console.WriteLine("📌 Contenu de l'exemplaire : " + JsonConvert.SerializeObject(abonnement, Formatting.Indented));

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
                    Console.WriteLine("❌ Erreur : Champs obligatoires manquants !");
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

                // Log du format envoyé
                Console.WriteLine("🚀 Payload envoyé : " + formEncodedPayload);

                // Envoi de la requête POST
                List<Abonnement> liste = TraitementRecup<Abonnement>(POST, "abonnement", formEncodedPayload);

                if (liste == null)
                {
                    Console.WriteLine("⚠️ Erreur : La liste retournée par l'API est null.");
                    return false;
                }

                // Log de la réponse API
                Console.WriteLine($"✅ Réponse API : {JsonConvert.SerializeObject(liste, Formatting.Indented)}");

                return liste != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 Erreur lors de l'ajout de l'exemplaire : " + ex.Message);
                return false;
            }

        }

        public bool AjouterExemplaire(Exemplaire exemplaire)
        {
            try
            {
                // Vérification et extraction des données
                var docDict = JObject.Parse(JsonConvert.SerializeObject(exemplaire));


                Console.WriteLine("📌 Contenu de l'exemplaire : " + JsonConvert.SerializeObject(exemplaire, Formatting.Indented));

                // Extraction et conversion des champs
                var exemplaireTriage = new
                {
                    id = docDict["Id"]?.ToString() ?? "MISSING_ID",
                    dateAchat = docDict["DateAchat"] != null && DateTime.TryParse(docDict["DateAchat"].ToString(), out DateTime date)
                ? date.ToString("yyyy-MM-dd") // Formatage pour MySQL
                : null,
                    photo = docDict["Photo"]?.ToString() ?? "",
                    idEtat = docDict["IdEtat"]?.ToString() ?? "MISSING_IDETAT"
                };

                // Vérification avant envoi
                if (string.IsNullOrEmpty(exemplaireTriage.id) || string.IsNullOrEmpty(exemplaireTriage.idEtat))
                {
                    Console.WriteLine("❌ Erreur : Champs obligatoires manquants !");
                    return false;
                }

                // Sérialisation en JSON
                string jsonPayload = JsonConvert.SerializeObject(
                    exemplaireTriage,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    }
                );

                // Encapsuler dans "champs=" et encoder en application/x-www-form-urlencoded
                string formEncodedPayload = $"champs={Uri.EscapeDataString(jsonPayload)}";

                // Log du format envoyé
                Console.WriteLine("🚀 Payload envoyé : " + formEncodedPayload);

                // Envoi de la requête POST
                List<Document> liste = TraitementRecup<Document>(POST, "exemplaire", formEncodedPayload);

                if (liste == null)
                {
                    Console.WriteLine("⚠️ Erreur : La liste retournée par l'API est null.");
                    return false;
                }

                // Log de la réponse API
                Console.WriteLine($"✅ Réponse API : {JsonConvert.SerializeObject(liste, Formatting.Indented)}");

                return liste != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 Erreur lors de l'ajout de l'exemplaire : " + ex.Message);
                return false;
            }

        }

        public bool AjouterCommande(Commande commande)
        {
            try
            {
                // Vérification et extraction des données
                var docDict = JObject.Parse(JsonConvert.SerializeObject(commande));


                Console.WriteLine("📌 Contenu de la commande : " + JsonConvert.SerializeObject(commande, Formatting.Indented));

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

                // Log du format envoyé
                Console.WriteLine("Payload envoyé : " + formEncodedPayload);

                // Envoi de la requête POST
                List<Document> liste = TraitementRecup<Document>(POST, "commande", formEncodedPayload);

                if (liste == null)
                {
                    Console.WriteLine("Erreur : La liste retournée par l'API est null.");
                    return false;
                }

                // Log de la réponse API
                Console.WriteLine($"Réponse API : {JsonConvert.SerializeObject(liste, Formatting.Indented)}");

                return liste != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'ajout de l'exemplaire : " + ex.Message);
                return false;
            }

        }

        public bool AjouterCommandeDocument(CommandeDocument commandeDocument)
        {
            try
            {
                // Vérification et extraction des données
                var docDict = JObject.Parse(JsonConvert.SerializeObject(commandeDocument));


                Console.WriteLine("📌 Contenu de la commande : " + JsonConvert.SerializeObject(commandeDocument, Formatting.Indented));

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

                // Log du format envoyé
                Console.WriteLine("Payload envoyé : " + formEncodedPayload);

                // Envoi de la requête POST
                List<Document> liste = TraitementRecup<Document>(POST, "commandedocument", formEncodedPayload);

                if (liste == null)
                {
                    Console.WriteLine("Erreur : La liste retournée par l'API est null.");
                    return false;
                }

                // Log de la réponse API
                Console.WriteLine($"Réponse API : {JsonConvert.SerializeObject(liste, Formatting.Indented)}");

                return liste != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'ajout de l'exemplaire : " + ex.Message);
                return false;
            }

        }

        public bool AjouterSuivi(Suivi suivi)
        {
            try
            {
                // Vérification et extraction des données
                var docDict = JObject.Parse(JsonConvert.SerializeObject(suivi));


                Console.WriteLine("📌 Contenu de la commande : " + JsonConvert.SerializeObject(suivi, Formatting.Indented));

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

                // Log du format envoyé
                Console.WriteLine("Payload envoyé : " + formEncodedPayload);

                // Envoi de la requête POST
                List<Document> liste = TraitementRecup<Document>(POST, "suivi", formEncodedPayload);

                if (liste == null)
                {
                    Console.WriteLine("Erreur : La liste retournée par l'API est null.");
                    return false;
                }

                // Log de la réponse API
                Console.WriteLine($"Réponse API : {JsonConvert.SerializeObject(liste, Formatting.Indented)}");

                return liste != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'ajout de l'exemplaire : " + ex.Message);
                return false;
            }

        }

        public bool AjouterRevue(object revue)
        {
            //effectuer une requete API de POST dans la table livre avec id, isbn, auteur et collection

            try
            {
                // Sélectionner uniquement les champs requis
                var docDict = JObject.FromObject(revue);

                Console.WriteLine("Contenu de document : " + JsonConvert.SerializeObject(revue, Formatting.Indented));


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

                // Log du format envoyé
                Console.WriteLine("Payload encodé envoyé : " + formEncodedPayload);

                // Envoyer la requête en POST avec application/x-www-form-urlencoded
                List<Document> liste = TraitementRecup<Document>(POST, "revue", formEncodedPayload);

                if (liste == null)
                {
                    Console.WriteLine("Erreur : La liste retournée par l'API est null.");
                    return false;
                }


                // Log de la réponse API
                Console.WriteLine($"Réponse API: {liste}");

                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'ajout du document : " + ex.Message);
                return false;
            }


            return true;
        }

        public bool AjouterLivre(object livre)
        {
            //effectuer une requete API de POST dans la table livre avec id, isbn, auteur et collection

            try
            {
                // Sélectionner uniquement les champs requis
                var docDict = JObject.FromObject(livre);

                Console.WriteLine("Contenu de document : " + JsonConvert.SerializeObject(livre, Formatting.Indented));


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

                // Log du format envoyé
                Console.WriteLine("Payload encodé envoyé : " + formEncodedPayload);

                // Envoyer la requête en POST avec application/x-www-form-urlencoded
                List<Document> liste = TraitementRecup<Document>(POST, "livre", formEncodedPayload);

                if (liste == null)
                {
                    Console.WriteLine("Erreur : La liste retournée par l'API est null.");
                    return false;
                }


                // Log de la réponse API
                Console.WriteLine($"Réponse API: {liste}");

                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'ajout du document : " + ex.Message);
                return false;
            }


            return true;
        }

        public bool AjouterDocument(object document)
        {
            try
            {
                // Sélectionner uniquement les champs requis
                var docDict = JObject.FromObject(document);

                Console.WriteLine("Contenu de document : " + JsonConvert.SerializeObject(document, Formatting.Indented));


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

                // Log du format envoyé
                Console.WriteLine("Payload encodé envoyé : " + formEncodedPayload);

                // Envoyer la requête en POST avec application/x-www-form-urlencoded
                List<Document> liste = TraitementRecup<Document>(POST, "document", formEncodedPayload);

                if (liste == null)
                {
                    Console.WriteLine("Erreur : La liste retournée par l'API est null.");
                    return false;
                }


                // Log de la réponse API
                Console.WriteLine($"Réponse API: {liste}");

                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'ajout du document : " + ex.Message);
                return false;
            }
        }

        public bool AjouterDvd(object dvd)
        {
            //effectuer une requete API de POST dans la table livre avec id, isbn, auteur et collection

            try
            {
                // Sélectionner uniquement les champs requis
                var docDict = JObject.FromObject(dvd);

                Console.WriteLine("Contenu de document : " + JsonConvert.SerializeObject(dvd, Formatting.Indented));


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

                // Log du format envoyé
                Console.WriteLine("Payload encodé envoyé : " + formEncodedPayload);

                // Envoyer la requête en POST avec application/x-www-form-urlencoded
                List<Document> liste = TraitementRecup<Document>(POST, "dvd", formEncodedPayload);

                if (liste == null)
                {
                    Console.WriteLine("Erreur : La liste retournée par l'API est null.");
                    return false;
                }


                // Log de la réponse API
                Console.WriteLine($"Réponse API: {liste}");

                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'ajout du document : " + ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 
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
        /// 
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
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", "champs=" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public void DictionnaireGenre()
        {
            List<Categorie> genres = GetAllGenres();

            foreach (var genre in genres)
            {
                classeurGenres[genre.Id] = (Genre)genre; // Conversion Categorie -> Genre si nécessaire
            }

            Console.WriteLine("📂 Classeur des genres créé avec succès !");

        }

        public void DictionnairePublic()
        {
            List<Categorie> publics = GetAllPublics();

            foreach (var pub in publics)
            {
                classeurPublics[pub.Id] = (Public)pub; // Conversion Categorie -> Public si nécessaire
            }

            Console.WriteLine("📂 Classeur des genres créé avec succès !");

        }

        public void DictionnaireRayon()
        {
            List<Categorie> rayons = GetAllRayons();

            foreach (var rayon in rayons)
            {
                classeurRayons[rayon.Id] = (Rayon)rayon; // Conversion Categorie -> Public si nécessaire
            }

            Console.WriteLine("📂 Classeur des genres créé avec succès !");

        }

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
            catch (Exception ex)
            {
                return false;
            }
        }

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

                Console.WriteLine("Date de fin = " + abonnement.DateFinAbonnement);

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
            catch (Exception ex)
            {
                return false;
            }
        }

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

                Console.WriteLine("Montant = " + commande.Montant);

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
            catch (Exception ex)
            {
                return false;
            }
        }

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
            catch (Exception ex)
            {
                return false;
            }
        }

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
            catch (Exception ex)
            {
                return false;
            }
        }

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
            catch (Exception ex)
            {
                return false;
            }
        }

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
            catch (Exception ex)
            {
                return false;
            }
        }

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
            catch (Exception ex)
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
                Console.WriteLine("⚠️ Aucun livre trouvé dans la base.");
                return null;
            }

            // Filtrer pour trouver le livre correspondant à l'ID
            Livre livreTrouve = lesLivres.FirstOrDefault(livre => livre.Id == id);

            if (livreTrouve == null)
            {
                Console.WriteLine($"❌ Aucun livre trouvé avec l'ID : {id}");
            }
            else
            {
                Console.WriteLine($"✅ Livre trouvé : {livreTrouve.Titre} (ID: {livreTrouve.Id})");
            }

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
                Console.WriteLine("⚠️ Aucun livre trouvé dans la base.");
                return null;
            }

            // Filtrer pour trouver le livre correspondant à l'ID
            Revue revueTrouve = lesRevues.FirstOrDefault(revue => revue.Id == id);

            if (revueTrouve == null)
            {
                Console.WriteLine($"❌ Aucun livre trouvé avec l'ID : {id}");
            }
            else
            {
                Console.WriteLine($"✅ Livre trouvé : {revueTrouve.Titre} (ID: {revueTrouve.Id})");
            }

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
                Console.WriteLine("⚠️ Aucun livre trouvé dans la base.");
                return null;
            }

            // Filtrer pour trouver le livre correspondant à l'ID
            Dvd dvdTrouve = lesDvd.FirstOrDefault(dvd => dvd.Id == id);

            if (dvdTrouve == null)
            {
                Console.WriteLine($"❌ Aucun dvd trouvé avec l'ID : {id}");
            }
            else
            {
                Console.WriteLine($"✅ dvd trouvé : {dvdTrouve.Titre} (ID: {dvdTrouve.Id})");
            }

            return dvdTrouve;
        }

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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression livre {livre.Id} : {ex.Message}");
                return false;
            }
        }

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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression commande {commande.Id} : {ex.Message}");
                return false;
            }
        }

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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression commande {abonnement.Id} : {ex.Message}");
                return false;
            }
        }

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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression commmandeDocument {commandeDocument.Id} : {ex.Message}");
                return false;
            }
        }

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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression suivi {suivi.Id} : {ex.Message}");
                return false;
            }
        }


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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression livre {dvd.Id} : {ex.Message}");
                return false;
            }
        }

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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression livre {revue.Id} : {ex.Message}");
                return false;
            }
        }

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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression livre {livre.Id} : {ex.Message}");
                return false;
            }
        }

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
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression livre {document.Id} : {ex.Message}");
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
            // trans
            List<T> liste = new List<T>();
            try
            {
                Console.WriteLine($"Méthode utilisée : {methode}, Message : {message}, Paramètres : {parametres}");

                Console.WriteLine($"🔍 [DEBUG] Méthode: {methode}");
                Console.WriteLine($"🔍 [DEBUG] Message: {message}");
                Console.WriteLine($"🔍 [DEBUG] Paramètres envoyés: {parametres}");



                JObject retour = api.RecupDistant(methode, message, parametres);

                Console.WriteLine(retour);
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

                        Console.WriteLine("Contenu brut de result : " + resultString);

                    }
                    // dans le cas du POST (insert), récupération de la liste d'objets
                    if (methode.Equals(POST))
                    {
                        Console.WriteLine($"Payload envoyé : {parametres}");
                        Console.WriteLine("Réponse brute de l'API : " + retour.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }catch(Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : "+e.Message);
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
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
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

    }
}
