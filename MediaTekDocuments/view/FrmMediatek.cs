using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        // Déclaration des variables
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgEtats = new BindingSource();
        private FrmAjouterLivre FrmAjouterLivre;
        private FrmModifierLivre FrmModifierLivre;
        private FrmAjouterRevue FrmAjouterRevue;
        private FrmAjouterDvD FrmAjouterDvD;
        private FrmModifierDvD FrmModifierDvD;
        private FrmModifierRevue FrmModifierRevue;
        private Utilisateur utilisateur;

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek(Utilisateur utilisateur)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this.FrmAjouterLivre = new FrmAjouterLivre(this);
            this.FrmAjouterRevue = new FrmAjouterRevue(this);
            this.FrmAjouterDvD = new FrmAjouterDvD(this);
            this.utilisateur = utilisateur;

            // Traitement de la visibilité des boutons selon le type d'utilisateur
            if (utilisateur.IdService == "00002") // Service Prêts
            {
                tabOngletsApplication.TabPages.Remove(tabReceptionRevue);
                tabOngletsApplication.TabPages.Remove(tabCommandesLivres);
                tabOngletsApplication.TabPages.Remove(tabCommandesDvd);
                tabOngletsApplication.TabPages.Remove(tabCommandesRevues);

                btn_modifierLivre.Visible = false;
                btn_supprimerLivre.Visible = false;
                btn_ajouterLivre.Visible = false;

                button1.Visible = false;
                button2.Visible = false;
                cbxLivresEtats.Visible = false;

                btn_modifierDVD.Visible = false;
                btn_supprimerDVD.Visible = false;
                btn_ajouterDVD.Visible = false;

                button12.Visible = false;
                button4.Visible = false;
                cbxDvdEtats.Visible = false;

                btn_ajouterRevue.Visible = false;
                btn_modifierRevue.Visible = false;
                btn_supprimerRevue.Visible = false;

            }

            this.tabCommandesLivres.Enter += new System.EventHandler(this.tabCommandesLivres_Enter);
            Console.WriteLine("✅ Événement tabCommandesLivres_Enter attaché !");
            this.tabCommandesDvd.Enter += new System.EventHandler(this.tabCommandesDvd_Enter);
            Console.WriteLine("✅ Événement tabCommandesDvd_Enter attaché !");
            this.tabCommandesRevues.Enter += new System.EventHandler(this.tabCommandesRevues_Enter);
            Console.WriteLine("✅ Événement tabCommandesLivres_Enter attaché !");
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboEtat(List<Etat> lesEtats, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesEtats;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private readonly BindingSource bdgCommandesLivresListe = new BindingSource();
        private readonly BindingSource bdgCommandesDvdListe = new BindingSource();
        private readonly BindingSource bdgCommandesRevueListe = new BindingSource();
        private readonly BindingSource bdgLivresExemplairesListe = new BindingSource();
        private readonly BindingSource bdgDvdExemplairesListe = new BindingSource();
        public List<Livre> lesLivres = new List<Livre>();
        public List<CommandeDocument> lesCommandesDocument = new List<CommandeDocument>();
        public List<Abonnement> lesAbonnements = new List<Abonnement>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cbxLivresEtats);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Méthode qui remplit le dgv des exemplaires de Livres
        /// </summary>
        private void RemplirLivresExemplaires()
        {
            if (dgvLivresListe.CurrentCell != null && bdgLivresListe.Current is Livre livre)
            {
                var exemplaires = controller.GetExemplairesLivre(livre.Id);
                var etats = controller.GetAllEtats();

                Console.WriteLine($"📘 Livre sélectionné : {livre.Id} ({livre.Titre})");
                Console.WriteLine("📚 Exemplaires associés :");
                foreach (var ex in exemplaires)
                {
                    Console.WriteLine($"   - Numéro : {ex.Numero}, Date achat : {ex.DateAchat:yyyy-MM-dd}, État : {ex.IdEtat}");
                }

                foreach (var ex in exemplaires)
                {
                    var etat = etats.Find(e => e.Id == ex.IdEtat);
                    ex.LibelleEtat = etat != null ? etat.Libelle : "Inconnu";
                }

                bdgLivresExemplairesListe.DataSource = exemplaires;
                dgvLivresExemplaires.DataSource = bdgLivresExemplairesListe;
                dgvLivresExemplaires.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                // Réordonner les colonnes pour afficher "Numéro du livre" en premier
                dgvLivresExemplaires.Columns["id"].DisplayIndex = 0;
                dgvLivresExemplaires.Columns["numero"].DisplayIndex = 1;
                dgvLivresExemplaires.Columns["dateAchat"].DisplayIndex = 2;
                dgvLivresExemplaires.Columns["photo"].DisplayIndex = 3;
                dgvLivresExemplaires.Columns["idEtat"].DisplayIndex = 4;

                // Renommer les colonnes
                dgvLivresExemplaires.Columns["id"].HeaderText = "Numéro du livre";
                dgvLivresExemplaires.Columns["dateAchat"].HeaderText = "Date d'achat";
                dgvLivresExemplaires.Columns["numero"].HeaderText = "Numéro d'exemplaire";
                dgvLivresExemplaires.Columns["photo"].HeaderText = "Photo";
                dgvLivresExemplaires.Columns["LibelleEtat"].HeaderText = "État";

                // Masquer des colonnes
                dgvLivresExemplaires.Columns["IdEtat"].Visible = false;
            }
        }

        /// <summary>
        /// Méthode qui remplit le dgv des exemplaires de Dvd
        /// </summary>
        private void RemplirDvdExemplaires()
        {
            if (dgvDvdListe.CurrentCell != null && bdgDvdListe.Current is Dvd dvd)
            {
                var exemplaires = controller.GetExemplairesDvd(dvd.Id);
                var etats = controller.GetAllEtats();

                Console.WriteLine($"Dvd sélectionné : {dvd.Id} ({dvd.Titre})");
                Console.WriteLine("Exemplaires associés :");
                foreach (var ex in exemplaires)
                {
                    Console.WriteLine($"   - Numéro : {ex.Numero}, Date achat : {ex.DateAchat:yyyy-MM-dd}, État : {ex.IdEtat}");
                }

                foreach (var ex in exemplaires)
                {
                    var etat = etats.Find(e => e.Id == ex.IdEtat);
                    ex.LibelleEtat = etat != null ? etat.Libelle : "Inconnu";
                }

                bdgDvdExemplairesListe.DataSource = exemplaires;
                dgvDvdExemplaires.DataSource = bdgDvdExemplairesListe;
                dgvDvdExemplaires.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                // Réordonner les colonnes pour afficher "Numéro du livre" en premier
                dgvDvdExemplaires.Columns["id"].DisplayIndex = 0;
                dgvDvdExemplaires.Columns["numero"].DisplayIndex = 1;
                dgvDvdExemplaires.Columns["dateAchat"].DisplayIndex = 2;
                dgvDvdExemplaires.Columns["photo"].DisplayIndex = 3;
                dgvDvdExemplaires.Columns["idEtat"].DisplayIndex = 4;

                // Renommer les colonnes
                dgvDvdExemplaires.Columns["id"].HeaderText = "Numéro du livre";
                dgvDvdExemplaires.Columns["dateAchat"].HeaderText = "Date d'achat";
                dgvDvdExemplaires.Columns["numero"].HeaderText = "Numéro d'exemplaire";
                dgvDvdExemplaires.Columns["photo"].HeaderText = "Photo";
                dgvDvdExemplaires.Columns["LibelleEtat"].HeaderText = "État";

                // Masquer des colonnes
                dgvDvdExemplaires.Columns["IdEtat"].Visible = false;
            }
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Remplit le DataGridView avec la liste des commandes de documents
        /// </summary>
        /// <param name="commandesDocument">Liste des commandes de documents</param>
        private void RemplirCommandesDocumentLivresListe(List<CommandeDocument> commandesDocument)
        {           
            lesCommandesEntieres.Clear(); // On vide la liste avant de la remplir

            foreach (var commandeDoc in commandesDocument)
            {
                Commande commande = controller.GetCommandeById(commandeDoc.Id);
                Suivi suivi = controller.GetSuiviById(commandeDoc.Id);

                if (commande != null)
                {
                    lesCommandesEntieres.Add(new CommandeEntiere(commande, commandeDoc, suivi));
                }
                else
                {
                    Console.WriteLine($"Aucune commande trouvée pour l'ID : {commandeDoc.Id}");
                }
            }

            // Mise à jour du DataGridView avec la liste fusionnée
            bdgCommandesLivresListe.DataSource = lesCommandesEntieres;
            dgv_commandesLivres.DataSource = bdgCommandesLivresListe;
            dgv_commandesLivres.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            Console.WriteLine($"{lesCommandesEntieres.Count} commandes affichées dans le DataGridView !");

            // Renommer les colonnes
            dgv_commandesLivres.Columns["Id"].HeaderText = "Numéro de commande";
            dgv_commandesLivres.Columns["DateCommande"].HeaderText = "Date de commande";
            dgv_commandesLivres.Columns["Montant"].HeaderText = "Montant (€)";
            dgv_commandesLivres.Columns["NbExemplaire"].HeaderText = "Nombre d'exemplaires";
            dgv_commandesLivres.Columns["IdLivreDvd"].HeaderText = "Numéro du livre";
            dgv_commandesLivres.Columns["stade"].HeaderText = "État de la commande";

            // Réordonner les colonnes pour afficher "Numéro du livre" en premier
            dgv_commandesLivres.Columns["IdLivreDvd"].DisplayIndex = 0;
            dgv_commandesLivres.Columns["Id"].DisplayIndex = 1;
            dgv_commandesLivres.Columns["DateCommande"].DisplayIndex = 2;
            dgv_commandesLivres.Columns["Montant"].DisplayIndex = 3;
            dgv_commandesLivres.Columns["NbExemplaire"].DisplayIndex = 4;
            dgv_commandesLivres.Columns["stade"].DisplayIndex = 5;


        }

        /// <summary>
        /// Remplit le DataGridView avec la liste des commandes de documents
        /// </summary>
        /// <param name="commandesDocument">Liste des commandes de documents</param>
        private void RemplirCommandesDocumentRevueListe(List<Abonnement> abonnement)
        {
            if(utilisateur.IdService == "00001")
            {
                // Mise à jour du DataGridView avec la liste fusionnée
                bdgCommandesRevueListe.DataSource = lesAbonnements;
                dgv_commandesRevue.DataSource = bdgCommandesRevueListe;
                dgv_commandesRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                Console.WriteLine($"{lesAbonnements.Count} commandes affichées dans le DataGridView !");

                // Renommer les colonnes
                dgv_commandesRevue.Columns["Id"].HeaderText = "Numéro de commande";
                dgv_commandesRevue.Columns["DateCommande"].HeaderText = "Date de commande";
                dgv_commandesRevue.Columns["Montant"].HeaderText = "Montant (€)";
                dgv_commandesRevue.Columns["DateFinAbonnement"].HeaderText = "Date de résiliation";
                dgv_commandesRevue.Columns["IdRevue"].HeaderText = "Numéro de revue";

                // Réordonner les colonnes pour afficher "Numéro du livre" en premier
                dgv_commandesRevue.Columns["IdRevue"].DisplayIndex = 1;
                dgv_commandesRevue.Columns["Id"].DisplayIndex = 2;
                dgv_commandesRevue.Columns["DateCommande"].DisplayIndex = 3;
                dgv_commandesRevue.Columns["DateFinAbonnement"].DisplayIndex = 3;
                dgv_commandesRevue.Columns["Montant"].DisplayIndex = 4;

                // Cacher les doublons
                dgv_commandesRevue.Columns["IdCommande"].Visible = false;
            }
        }

        /// <summary>
        /// Remplit le DataGridView avec la liste des commandes de documents
        /// </summary>
        /// <param name="commandesDocument">Liste des commandes de documents</param>
        private void RemplirCommandesDocumentDvdListe(List<CommandeDocument> commandesDocument)
        {
            lesCommandesEntieres.Clear(); // On vide la liste avant de la remplir

            foreach (var commandeDoc in commandesDocument)
            {
                Commande commande = controller.GetCommandeById(commandeDoc.Id);
                Suivi suivi = controller.GetSuiviById(commandeDoc.Id);

                if (commande == null)
                {
                    Console.WriteLine($"❌ Aucune commande trouvée pour l'ID : {commandeDoc.Id}");
                    continue;
                }

                // Vérifier si le suivi est NULL et en créer un par défaut si besoin
                if (suivi == null)
                {
                    Console.WriteLine($"⚠️ Aucun suivi trouvé pour l'ID : {commandeDoc.Id}. Création d'un suivi par défaut.");
                    suivi = new Suivi(commandeDoc.Id, "en cours", commandeDoc.IdLivreDvd);
                }

                // Maintenant qu'on est sûr que commande et suivi sont bien définis, on peut créer l'objet
                lesCommandesEntieres.Add(new CommandeEntiere(commande, commandeDoc, suivi));
            }

            // Mise à jour du DataGridView avec la liste fusionnée
            bdgCommandesDvdListe.DataSource = lesCommandesEntieres;
            dgv_commandesDvd.DataSource = bdgCommandesDvdListe;
            dgv_commandesDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            Console.WriteLine($"{lesCommandesEntieres.Count} commandes affichées dans le DataGridView !");

            // Renommer les colonnes
            dgv_commandesDvd.Columns["Id"].HeaderText = "Numéro de commande";
            dgv_commandesDvd.Columns["DateCommande"].HeaderText = "Date de commande";
            dgv_commandesDvd.Columns["Montant"].HeaderText = "Montant (€)";
            dgv_commandesDvd.Columns["NbExemplaire"].HeaderText = "Nombre d'exemplaires";
            dgv_commandesDvd.Columns["IdLivreDvd"].HeaderText = "Numéro du dvd";
            dgv_commandesDvd.Columns["stade"].HeaderText = "État de la commande";

            // Réordonner les colonnes pour afficher "Numéro du livre" en premier
            dgv_commandesDvd.Columns["IdLivreDvd"].DisplayIndex = 0;
            dgv_commandesDvd.Columns["Id"].DisplayIndex = 1;
            dgv_commandesDvd.Columns["DateCommande"].DisplayIndex = 2;
            dgv_commandesDvd.Columns["Montant"].DisplayIndex = 3;
            dgv_commandesDvd.Columns["NbExemplaire"].DisplayIndex = 4;
            dgv_commandesDvd.Columns["stade"].DisplayIndex = 5;


        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheCommandesLivresInfos(Livre livre, CommandeEntiere commande)
        {
            txbCLivresAuteur.Text = livre.Auteur;
            txbCLivresCollection.Text = livre.Collection;
            txbCLivresImage.Text = livre.Image;
            txbCLivresIsbn.Text = livre.Isbn;
            txbCLivresNumeroCom.Text = commande.Id;
            txbCLivresGenre.Text = livre.Genre;
            txbCLivresPublic.Text = livre.Public;
            txbCLivresRayon.Text = livre.Rayon;
            txbCLivresTitre.Text = livre.Titre;
            txbCLivresNumeroDoc.Text = commande.IdLivreDvd;
            txbCLivresStade.Text = commande.stade;
            txbDate.Text = commande.DateCommande.ToString();
            txbNombre.Text = commande.NbExemplaire.ToString();
            string image = livre.Image;
            try
            {
                pcbCLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbCLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheCommandesRevueInfos(Revue revue, Abonnement abonnement)
        {
            txbCRevueImage.Text = revue.Image;
            txbCRevueNumeroCom.Text = abonnement.Id;
            txbCRevueGenre.Text = revue.Genre;
            txbCRevuePublic.Text = revue.Public;
            txbCRevueRayon.Text = revue.Rayon;
            txbCRevuePeriodicite.Text = revue.Periodicite;
            txbCRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbCRevueTitre.Text = revue.Titre;
            txbCRevueNumeroDoc.Text = abonnement.IdRevue;
            txbCRevueDate.Text = abonnement.DateCommande.ToString();
            txbCRevueDateFin.Text = abonnement.DateFinAbonnement.ToString();

            Exemplaire exemplaire = controller.GetExemplaireById(revue.Id);

            txbCRevueDateParution.Text = exemplaire.DateAchat.ToString();
            string image = revue.Image;
            try
            {
                pcbCRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbCRevueImage.Image = null;
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheCommandesDvdInfos(Dvd dvd, CommandeEntiere commande)
        {
            txbCDvdRealisateur.Text = dvd.Realisateur;
            txbCDvdSynopsis.Text = dvd.Synopsis;
            txbCDvdImage.Text = dvd.Image;
            txbCDvdDuree.Text = dvd.Duree.ToString();
            txbCDvdNumeroCom.Text = commande.Id;
            txbCDvdGenre.Text = dvd.Genre;
            txbCDvdPublic.Text = dvd.Public;
            txbCDvdRayon.Text = dvd.Rayon;
            txbCDvdTitre.Text = dvd.Titre;
            txbCDvdNumeroDoc.Text = commande.IdLivreDvd;
            txbCDvdStade.Text = commande.stade;
            txbCDvdDate.Text = commande.DateCommande.ToString();
            txbCDvdNombre.Text = commande.NbExemplaire.ToString();
            string image = dvd.Image;
            try
            {
                pcbCDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbCDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideCommandesLivresInfos()
        {
            txbCLivresAuteur.Text = "";
            txbCLivresCollection.Text = "";
            txbCLivresImage.Text = "";
            txbCLivresIsbn.Text = "";
            txbCLivresNumeroDoc.Text = "";
            txbCLivresGenre.Text = "";
            txbCLivresPublic.Text = "";
            txbCLivresRayon.Text = "";
            txbCLivresTitre.Text = "";
            pcbCLivresImage.Image = null;
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideCommandesRevueInfos()
        {
            txbCRevueImage.Text = "";
            txbCRevueNumeroDoc.Text = "";
            txbCRevueGenre.Text = "";
            txbCRevuePublic.Text = "";
            txbCRevueRayon.Text = "";
            txbCRevueTitre.Text = "";
            pcbCRevueImage.Image = null;
        }

        /// <summary>
        /// Méthode qui vide les infos des commandes de Dvd
        /// </summary>
        private void VideCommandesDvdInfos()
        {
            txbCDvdRealisateur.Text = "";
            txbCDvdNumeroDoc.Text = "";
            txbCDvdImage.Text = "";
            txbCDvdNumeroCom.Text = "";
            txbCDvdGenre.Text = "";
            txbCDvdPublic.Text = "";
            txbCDvdRayon.Text = "";
            txbCDvdTitre.Text = "";
            pcbCDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                    RemplirLivresExemplaires();
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }


        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        public void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideCommandesLivresZones()
        {
            txbCLivresNumRechercheDoc.Text = "";
            txbCLivresNumRechercheCom.Text = "";
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideCommandesRevueZones()
        {
            txbCRevueNumRechercheDoc.Text = "";
            txbCRevueNumRechercheCom.Text = "";
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideCommandesDvdZones()
        {
            txbCDvdNumRechercheDoc.Text = "";
            txbCDvdNumRechercheCom.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        public List<Dvd> lesDvd = new List<Dvd>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cbxDvdEtats);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                    RemplirDvdExemplaires();
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        public void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        public List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        public void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();

        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
            RemplirComboEtat(controller.GetAllEtats(), bdgEtats, cbxParutionsEtats);
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                var etats = controller.GetAllEtats();

                foreach (var ex in exemplaires)
                {
                    Console.WriteLine($"   - Numéro : {ex.Numero}, Date achat : {ex.DateAchat:yyyy-MM-dd}, État : {ex.IdEtat}");
                }

                foreach (var ex in exemplaires)
                {
                    var etat = etats.Find(e => e.Id == ex.IdEtat);
                    ex.LibelleEtat = etat != null ? etat.Libelle : "Inconnu";
                }

                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.Columns["photo"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }

            
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocument = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = "00001";
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);

                    if (controller.AjouterExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("Le numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("Le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("Le numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv_commandesLivres.CurrentCell != null && bdgCommandesLivresListe.Count > 0)
            {
                int index = bdgCommandesLivresListe.Position;
                if (index >= 0 && index < bdgCommandesLivresListe.Count) // ✅ Vérification avant d'accéder
                {
                    CommandeEntiere commande = (CommandeEntiere)bdgCommandesLivresListe.List[index];
                    Livre livre = controller.GetLivre(commande.IdLivreDvd);

                    AfficheCommandesLivresInfos(livre, commande);
                    VideCommandesLivresZones();
                    VerifierSuiviCommandeLivres();
                }
            }
            else
            {
                VideCommandesLivresInfos();
            }
        }
        #endregion

        /// <summary>
        /// Actions sur le chargement de la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMediatek_Load(object sender, EventArgs e)
        {
            controller.GetAllDictionnaries();

        }

        /// <summary>
        /// Bouton qui ouvre la fenêtre d'ajout d'un Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ajouterLivre_Click(object sender, EventArgs e)
        {
            FrmAjouterLivre.ShowDialog();
        }

        /// <summary>
        /// Bouton qui ouvre la fenêtre de modification d'un Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_modifierLivre_Click(object sender, EventArgs e)
        {
            Livre livre = new Livre(
            txbLivresNumero.Text,
            txbLivresTitre.Text,
            txbLivresImage.Text,
            txbLivresIsbn.Text,
            txbLivresAuteur.Text,
            txbLivresCollection.Text,

            // Récupérer l'ID du genre en fonction du libellé
            controller.GetIdByNameOfGenre(txbLivresGenre.Text),
            txbLivresGenre.Text,  // libellé du genre

            // Récupérer l'ID du public en fonction du libellé
            controller.GetIdByNameOfPublic(txbLivresPublic.Text),
            txbLivresPublic.Text,  // libellé du public

            // Récupérer l'ID du rayon en fonction du libellé
            controller.GetIdByNameOfRayon(txbLivresRayon.Text),
            txbLivresRayon.Text   // libellé du rayon
            );

            FrmModifierLivre = new FrmModifierLivre(livre, this);
            FrmModifierLivre.ShowDialog();
        }

        /// <summary>
        /// Bouton qui permet la suppression d'un Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_supprimerLivre_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbLivresNumero.Text))
            {
                MessageBox.Show("Veuillez sélectionner un livre à supprimer.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Vérifier qu'un document n'est pas rataché à un exemplaire avant de passer à la suite
            if (controller.GetExemplaireById(txbLivresNumero.Text) == null)
            {

                // Demander une confirmation avant de supprimer
                DialogResult result = MessageBox.Show(
                "Voulez-vous vraiment supprimer ce livre ? Cette action est irréversible.",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

                Livre livre = new Livre(
                txbLivresNumero.Text,
                txbLivresTitre.Text,
                txbLivresImage.Text,
                txbLivresIsbn.Text,
                txbLivresAuteur.Text,
                txbLivresCollection.Text,

                // Récupérer l'ID du genre en fonction du libellé
                controller.GetIdByNameOfGenre(txbLivresGenre.Text),
                txbLivresGenre.Text,  // libellé du genre

                // Récupérer l'ID du public en fonction du libellé
                controller.GetIdByNameOfPublic(txbLivresPublic.Text),
                txbLivresPublic.Text,  // libellé du public

                // Récupérer l'ID du rayon en fonction du libellé
                controller.GetIdByNameOfRayon(txbLivresRayon.Text),
                txbLivresRayon.Text   // libellé du rayon
                );

                Document document = new Document(
                txbLivresNumero.Text,
                txbLivresTitre.Text,
                txbLivresImage.Text,
                // Récupérer l'ID du genre en fonction du libellé
                controller.GetIdByNameOfGenre(txbLivresGenre.Text),
                txbLivresGenre.Text,  // libellé du genre

                // Récupérer l'ID du public en fonction du libellé
                controller.GetIdByNameOfPublic(txbLivresPublic.Text),
                txbLivresPublic.Text,  // libellé du public

                // Récupérer l'ID du rayon en fonction du libellé
                controller.GetIdByNameOfRayon(txbLivresRayon.Text),
                txbLivresRayon.Text   // libellé du rayon
                    );

                if (result == DialogResult.Yes)
                {
                    bool success = controller.SupprimerLivre(livre, document); // Appel de la méthode de suppression

                    if (success)
                    {
                        Console.WriteLine("Suppression réussie.");
                        MessageBox.Show("Le livre a bien été supprimé.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Console.WriteLine("Erreur lors de la suppression.");
                        MessageBox.Show("Erreur lors de la suppression du livre.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Ce livre ne peut pas être supprimé car il est rattaché à un exemplaire.");
            }

            lesLivres = controller.GetAllLivres();
            RemplirLivresListeComplete();

        }

        /// <summary>
        /// Bouton qui ouvre la fenêtre d'ajout d'une Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ajouterRevue_Click(object sender, EventArgs e)
        {
            FrmAjouterRevue.ShowDialog();
        }

        /// <summary>
        /// Bouton qui ouvre la fenêtre d'ajout d'un Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ajouterDVD_Click(object sender, EventArgs e)
        {
            FrmAjouterDvD.ShowDialog();
        }

        /// <summary>
        /// Bouton qui ouvre la fenêtre de modification d'un Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_modifierDVD_Click(object sender, EventArgs e)
        {
            Dvd dvd = new Dvd(
            txbDvdNumero.Text,
            txbDvdTitre.Text,
            txbDvdImage.Text,
            int.Parse(txbDvdDuree.Text),
            txbDvdRealisateur.Text,
            txbDvdSynopsis.Text,

            // Récupérer l'ID du genre en fonction du libellé
            controller.GetIdByNameOfGenre(txbLivresGenre.Text),
            txbLivresGenre.Text,  // libellé du genre

            // Récupérer l'ID du public en fonction du libellé
            controller.GetIdByNameOfPublic(txbLivresPublic.Text),
            txbLivresPublic.Text,  // libellé du public

            // Récupérer l'ID du rayon en fonction du libellé
            controller.GetIdByNameOfRayon(txbLivresRayon.Text),
            txbLivresRayon.Text   // libellé du rayon
            );

            FrmModifierDvD = new FrmModifierDvD(dvd, this);
            FrmModifierDvD.ShowDialog();
        }

        /// <summary>
        /// Bouton qui ouvre la fenêtre de modification d'une Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_modifierRevue_Click(object sender, EventArgs e)
        {
            Revue revue = new Revue(
                txbRevuesNumero.Text,
                txbRevuesTitre.Text,
                txbRevuesImage.Text,
                // Récupérer l'ID du genre en fonction du libellé
                controller.GetIdByNameOfGenre(txbLivresGenre.Text),
                txbRevuesGenre.Text,  // libellé du genre

                // Récupérer l'ID du public en fonction du libellé
                controller.GetIdByNameOfPublic(txbLivresPublic.Text),
                txbRevuesPublic.Text,  // libellé du public

                // Récupérer l'ID du rayon en fonction du libellé
                controller.GetIdByNameOfRayon(txbLivresRayon.Text),
                txbRevuesRayon.Text,   // libellé du rayon
                txbRevuesPeriodicite.Text,
                int.Parse(txbRevuesDateMiseADispo.Text)
                );

            FrmModifierRevue = new FrmModifierRevue(revue, this);
            FrmModifierRevue.ShowDialog();
        }

        /// <summary>
        /// Bouton qui permet la suppression d'une Revue 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_supprimerRevue_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbRevuesNumero.Text))
            {
                MessageBox.Show("Veuillez sélectionner un livre à supprimer.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Vérifier qu'un document n'est pas rataché à un exemplaire avant de passer à la suite
            if (controller.GetExemplaireById(txbRevuesNumero.Text) == null)
            {

                // Demander une confirmation avant de supprimer
                DialogResult result = MessageBox.Show(
                "Voulez-vous vraiment supprimer cette revue ? Cette action est irréversible.",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

                Revue revue = new Revue(
                    txbRevuesNumero.Text,
                    txbRevuesTitre.Text,
                    txbRevuesImage.Text,
                    // Récupérer l'ID du genre en fonction du libellé
                    controller.GetIdByNameOfGenre(txbLivresGenre.Text),
                    txbRevuesGenre.Text,  // libellé du genre

                    // Récupérer l'ID du public en fonction du libellé
                    controller.GetIdByNameOfPublic(txbLivresPublic.Text),
                    txbRevuesPublic.Text,  // libellé du public

                    // Récupérer l'ID du rayon en fonction du libellé
                    controller.GetIdByNameOfRayon(txbLivresRayon.Text),
                    txbRevuesRayon.Text,   // libellé du rayon
                    txbRevuesPeriodicite.Text,
                    int.Parse(txbRevuesDateMiseADispo.Text)
                    );

                Document document = new Document(
                txbRevuesNumero.Text,
                txbRevuesTitre.Text,
                txbRevuesImage.Text,
                // Récupérer l'ID du genre en fonction du libellé
                controller.GetIdByNameOfGenre(txbRevuesGenre.Text),
                txbRevuesGenre.Text,  // libellé du genre

                // Récupérer l'ID du public en fonction du libellé
                controller.GetIdByNameOfPublic(txbRevuesPublic.Text),
                txbRevuesPublic.Text,  // libellé du public

                // Récupérer l'ID du rayon en fonction du libellé
                controller.GetIdByNameOfRayon(txbRevuesRayon.Text),
                txbRevuesRayon.Text   // libellé du rayon
                    );

                if (result == DialogResult.Yes)
                {
                    bool success = controller.SupprimerRevue(revue, document); // Appel de la méthode de suppression

                    if (success)
                    {
                        Console.WriteLine("✅ Suppression réussie.");
                        MessageBox.Show("Le livre a bien été supprimé.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Console.WriteLine("❌ Erreur lors de la suppression.");
                        MessageBox.Show("Erreur lors de la suppression du livre.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Cette revue ne peut pas être supprimé car elle est rattachée à un exemplaire.");
            }

            lesRevues = controller.GetAllRevues();
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Bouton qui permet la suppression d'un Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_supprimerDVD_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbDvdNumero.Text))
            {
                MessageBox.Show("Veuillez sélectionner un livre à supprimer.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Vérifier qu'un document n'est pas rataché à un exemplaire avant de passer à la suite
            if (controller.GetExemplaireById(txbDvdNumero.Text) == null)
            {
                // Demander une confirmation avant de supprimer
                DialogResult result = MessageBox.Show(
                    "Voulez-vous vraiment supprimer ce dvd ? Cette action est irréversible.",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                Dvd dvd = new Dvd(
                txbDvdNumero.Text,
                txbDvdTitre.Text,
                txbDvdImage.Text,
                int.Parse(txbDvdDuree.Text),
                txbDvdRealisateur.Text,
                txbDvdSynopsis.Text,


                // Récupérer l'ID du genre en fonction du libellé
                controller.GetIdByNameOfGenre(txbDvdGenre.Text),
                txbDvdGenre.Text,  // libellé du genre

                // Récupérer l'ID du public en fonction du libellé
                controller.GetIdByNameOfPublic(txbDvdPublic.Text),
                txbDvdPublic.Text,  // libellé du public

                // Récupérer l'ID du rayon en fonction du libellé
                controller.GetIdByNameOfRayon(txbDvdRayon.Text),
                txbDvdRayon.Text   // libellé du rayon
                );

                Document document = new Document(
                txbDvdNumero.Text,
                txbDvdTitre.Text,
                txbDvdImage.Text,
                // Récupérer l'ID du genre en fonction du libellé
                controller.GetIdByNameOfGenre(txbDvdGenre.Text),
                txbDvdGenre.Text,  // libellé du genre

                // Récupérer l'ID du public en fonction du libellé
                controller.GetIdByNameOfPublic(txbDvdPublic.Text),
                txbDvdPublic.Text,  // libellé du public

                // Récupérer l'ID du rayon en fonction du libellé
                controller.GetIdByNameOfRayon(txbDvdRayon.Text),
                txbDvdRayon.Text   // libellé du rayon
                    );

                if (result == DialogResult.Yes)
                {
                    bool success = controller.SupprimerDvd(dvd, document); // Appel de la méthode de suppression

                    if (success)
                    {
                        Console.WriteLine("✅ Suppression réussie.");
                        MessageBox.Show("Le dvd a bien été supprimé.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Console.WriteLine("❌ Erreur lors de la suppression.");
                        MessageBox.Show("Erreur lors de la suppression du dvd.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Ce dvd ne peut pas être supprimé car il est rattaché à un exemplaire.");
            }

            lesDvd = controller.GetAllDvd();
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Actions à l'entrée de la page CommandesLivres 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesLivres_Enter(object sender, EventArgs e)
        {
            Console.WriteLine("🔎 tabCommandesLivres_Enter est bien exécuté !");
            lesCommandesDocument = controller.GetAllCommandesDocumentLivres();
            Console.WriteLine($"📌 Nombre de commandes livres récupérées : {lesCommandesDocument.Count}");

            RemplirCommandesLivresListeComplete();

            VerifierSuiviCommandeLivres();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        public void RemplirCommandesLivresListeComplete()
        {
            RemplirCommandesDocumentLivresListe(lesCommandesDocument);
            VideCommandesLivresZones();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        public void RemplirCommandesRevueListeComplete()
        {
            RemplirCommandesDocumentRevueListe(lesAbonnements);
            VideCommandesRevueZones();
        }

        /// <summary>
        /// Affichage de la liste complète des dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        public void RemplirCommandesDvdListeComplete()
        {
            RemplirCommandesDocumentDvdListe(lesCommandesDocument);
            VideCommandesDvdZones();
        }

        /// <summary>
        /// Actions sur le changement de sélection du dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_commandesLivres_SelectionChanged(object sender, EventArgs e)
        {
            // Vérifie si le DataGridView a des lignes et si une cellule est bien sélectionnée
            if (dgv_commandesLivres.CurrentCell != null && bdgCommandesLivresListe.Position >= 0 && bdgCommandesLivresListe.Position < bdgCommandesLivresListe.Count)
            {
                CommandeEntiere commande = (CommandeEntiere)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];

                // Vérifie si la commande existe avant d'appeler GetLivre
                if (commande != null)
                {
                    Livre livre = controller.GetLivre(commande.IdLivreDvd);
                    AfficheCommandesLivresInfos(livre, commande);
                }

                VerifierSuiviCommandeLivres();
            }
            else
            {
                VideCommandesLivresInfos();
                VerifierSuiviCommandeLivres();
            }
        }

        public List<CommandeEntiere> lesCommandesEntieres = new List<CommandeEntiere>();

        private string colonneTriée = "";
        private bool ordreTriAscendant = true;

        /// <summary>
        /// Actions sur les en têtes du dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_commandesLivres_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideCommandesLivresZones();

            string colonne = dgv_commandesLivres.Columns[e.ColumnIndex].DataPropertyName;

            if (!string.IsNullOrEmpty(colonne) && lesCommandesEntieres.Count > 0)
            {
                // Vérifier si on clique sur la même colonne pour inverser le tri
                bool ascendant = true;
                if (colonneTriée == colonne)
                {
                    ordreTriAscendant = !ordreTriAscendant;
                    ascendant = ordreTriAscendant;
                }
                else
                {
                    colonneTriée = colonne;
                    ordreTriAscendant = true;
                }

                // Trier la liste fusionnée
                List<CommandeEntiere> sortedList = ascendant
                    ? lesCommandesEntieres.OrderBy(x => x.GetType().GetProperty(colonne).GetValue(x, null)).ToList()
                    : lesCommandesEntieres.OrderByDescending(x => x.GetType().GetProperty(colonne).GetValue(x, null)).ToList();

                // Mettre à jour l'affichage
                bdgCommandesLivresListe.DataSource = sortedList;
                dgv_commandesLivres.DataSource = bdgCommandesLivresListe;
            }
        }

        /// <summary>
        /// Actions sur le bouton de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbCLivresNumRechercheDoc.Text))
            {
                // Si la recherche est vide, on recharge toute la liste
                RemplirCommandesLivresListeComplete();
                return;
            }

            // Récupérer la liste actuelle affichée dans le DataGridView
            List<CommandeEntiere> listeActuelle = bdgCommandesLivresListe.DataSource as List<CommandeEntiere>;

            if (listeActuelle == null || listeActuelle.Count == 0)
            {
                MessageBox.Show("Aucune commande disponible.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Recherche de la commande par numéro de document
            CommandeEntiere commandeTrouvee = listeActuelle
                .Find(x => x.IdLivreDvd.ToString().Equals(txbCLivresNumRechercheDoc.Text));

            if (commandeTrouvee != null)
            {
                // Créer une liste contenant uniquement la commande trouvée
                List<CommandeEntiere> commandesFiltrees = new List<CommandeEntiere>() { commandeTrouvee };

                // Mettre à jour l'affichage avec la liste filtrée
                bdgCommandesLivresListe.DataSource = commandesFiltrees;
                dgv_commandesLivres.DataSource = bdgCommandesLivresListe;

                // Sélectionner la première ligne
                if (dgv_commandesLivres.Rows.Count > 0)
                {
                    dgv_commandesLivres.ClearSelection();
                    dgv_commandesLivres.Rows[0].Selected = true;
                    dgv_commandesLivres.CurrentCell = dgv_commandesLivres.Rows[0].Cells[0];
                }

                // Mettre à jour les informations du livre sélectionné
                dgv_commandesLivres_SelectionChanged(null, null);
            }
            else
            {
                MessageBox.Show("Numéro introuvable", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RemplirCommandesLivresListeComplete();
            }
        }

        /// <summary>
        /// Actions sur le bouton de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbCLivresNumRechercheCom.Text))
            {
                // Si la recherche est vide, on recharge toute la liste
                RemplirCommandesLivresListeComplete();
                return;
            }

            // Récupérer la liste actuelle affichée dans le DataGridView
            List<CommandeEntiere> listeActuelle = bdgCommandesLivresListe.DataSource as List<CommandeEntiere>;

            if (listeActuelle == null || listeActuelle.Count == 0)
            {
                MessageBox.Show("Aucune commande disponible.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Recherche de la commande par numéro de document
            CommandeEntiere commandeTrouvee = listeActuelle
                .Find(x => x.IdLivreDvd.ToString().Equals(txbCLivresNumRechercheCom.Text));

            if (commandeTrouvee != null)
            {
                // Créer une liste contenant uniquement la commande trouvée
                List<CommandeEntiere> commandesFiltrees = new List<CommandeEntiere>() { commandeTrouvee };

                // Mettre à jour l'affichage avec la liste filtrée
                bdgCommandesLivresListe.DataSource = commandesFiltrees;
                dgv_commandesLivres.DataSource = bdgCommandesLivresListe;

                // Sélectionner la première ligne
                if (dgv_commandesLivres.Rows.Count > 0)
                {
                    dgv_commandesLivres.ClearSelection();
                    dgv_commandesLivres.Rows[0].Selected = true;
                    dgv_commandesLivres.CurrentCell = dgv_commandesLivres.Rows[0].Cells[0];
                }

                // Mettre à jour les informations du livre sélectionné
                dgv_commandesLivres_SelectionChanged(null, null);
            }
            else
            {
                MessageBox.Show("Numéro introuvable", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RemplirCommandesLivresListeComplete();
            }
        }

        /// <summary>
        /// Actions sur le bouton de réception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Réception_Click(object sender, EventArgs e)
        {
            if (txbCLivresStade.Text != "livrée")
            {
                if (txbCLivresNumeroCom.Text != null)
                {
                    string photo = "";
                    string Etat = "00001";
                    string stadeLivré = "livrée";
                    if (!int.TryParse(txbNombre.Text, out int unites) || unites < 0)
                    {
                        MessageBox.Show("Veuillez entrer un nombre valide d'exemplaires.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Exemplaire exemplaire = new Exemplaire(
                        0,
                        DateTime.Now,
                        null,
                        Etat,
                        txbCLivresNumeroDoc.Text
                         );
                    for (int i = 0; i < int.Parse(txbNombre.Text); i++)
                    {
                        controller.AjouterExemplaire(exemplaire);
                    }
                    Suivi suivi = new Suivi(
                            txbCLivresNumeroCom.Text,
                            stadeLivré,
                            txbCLivresNumeroDoc.Text
                            );
                    // Appelle l'API pour ajouter le livre
                    bool succes2 = controller.ModifierSuivi(suivi);

                    if (succes2)
                    {
                        MessageBox.Show("Suivi modifié avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RemplirCommandesLivresListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de l'ajout du suivi.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez saisir un numéro d'exemplaire.");
                }
            }
            else
            {
                MessageBox.Show("La commande a déjà été réceptionnée.");
            }

            ViderDgvCommandesLivres();
            lesCommandesDocument = controller.GetAllCommandesDocumentLivres();
            RemplirCommandesLivresListeComplete();
            VerifierSuiviCommandeLivres();
        }

        /// <summary>
        /// Méthode qui gère la visibilité des boutons selon leur libellé de suivi
        /// </summary>
        private void VerifierSuiviCommandeLivres()
        {
            if (txbCLivresStade.Text == "livrée")
            {
                btn_Réception.Enabled = false;
                btn_Réception.Visible = false;
                btn_supprimerComLivres.Enabled = false;

                btn_relance.Enabled = false;
                btn_relance.Visible = false;

                btn_réglée.Enabled = true;
                btn_réglée.Visible = true;
            }
            if (txbCLivresStade.Text == "en cours")
            {
                btn_Réception.Enabled = true;
                btn_Réception.Visible = true;
                btn_supprimerComLivres.Enabled = true;

                btn_relance.Enabled = true;
                btn_relance.Visible = true;

                btn_réglée.Enabled = false;
                btn_réglée.Visible = false;
            }
            if (txbCLivresStade.Text == "relancée")
            {
                btn_Réception.Enabled = true;
                btn_Réception.Visible = true;
                btn_supprimerComLivres.Enabled = true;

                btn_relance.Enabled = false;
                btn_relance.Visible = false;

                btn_réglée.Enabled = false;
                btn_réglée.Visible = false;
            }
            if (txbCLivresStade.Text == "réglée")
            {
                btn_Réception.Enabled = false;
                btn_Réception.Visible = false;
                btn_supprimerComLivres.Enabled = false;

                btn_relance.Enabled = false;
                btn_relance.Visible = false;

                btn_réglée.Enabled = false;
                btn_réglée.Visible = false;
            }
        }

        /// <summary>
        /// Méthode qui gère la visibilité des boutons selon leur libellé de suivi
        /// </summary>
        private void VerifierSuiviCommandeDvd()
        {
            if (txbCDvdStade.Text == "livrée")
            {
                btn_ReceptionnéeDvd.Enabled = false;
                btn_ReceptionnéeDvd.Visible = false;
                btn_supprimerComDvd.Enabled = false;

                btn_relanceDvd.Enabled = false;
                btn_relanceDvd.Visible = false;

                btn_régléeDvd.Enabled = true;
                btn_régléeDvd.Visible = true;
            }
            if (txbCDvdStade.Text == "en cours")
            {
                btn_ReceptionnéeDvd.Enabled = true;
                btn_ReceptionnéeDvd.Visible = true;
                btn_supprimerComDvd.Enabled = true;

                btn_relanceDvd.Enabled = true;
                btn_relanceDvd.Visible = true;

                btn_régléeDvd.Enabled = false;
                btn_régléeDvd.Visible = false;
            }
            if (txbCDvdStade.Text == "relancée")
            {
                btn_ReceptionnéeDvd.Enabled = true;
                btn_ReceptionnéeDvd.Visible = true;
                btn_supprimerComDvd.Enabled = true;

                btn_relanceDvd.Enabled = false;
                btn_relanceDvd.Visible = false;

                btn_régléeDvd.Enabled = false;
                btn_régléeDvd.Visible = false;
            }
            if (txbCDvdStade.Text == "réglée")
            {
                btn_ReceptionnéeDvd.Enabled = false;
                btn_ReceptionnéeDvd.Visible = false;
                btn_supprimerComDvd.Enabled = false;

                btn_relanceDvd.Enabled = false;
                btn_relanceDvd.Visible = false;

                btn_régléeDvd.Enabled = false;
                btn_régléeDvd.Visible = false;
            }
        }

        /// <summary>
        /// Actions sur le bouton d'ajout d'une commande de Livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AjouterComLivres_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            string stade = "en cours";
            Commande commande = new Commande(
                txbCLivresNumCommande.Text,
                date,
                double.Parse(txbCLivresMontantCom.Text)
            );
            CommandeDocument commandeDocument = new CommandeDocument(
                txbCLivresNumCommande.Text,
                (int)nuUnite.Value,
                txbCLivresNumCom.Text);
            Suivi suivi = new Suivi(
                txbCLivresNumCommande.Text,
                stade,
                txbCLivresNumCom.Text
                );
            controller.AjouterCommande(commande);
            controller.AjouterCommandeDocument(commandeDocument);
            controller.AjouterSuivi(suivi);

            ViderDgvCommandesLivres();
            lesCommandesDocument = controller.GetAllCommandesDocumentLivres();
            RemplirCommandesLivresListeComplete();
            VerifierSuiviCommandeLivres();
        }

        /// <summary>
        /// Actions sur le bouton de suppression d'une commande de Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_supprimerComLivres_Click(object sender, EventArgs e)
        {
            double montant = 1;
            if (string.IsNullOrEmpty(txbCLivresNumeroDoc.Text) && string.IsNullOrEmpty(txbCLivresNumeroCom.Text))
            {
                MessageBox.Show("Veuillez sélectionner une commande à supprimer.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txbCLivresStade.Text == "livrée")
            {
                MessageBox.Show("La commande ne peut pas être supprimée. Elle a été livrée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Commande commande = new Commande(
                txbCLivresNumeroCom.Text,
                DateTime.Parse(txbDate.Text),
                montant
            );

            CommandeDocument commandeDocument = new CommandeDocument(
                txbCLivresNumeroCom.Text,
                (int)nuUnite.Value,
                txbCLivresNumCom.Text);

            Suivi suivi = new Suivi(
                txbCLivresNumeroCom.Text,
                txbCLivresStade.Text,
                txbCLivresNumCom.Text
                );

            Console.WriteLine("La commande = " + commande.Id + commandeDocument.Id + suivi.Id);

            // Demander une confirmation avant de supprimer
            DialogResult result = MessageBox.Show(
                "Voulez-vous vraiment supprimer cette commande ? Cette action est irréversible.",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                bool success = controller.SupprimerCommande(commande, commandeDocument, suivi); // Appel de la méthode de suppression

                if (success)
                {
                    Console.WriteLine("✅ Suppression réussie.");
                    MessageBox.Show("La commande a bien été supprimée.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // ⚠️ Mettre à jour le DGV après suppression
                    RemplirCommandesLivresListeComplete();

                    // Sélectionner la première ligne si possible
                    if (dgv_commandesLivres.Rows.Count > 0)
                    {
                        dgv_commandesLivres.ClearSelection();
                        dgv_commandesLivres.Rows[0].Selected = true;
                        dgv_commandesLivres.CurrentCell = dgv_commandesLivres.Rows[0].Cells[0];
                    }

                    // ⚠️ Mettre à jour les informations du livre sélectionné
                    dgv_commandesLivres_SelectionChanged(null, null);
                }
                else
                {
                    Console.WriteLine("❌ Erreur lors de la suppression.");
                    MessageBox.Show("Erreur lors de la suppression de la commande.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            ViderDgvCommandesLivres();

            lesCommandesDocument = controller.GetAllCommandesDocumentLivres();
            RemplirCommandesLivresListeComplete();
            VerifierSuiviCommandeLivres();

        }

        /// <summary>
        /// Méthode qui vide le dgv
        /// </summary>
        private void ViderDgvCommandesLivres()
        {
            dgv_commandesLivres.DataSource = null;
            dgv_commandesLivres.Rows.Clear();
            dgv_commandesLivres.Columns.Clear();
        }

        /// <summary>
        /// Méthode qui vide le dgv
        /// </summary>
        private void ViderDgvCommandesDvd()
        {
            dgv_commandesDvd.DataSource = null;
            dgv_commandesDvd.Rows.Clear();
            dgv_commandesDvd.Columns.Clear();
        }

        /// <summary>
        /// Méthode qui vide le dgv
        /// </summary>
        private void ViderDgvCommandesRevue()
        {
            dgv_commandesRevue.DataSource = null;
            dgv_commandesRevue.Rows.Clear();
            dgv_commandesRevue.Columns.Clear();
        }

        /// <summary>
        /// Bouton qui permet de changer le libelle de suivi en Réglée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_réglée_Click(object sender, EventArgs e)
        {
            if (txbCLivresStade.Text == "livrée")
            {
                if (txbCLivresNumeroCom.Text != null)
                {
                    string stadeRéglée = "réglée";
                    if (!int.TryParse(txbNombre.Text, out int unites) || unites < 0)
                    {
                        MessageBox.Show("Veuillez entrer un nombre valide d'exemplaires.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Suivi suivi = new Suivi(
                            txbCLivresNumeroCom.Text,
                            stadeRéglée,
                            txbCLivresNumeroDoc.Text
                            );

                    Console.WriteLine("suivi =" + suivi);
                    // Appelle l'API pour ajouter le livre
                    bool succes2 = controller.ModifierSuivi(suivi);

                    if (succes2)
                    {
                        MessageBox.Show("Suivi modifié avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RemplirCommandesLivresListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de l'ajout du suivi.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez saisir un numéro d'exemplaire.");
                }
            }

            ViderDgvCommandesLivres();
            RemplirCommandesLivresListeComplete();
            VerifierSuiviCommandeLivres();
        }

        /// <summary>
        /// Bouton qui permet de changer le libelle de suivi en Relancée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_relance_Click(object sender, EventArgs e)
        {
            if (txbCLivresStade.Text == "en cours")
            {
                if (txbCLivresNumeroCom.Text != null)
                {
                    string stadeRéglée = "relancée";
                    if (!int.TryParse(txbNombre.Text, out int unites) || unites < 0)
                    {
                        MessageBox.Show("Veuillez entrer un nombre valide d'exemplaires.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Suivi suivi = new Suivi(
                            txbCLivresNumeroCom.Text,
                            stadeRéglée,
                            txbCLivresNumeroDoc.Text
                            );

                    Console.WriteLine("suivi =" + suivi);
                    // Appelle l'API pour ajouter le livre
                    bool succes2 = controller.ModifierSuivi(suivi);

                    if (succes2)
                    {
                        MessageBox.Show("Suivi modifié avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RemplirCommandesLivresListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de l'ajout du suivi.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez saisir un numéro d'exemplaire.");
                }
            }

            ViderDgvCommandesLivres();
            RemplirCommandesLivresListeComplete();
            VerifierSuiviCommandeLivres();
        }

        /// <summary>
        /// Actions sur la sélection des en têtes du dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_commandesDvd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideCommandesDvdZones();

            string colonne = dgv_commandesDvd.Columns[e.ColumnIndex].DataPropertyName;

            if (!string.IsNullOrEmpty(colonne) && lesCommandesEntieres.Count > 0)
            {
                // Vérifier si on clique sur la même colonne pour inverser le tri
                bool ascendant = true;
                if (colonneTriée == colonne)
                {
                    ordreTriAscendant = !ordreTriAscendant;
                    ascendant = ordreTriAscendant;
                }
                else
                {
                    colonneTriée = colonne;
                    ordreTriAscendant = true;
                }

                // Trier la liste fusionnée
                List<CommandeEntiere> sortedList = ascendant
                    ? lesCommandesEntieres.OrderBy(x => x.GetType().GetProperty(colonne).GetValue(x, null)).ToList()
                    : lesCommandesEntieres.OrderByDescending(x => x.GetType().GetProperty(colonne).GetValue(x, null)).ToList();

                // Mettre à jour l'affichage
                bdgCommandesDvdListe.DataSource = sortedList;
                dgv_commandesDvd.DataSource = bdgCommandesDvdListe;
            }
        }

        /// <summary>
        /// Actions sur le changement de sélection du dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_commandesDvd_SelectionChanged(object sender, EventArgs e)
        {
            // Vérifie si le DataGridView a des lignes et si une cellule est bien sélectionnée
            if (dgv_commandesDvd.CurrentCell != null && bdgCommandesDvdListe.Position >= 0 && bdgCommandesDvdListe.Position < bdgCommandesDvdListe.Count)
            {
                CommandeEntiere commande = (CommandeEntiere)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];

                // Vérifie si la commande existe avant d'appeler GetLivre
                if (commande != null)
                {
                    Dvd dvd = controller.GetDvd(commande.IdLivreDvd);
                    AfficheCommandesDvdInfos(dvd, commande);
                }

                VerifierSuiviCommandeDvd();
            }
            else
            {
                VideCommandesDvdInfos();
                VerifierSuiviCommandeDvd();
            }
        }

        /// <summary>
        /// Actions à l'entrée de la page de CommandesDvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesDvd_Enter(object sender, EventArgs e)
        {
            lesCommandesDocument = controller.GetAllCommandesDocumentDvd();
            RemplirCommandesDvdListeComplete();

            VerifierSuiviCommandeDvd();
        }

        /// <summary>
        /// Bouton qui permet la suppression d'une commande de Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_supprimerComDvd_Click(object sender, EventArgs e)
        {
            double montant = 1;
            if (string.IsNullOrEmpty(txbCDvdNumeroDoc.Text) && string.IsNullOrEmpty(txbCDvdNumeroCom.Text))
            {
                MessageBox.Show("Veuillez sélectionner une commande à supprimer.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txbCDvdStade.Text == "livrée")
            {
                MessageBox.Show("La commande ne peut pas être supprimée. Elle a été livrée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Commande commande = new Commande(
                txbCDvdNumeroCom.Text,
                DateTime.Parse(txbCDvdDate.Text),
                montant
            );

            CommandeDocument commandeDocument = new CommandeDocument(
                txbCDvdNumeroCom.Text,
                (int)nuDvdUnite.Value,
                txbCDvdNumCom.Text);

            Suivi suivi = new Suivi(
                txbCDvdNumeroCom.Text,
                txbCDvdStade.Text,
                txbCDvdNumCom.Text
                );

            Console.WriteLine("La commande = " + commande.Id + commandeDocument.Id + suivi.Id);

            // Demander une confirmation avant de supprimer
            DialogResult result = MessageBox.Show(
                "Voulez-vous vraiment supprimer cette commande ? Cette action est irréversible.",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                bool success = controller.SupprimerCommande(commande, commandeDocument, suivi); // Appel de la méthode de suppression

                if (success)
                {
                    Console.WriteLine("✅ Suppression réussie.");
                    MessageBox.Show("La commande a bien été supprimée.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // ⚠️ Mettre à jour le DGV après suppression
                    RemplirCommandesDvdListeComplete();

                    // Sélectionner la première ligne si possible
                    if (dgv_commandesDvd.Rows.Count > 0)
                    {
                        dgv_commandesDvd.ClearSelection();
                        dgv_commandesDvd.Rows[0].Selected = true;
                        dgv_commandesDvd.CurrentCell = dgv_commandesDvd.Rows[0].Cells[0];
                    }

                    // ⚠️ Mettre à jour les informations du livre sélectionné
                    dgv_commandesDvd_SelectionChanged(null, null);
                }
                else
                {
                    Console.WriteLine("❌ Erreur lors de la suppression.");
                    MessageBox.Show("Erreur lors de la suppression de la commande.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            ViderDgvCommandesDvd();

            lesCommandesDocument = controller.GetAllCommandesDocumentDvd();
            RemplirCommandesDvdListeComplete();
            VerifierSuiviCommandeDvd();
        }

        /// <summary>
        /// Bouton qui permet l'ajout d'une commande de Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AjouterComDvd_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            string stade = "en cours";

            Commande commande = new Commande(
                txbCDvdNumCommande.Text,
                date,
                double.Parse(txbCDvdMontantCom.Text)
            );
            CommandeDocument commandeDocument = new CommandeDocument(
                txbCDvdNumCommande.Text,
                (int)nuDvdUnite.Value,
                txbCDvdNumCom.Text);
            Suivi suivi = new Suivi(
                txbCDvdNumCommande.Text,
                stade,
                txbCDvdNumCom.Text
                );
            controller.AjouterCommande(commande);
            controller.AjouterCommandeDocument(commandeDocument);
            controller.AjouterSuivi(suivi);

            ViderDgvCommandesDvd();
            lesCommandesDocument = controller.GetAllCommandesDocumentDvd();
            RemplirCommandesDvdListeComplete();
            VerifierSuiviCommandeDvd();
        }

        /// <summary>
        /// Bouton qui permet une recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbCDvdNumRechercheDoc.Text))
            {
                // Si la recherche est vide, on recharge toute la liste
                RemplirCommandesDvdListeComplete();
                return;
            }

            // Récupérer la liste actuelle affichée dans le DataGridView
            List<CommandeEntiere> listeActuelle = bdgCommandesDvdListe.DataSource as List<CommandeEntiere>;

            if (listeActuelle == null || listeActuelle.Count == 0)
            {
                MessageBox.Show("Aucune commande disponible.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Recherche de la commande par numéro de document
            CommandeEntiere commandeTrouvee = listeActuelle
                .Find(x => x.IdLivreDvd.ToString().Equals(txbCDvdNumRechercheDoc.Text));

            if (commandeTrouvee != null)
            {
                // Créer une liste contenant uniquement la commande trouvée
                List<CommandeEntiere> commandesFiltrees = new List<CommandeEntiere>() { commandeTrouvee };

                // Mettre à jour l'affichage avec la liste filtrée
                bdgCommandesDvdListe.DataSource = commandesFiltrees;
                dgv_commandesDvd.DataSource = bdgCommandesDvdListe;

                // Sélectionner la première ligne
                if (dgv_commandesDvd.Rows.Count > 0)
                {
                    dgv_commandesDvd.ClearSelection();
                    dgv_commandesDvd.Rows[0].Selected = true;
                    dgv_commandesDvd.CurrentCell = dgv_commandesDvd.Rows[0].Cells[0];
                }

                // ⚠️ Mettre à jour les informations du livre sélectionné
                dgv_commandesDvd_SelectionChanged(null, null);
            }
            else
            {
                MessageBox.Show("Numéro introuvable", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RemplirCommandesDvdListeComplete();
            }
        }

        /// <summary>
        /// Bouton qui permet le changement du libellé de suivi en Relancée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_relanceDvd_Click(object sender, EventArgs e)
        {
            if (txbCDvdStade.Text == "en cours")
            {
                if (txbCDvdNumeroCom.Text != null)
                {
                    string stadeRéglée = "relancée";
                    if (!int.TryParse(txbCDvdNombre.Text, out int unites) || unites < 0)
                    {
                        MessageBox.Show("Veuillez entrer un nombre valide d'exemplaires.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Suivi suivi = new Suivi(
                            txbCDvdNumeroCom.Text,
                            stadeRéglée,
                            txbCDvdNumeroDoc.Text
                            );

                    Console.WriteLine("suivi =" + suivi);
                    // Appelle l'API pour ajouter le livre
                    bool succes2 = controller.ModifierSuivi(suivi);

                    if (succes2)
                    {
                        MessageBox.Show("Suivi modifié avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RemplirCommandesDvdListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de l'ajout du suivi.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez saisir un numéro d'exemplaire.");
                }
            }

            ViderDgvCommandesDvd();
            RemplirCommandesDvdListeComplete();
            VerifierSuiviCommandeDvd();
        }

        /// <summary>
        /// Bouton qui permet le changement du libellé de suivi en Réglée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_régléeDvd_Click(object sender, EventArgs e)
        {
            if (txbCDvdStade.Text == "livrée")
            {
                if (txbCDvdNumeroCom.Text != null)
                {
                    string stadeRéglée = "réglée";
                    if (!int.TryParse(txbCDvdNombre.Text, out int unites) || unites < 0)
                    {
                        MessageBox.Show("Veuillez entrer un nombre valide d'exemplaires.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Suivi suivi = new Suivi(
                            txbCDvdNumeroCom.Text,
                            stadeRéglée,
                            txbCDvdNumeroDoc.Text
                            );

                    Console.WriteLine("suivi =" + suivi);
                    // Appelle l'API pour ajouter le livre
                    bool succes2 = controller.ModifierSuivi(suivi);

                    if (succes2)
                    {
                        MessageBox.Show("Suivi modifié avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RemplirCommandesDvdListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de l'ajout du suivi.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez saisir un numéro d'exemplaire.");
                }
            }

            ViderDgvCommandesDvd();
            RemplirCommandesDvdListeComplete();
            VerifierSuiviCommandeDvd();
        }

        /// <summary>
        /// Bouton qui permet le changement du libellé de suivi en Livrée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ReceptionnéeDvd_Click(object sender, EventArgs e)
        {
            if (txbCDvdStade.Text != "livrée")
            {
                if (txbCDvdNumeroCom.Text != null)
                {
                    string photo = "";
                    string Etat = "00001";
                    string stadeLivré = "livrée";
                    if (!int.TryParse(txbCDvdNombre.Text, out int unites) || unites < 0)
                    {
                        MessageBox.Show("Veuillez entrer un nombre valide d'exemplaires.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Exemplaire exemplaire = new Exemplaire(
                        0,
                        DateTime.Now,
                        null,
                        Etat,
                        txbCDvdNumeroDoc.Text
                         );
                    for (int i = 0; i < int.Parse(txbCDvdNombre.Text); i++)
                    {
                        controller.AjouterExemplaire(exemplaire);
                    }

                    Suivi suivi = new Suivi(
                            txbCDvdNumeroCom.Text,
                            stadeLivré,
                            txbCDvdNumeroDoc.Text
                            );
                    // Appelle l'API pour ajouter le livre
                    bool succes2 = controller.ModifierSuivi(suivi);

                    if (succes2)
                    {
                        MessageBox.Show("Suivi modifié avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RemplirCommandesDvdListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de l'ajout du suivi.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez saisir un numéro d'exemplaire.");
                }
            }
            else
            {
                MessageBox.Show("La commande a déjà été réceptionnée.");
            }

            ViderDgvCommandesDvd();
            lesCommandesDocument = controller.GetAllCommandesDocumentDvd();
            RemplirCommandesDvdListeComplete();
            VerifierSuiviCommandeDvd();
        }

        /// <summary>
        /// Bouton de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbCDvdNumRechercheCom.Text))
            {
                // Si la recherche est vide, on recharge toute la liste
                RemplirCommandesDvdListeComplete();
                return;
            }

            // Récupérer la liste actuelle affichée dans le DataGridView
            List<CommandeEntiere> listeActuelle = bdgCommandesDvdListe.DataSource as List<CommandeEntiere>;

            if (listeActuelle == null || listeActuelle.Count == 0)
            {
                MessageBox.Show("Aucune commande disponible.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Recherche de la commande par numéro de document
            CommandeEntiere commandeTrouvee = listeActuelle
                .Find(x => x.Id.ToString().Equals(txbCDvdNumRechercheCom.Text));

            if (commandeTrouvee != null)
            {
                // Créer une liste contenant uniquement la commande trouvée
                List<CommandeEntiere> commandesFiltrees = new List<CommandeEntiere>() { commandeTrouvee };

                // Mettre à jour l'affichage avec la liste filtrée
                bdgCommandesDvdListe.DataSource = commandesFiltrees;
                dgv_commandesDvd.DataSource = bdgCommandesDvdListe;

                // Sélectionner la première ligne
                if (dgv_commandesDvd.Rows.Count > 0)
                {
                    dgv_commandesDvd.ClearSelection();
                    dgv_commandesDvd.Rows[0].Selected = true;
                    dgv_commandesDvd.CurrentCell = dgv_commandesDvd.Rows[0].Cells[0];
                }

                // ⚠️ Mettre à jour les informations du livre sélectionné
                dgv_commandesDvd_SelectionChanged(null, null);
            }
            else
            {
                MessageBox.Show("Numéro introuvable", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RemplirCommandesDvdListeComplete();
            }
        }

        /// <summary>
        /// Bouton de suppression d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            Commande commande = new Commande(
                txbCRevueNumeroCom.Text,
                DateTime.Parse(txbCRevueDate.Text),
                1
            );
            Abonnement abonnement = new Abonnement(
                txbCRevueNumeroCom.Text,
                DateTime.Parse(txbCRevueDateFin.Text),
                txbCRevueNumeroDoc.Text,
                commande
                );
            Exemplaire exemplaire = controller.GetExemplaireById(txbCRevueNumeroDoc.Text);

            Console.WriteLine(exemplaire.DateAchat);
            

            if(controller.ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, exemplaire.DateAchat) == true)
            {
                // Demander une confirmation avant de supprimer
                DialogResult result = MessageBox.Show(
                    "Voulez-vous vraiment supprimer cette commande ? Cette action est irréversible.",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    bool success = controller.SupprimerAbonnement(commande, abonnement); // Appel de la méthode de suppression

                    if (success)
                    {
                        Console.WriteLine("✅ Suppression réussie.");
                        MessageBox.Show("La commande a bien été supprimée.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // ⚠️ Mettre à jour le DGV après suppression
                        RemplirCommandesRevueListeComplete();

                        // Sélectionner la première ligne si possible
                        if (dgv_commandesRevue.Rows.Count > 0)
                        {
                            dgv_commandesRevue.ClearSelection();
                            dgv_commandesRevue.Rows[0].Selected = true;
                            dgv_commandesRevue.CurrentCell = dgv_commandesRevue.Rows[0].Cells[0];
                        }

                        // ⚠️ Mettre à jour les informations du livre sélectionné
                        dgv_commandesRevue_SelectionChanged(null, null);
                    }
                    else
                    {
                        Console.WriteLine("❌ Erreur lors de la suppression.");
                        MessageBox.Show("Erreur lors de la suppression de la commande.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                ViderDgvCommandesRevue();

                lesAbonnements = controller.GetAllAbonnements();
                RemplirCommandesRevueListeComplete();
            }
            else
            {
                MessageBox.Show("La revue n'a pas expirée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }   
        }

        /// <summary>
        /// Actions sur le changement de sélection du dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_commandesRevue_SelectionChanged(object sender, EventArgs e)
        {
            // Vérifie si le DataGridView a des lignes et si une cellule est bien sélectionnée
            if (dgv_commandesRevue.CurrentCell != null && bdgCommandesRevueListe.Position >= 0 && bdgCommandesRevueListe.Position < bdgCommandesRevueListe.Count)
            {
                Abonnement abonnement = (Abonnement)bdgCommandesRevueListe.List[bdgCommandesRevueListe.Position];

                // Vérifie si la commande existe avant d'appeler GetLivre
                if (abonnement != null)
                {
                    Revue revue = controller.GetRevue(abonnement.IdRevue);
                    AfficheCommandesRevueInfos(revue, abonnement);
                }
            }
            else
            {
                VideCommandesRevueInfos();
            }
        }

        /// <summary>
        /// Bouton qui modifie un Abonnement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            DateTime dateFin = dateTimePicker1.Value;
            string idEtat = "00001";

            Commande commande = new Commande(
                txbCRevueNumCommande.Text,
                date,
                double.Parse(txbCRevueMontant.Text)
            );
            Abonnement abonnement = new Abonnement(
                commande.Id,
                dateFin,
                txbCRevueNumCom.Text,
                commande
                );
            Exemplaire exemplaire = new Exemplaire(
                0,
                date,
                null,
                idEtat,
                txbCRevueNumCom.Text

                );

            // Vérification si l'abonnement existe déjà
            bool verification = controller.CheckAbonnementByIdRevue(abonnement.IdRevue);

            Console.WriteLine("Vérification est " + verification);

            // Si oui, renouveler
            if (verification == true)
            {
                bool success = controller.UpdateAbonnement(abonnement);
                bool success2 = controller.UpdateMontantCommande(commande);

                if (success && success2)
                {
                    Console.WriteLine("Abonnement mis à jour.");
                }
                else
                {
                    Console.WriteLine("Echec de la mise à jour de l'abonnement");
                }
            }
            // Si non, créer un nouvel abonnement
            else
            {
                controller.AjouterCommande(commande);
                controller.AjouterAbonnement(abonnement);
                controller.AjouterExemplaire(exemplaire);
            }

            // Mettre à jour le dgv
            ViderDgvCommandesRevue();
            lesAbonnements = controller.GetAllAbonnements();
            RemplirCommandesRevueListeComplete();
        }

        /// <summary>
        /// Actions à l'entrée de la page de CommandesRevues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesRevues_Enter(object sender, EventArgs e)
        {
            lesAbonnements = controller.GetAllAbonnements();
            RemplirCommandesRevueListeComplete();
        }

        /// <summary>
        /// Bouton de recherche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbCRevueNumRechercheDoc.Text))
            {
                // Si la recherche est vide, on recharge toute la liste
                RemplirCommandesRevueListeComplete();
                return;
            }

            // Récupérer la liste actuelle affichée dans le DataGridView
            List<Abonnement> listeActuelle = bdgCommandesRevueListe.DataSource as List<Abonnement>;

            if (listeActuelle == null || listeActuelle.Count == 0)
            {
                MessageBox.Show("Aucune commande disponible.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Recherche de la commande par numéro de document
            Abonnement abonnementTrouve = listeActuelle
                .Find(x => x.IdRevue.ToString().Equals(txbCRevueNumRechercheDoc.Text));

            if (abonnementTrouve != null)
            {
                // Créer une liste contenant uniquement la commande trouvée
                List<Abonnement> commandesFiltrees = new List<Abonnement>() { abonnementTrouve };

                // Mettre à jour l'affichage avec la liste filtrée
                bdgCommandesRevueListe.DataSource = commandesFiltrees;
                dgv_commandesRevue.DataSource = bdgCommandesRevueListe;

                // Sélectionner la première ligne
                if (dgv_commandesRevue.Rows.Count > 0)
                {
                    dgv_commandesRevue.ClearSelection();
                    dgv_commandesRevue.Rows[0].Selected = true;
                    dgv_commandesRevue.CurrentCell = dgv_commandesRevue.Rows[0].Cells[0];
                }

                // Mettre à jour les informations du livre sélectionné
                dgv_commandesRevue_SelectionChanged(null, null);
            }
            else
            {
                MessageBox.Show("Numéro introuvable", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RemplirCommandesRevueListeComplete();
            }
        }

        /// <summary>
        /// Actions sur le bouton de modification d'un état
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvLivresExemplaires.CurrentRow != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgLivresExemplairesListe.List[bdgLivresExemplairesListe.Position];
                Etat etat = new Etat(
                    ((Etat)cbxLivresEtats.SelectedItem).Id,
                    "test"
                    );

                if (cbxLivresEtats.SelectedItem != null)
                {

                    bool succes = controller.ModifierEtatExemplaire(exemplaire, etat);

                    if (succes)
                    {
                        MessageBox.Show("État modifié avec succès !");
                        RemplirLivresExemplaires(); // Refresh de la liste
                    }
                    else
                    {
                        MessageBox.Show("Échec de la modification.");
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un état.");
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire.");
            }
        }

        /// <summary>
        /// Actions sur le bouton de suppression
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            // Demander une confirmation avant de supprimer
            DialogResult result = MessageBox.Show(
                "Voulez-vous vraiment supprimer cet exemplaire ? Cette action est irréversible.",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {

                if (dgvLivresExemplaires.CurrentRow != null)
                {
                    Exemplaire exemplaire = (Exemplaire)bdgLivresExemplairesListe.List[bdgLivresExemplairesListe.Position];

                    bool succes = controller.SupprimerExemplaire(exemplaire);

                    if (succes)
                    {
                        MessageBox.Show("Suppression avec succès !");
                        RemplirLivresExemplaires(); // Refresh de la liste
                    }
                    else
                    {
                        MessageBox.Show("Échec de la suppression.");
                    }

                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un exemplaire.");
                }
            }
        }

        /// <summary>
        /// Actions sur le bouton de modification d'un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            if (dgvDvdExemplaires.CurrentRow != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgDvdExemplairesListe.List[bdgDvdExemplairesListe.Position];
                Etat etat = new Etat(
                    ((Etat)cbxDvdEtats.SelectedItem).Id,
                    "test"
                    );

                if (cbxDvdEtats.SelectedItem != null)
                {

                    bool succes = controller.ModifierEtatExemplaire(exemplaire, etat);

                    if (succes)
                    {
                        MessageBox.Show("État modifié avec succès !");
                        RemplirDvdExemplaires(); // Refresh de la liste
                    }
                    else
                    {
                        MessageBox.Show("Échec de la modification.");
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un état.");
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire.");
            }
        }

        /// <summary>
        /// Actions sur le bouton de suppression d'un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            // Demander une confirmation avant de supprimer
            DialogResult result = MessageBox.Show(
                "Voulez-vous vraiment supprimer cet exemplaire ? Cette action est irréversible.",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {

                if (dgvDvdExemplaires.CurrentRow != null)
                {
                    Exemplaire exemplaire = (Exemplaire)bdgDvdExemplairesListe.List[bdgDvdExemplairesListe.Position];

                    bool succes = controller.SupprimerExemplaire(exemplaire);

                    if (succes)
                    {
                        MessageBox.Show("Suppression avec succès !");
                        RemplirDvdExemplaires(); // Refresh de la liste
                    }
                    else
                    {
                        MessageBox.Show("Échec de la suppression.");
                    }

                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un exemplaire.");
                }
            }
        }

        /// <summary>
        /// Actions sur le bouton de modification d'un état
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentRow != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                Etat etat = new Etat(
                    ((Etat)cbxParutionsEtats.SelectedItem).Id,
                    "test"
                    );

                if (cbxParutionsEtats.SelectedItem != null)
                {

                    bool succes = controller.ModifierEtatExemplaire(exemplaire, etat);

                    if (succes)
                    {
                        MessageBox.Show("État modifié avec succès !");
                        AfficheReceptionExemplairesRevue(); // Refresh de la liste
                    }
                    else
                    {
                        MessageBox.Show("Échec de la modification.");
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un état.");
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un exemplaire.");
            }
        }

        /// <summary>
        /// Actions sur le bouton de suppression d'un exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            // Demander une confirmation avant de supprimer
            DialogResult result = MessageBox.Show(
                "Voulez-vous vraiment supprimer cet exemplaire ? Cette action est irréversible.",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {

                if (dgvReceptionExemplairesListe.CurrentRow != null)
                {
                    Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];

                    bool succes = controller.SupprimerExemplaire(exemplaire);

                    if (succes)
                    {
                        MessageBox.Show("Suppression avec succès !");
                        AfficheReceptionExemplairesRevue(); // Refresh de la liste
                    }
                    else
                    {
                        MessageBox.Show("Échec de la suppression.");
                    }

                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un exemplaire.");
                }
            }
        }

        /// <summary>
        /// Permet d'ignore un exception récurrente du dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is IndexOutOfRangeException)
            {
                e.ThrowException = false; // Ignore l'exception
            }
        }

        /// <summary>
        /// Permet d'ignore un exception récurrente du dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresListe_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is IndexOutOfRangeException)
            {
                e.ThrowException = false; // Ignore l'exception
            }
        }

        /// <summary>
        /// Permet d'ignore un exception récurrente du dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is IndexOutOfRangeException)
            {
                e.ThrowException = false; // Ignore l'exception
            }
        }
    }
}
