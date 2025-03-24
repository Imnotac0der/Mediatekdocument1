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
    public partial class FrmAbonnements : Form
    {
        private FrmMediatek FrmMediatek;
        private readonly BindingSource bdgAbonnements = new BindingSource();
        public List<Abonnement> lesAbonnements = new List<Abonnement>();
        private FrmMediatekController controller;
        public FrmAbonnements(FrmMediatek frmMediatek)
        {
            InitializeComponent();
            this.FrmMediatek = frmMediatek;
            this.controller = new FrmMediatekController();
        }

        private void FrmAbonnements_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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

        private void RemplirAbonnementsListeComplete()
        {
            RemplirAbonnementsListe(lesAbonnements);
        }

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

        private void dgvAbonnementsListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAbonnementsListe.CurrentCell != null)
            {
                try
                {
                    Abonnement abonnement = (Abonnement)bdgAbonnements.List[bdgAbonnements.Position];
                }
                catch
                {
                    //rien
                }
            }
            else
            {
                //rien
            }
        }
    }
}
