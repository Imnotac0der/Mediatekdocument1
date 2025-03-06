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
    public partial class FrmAjouterRevue : Form
    {
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private FrmMediatekController controller = new FrmMediatekController();
        private FrmMediatek frmMediatek;
        public FrmAjouterRevue(FrmMediatek frmMediatek)
        {
            InitializeComponent();
            this.frmMediatek = frmMediatek;
        }

        private void btn_ajouterRevue_Click(object sender, EventArgs e)
        {
            AjouterRevue();
            frmMediatek.lesRevues = controller.GetAllRevues();
            frmMediatek.RemplirRevuesListeComplete();
        }

        private void FrmAjouterRevue_Load(object sender, EventArgs e)
        {
            //remplir le cb_genre de tous les genres
            RemplirCombo(controller.GetAllGenres(), bdgGenres, cb_genre);

            //remplir le cb_public de tous les différents publics
            RemplirCombo(controller.GetAllPublics(), bdgPublics, cb_public);

            //remplir le cb_rayon de tous les différents rayons
            RemplirCombo(controller.GetAllRayons(), bdgRayons, cb_rayon);
        }
        public void RemplirCombo(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        /// <summary>
        /// Récupère les informations du formulaire et tente d'ajouter un livre
        /// </summary>
        private void AjouterRevue()
        {
            // Vérifie que tous les champs sont remplis
            if (string.IsNullOrWhiteSpace(txbRevueTitre.Text) ||
                txbId.Text == null ||
                string.IsNullOrWhiteSpace(txbPeriodicite.Text) ||
                string.IsNullOrWhiteSpace(txbDispo.Text) ||
                cb_genre.SelectedItem == null ||
                cb_public.SelectedItem == null ||
                cb_rayon.SelectedItem == null)
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Revue revueModifie = new Revue(
                txbId.Text,
                txbRevueTitre.Text,
                txbRevueImage.Text,
                controller.GetIdByNameOfGenre(cb_genre.Text), cb_genre.Text,
                controller.GetIdByNameOfPublic(cb_public.Text), cb_public.Text,
                controller.GetIdByNameOfRayon(cb_rayon.Text), cb_rayon.Text,
                txbPeriodicite.Text,
                int.Parse(txbDispo.Text)
            );



            // Appelle l'API pour ajouter le livre
            bool succes = controller.AjouterRevue(revueModifie);

            if (succes)
            {
                MessageBox.Show("Livre ajouté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout du livre.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_parcourir_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Configurer le filtre pour afficher uniquement les fichiers image
                openFileDialog.Filter = "Images (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Tous les fichiers (*.*)|*.*";
                openFileDialog.Title = "Sélectionner une image";

                // Ouvrir la boîte de dialogue et vérifier si l'utilisateur a sélectionné un fichier
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Mettre à jour le champ texte avec le chemin du fichier sélectionné
                    txbRevueImage.Text = openFileDialog.FileName;

                    try
                    {
                        // Charger et afficher l'image dans le PictureBox
                        pcbLivresImage.Image = new Bitmap(openFileDialog.FileName);
                        pcbLivresImage.SizeMode = PictureBoxSizeMode.Zoom; // Ajuster l'image pour qu'elle tienne bien dans le PictureBox
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erreur lors du chargement de l'image : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
