using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;

namespace MediaTekDocuments.view
{
    /// <summary>
    /// Formulaire d'affichage et de gestion des abonnements.
    /// </summary>
    public partial class FrmAbonnements : Form
    {
        // Création des variables
        private readonly BindingSource bdgAbonnements = new BindingSource();
        public List<Abonnement> lesAbonnements = new List<Abonnement>();
        private FrmMediatekController controller;
        private Utilisateur utilisateur;

        /// <summary>
        /// Initialisation de la fenêtre
        /// </summary>
        /// <param name="utilisateur"></param>
        public FrmAbonnements(Utilisateur utilisateur)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this.utilisateur = utilisateur;
        }

        /// <summary>
        /// Bouton qui permet la fermeture de la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            FrmMediatek frmMediatek = new FrmMediatek(utilisateur);
            frmMediatek.ShowDialog();
            this.Close();
        }

        /// <summary>
        /// Actions à l'entrée du dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresListe_Enter(object sender, EventArgs e)
        {
            try
            {
                lesAbonnements = controller.GetAllAbonnements30days();

                if (lesAbonnements == null || lesAbonnements.Count == 0)
                {
                    Console.WriteLine("Aucun abonnement trouvé, fermeture de la fenêtre.");
                    this.Close();
                }
                else
                {
                    Console.WriteLine($"{lesAbonnements.Count} abonnement(s) trouvé(s) :");
                    foreach (Abonnement abonnement in lesAbonnements)
                    {
                        Console.WriteLine($"→ ID: {abonnement.Id}, Titre: {abonnement.IdRevue}, Début: {abonnement.DateCommande:yyyy-MM-dd}, Fin: {abonnement.DateFinAbonnement:yyyy-MM-dd}");
                    }

                    RemplirAbonnementsListeComplete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur dans dgvLivresListe_Enter : {ex.Message}");
                this.Close();
            }
        }

        /// <summary>
        /// Méthode qui appelle RemplirAbonnementsListe
        /// </summary>
        private void RemplirAbonnementsListeComplete()
        {
            RemplirAbonnementsListe(lesAbonnements);
        }

        /// <summary>
        /// Méthode qui remplit le dgv
        /// </summary>
        /// <param name="abonnements"></param>
        private void RemplirAbonnementsListe(List<Abonnement> abonnements)
        {
            bdgAbonnements.DataSource = abonnements;
            dgvAbonnementsListe.DataSource = bdgAbonnements;
            dgvAbonnementsListe.Columns["idCommande"].Visible = false;
            dgvAbonnementsListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvAbonnementsListe.Columns["id"].DisplayIndex = 0;
            dgvAbonnementsListe.Columns["idRevue"].DisplayIndex = 1;
            dgvAbonnementsListe.Columns["dateCommande"].DisplayIndex = 2;

            // Renommer les colonnes
            dgvAbonnementsListe.Columns["id"].HeaderText = "Numéro de commande";
            dgvAbonnementsListe.Columns["DateCommande"].HeaderText = "Date de commande";
            dgvAbonnementsListe.Columns["Montant"].HeaderText = "Montant (€)";
            dgvAbonnementsListe.Columns["idRevue"].HeaderText = "Numéro de document";
            dgvAbonnementsListe.Columns["dateFinAbonnement"].HeaderText = "Date d'expiration";
        }

        /// <summary>
        /// Actions sur le changement de sélection dans le dgv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAbonnementsListe.CurrentCell != null)
            {
                try
                {
                    Abonnement abonnement = (Abonnement)bdgAbonnements.List[bdgAbonnements.Position];
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Erreur." + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                //rien
            }
        }
    }
}
