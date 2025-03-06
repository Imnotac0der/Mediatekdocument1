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
using Newtonsoft.Json;
using MediaTekDocuments.dal;

namespace MediaTekDocuments.view
{
    public partial class FrmAjouterLivre : Form
    {
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private FrmMediatekController controller = new FrmMediatekController();
        private FrmMediatek frmMediatek;
        public FrmAjouterLivre(FrmMediatek frmMediatek)
        {
            InitializeComponent();
            this.frmMediatek = frmMediatek;
        }

        private void label59_Click(object sender, EventArgs e)
        {

        }

        private void pcbLivresImage_Click(object sender, EventArgs e)
        {

        }

        private void FrmAjouterLivre_Load(object sender, EventArgs e)
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
        private void AjouterLivre()
        {
            // Vérifie que tous les champs sont remplis
            if (string.IsNullOrWhiteSpace(txbLivresTitre.Text) ||
                txbLivresIsbn.Text == null ||
                string.IsNullOrWhiteSpace(txbLivresAuteur.Text) ||
                string.IsNullOrWhiteSpace(txbLivresCollection.Text) ||
                cb_genre.SelectedItem == null ||
                cb_public.SelectedItem == null ||
                cb_rayon.SelectedItem == null ||
                txbId == null)
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Livre livre = new Livre(
                txbId.Text,
                txbLivresTitre.Text,
                txbLivresImage.Text,
                txbLivresIsbn.Text,
                txbLivresAuteur.Text,
                txbLivresCollection.Text,
                controller.GetIdByNameOfGenre(cb_genre.Text), cb_genre.Text,
                controller.GetIdByNameOfPublic(cb_public.Text), cb_public.Text,
                controller.GetIdByNameOfRayon(cb_rayon.Text), cb_rayon.Text
            );



            // Appelle l'API pour ajouter le livre
            bool succes = controller.AjouterLivre(livre);

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


        private void btn_ajouterLivre_Click(object sender, EventArgs e)
        {         
            AjouterLivre();
            frmMediatek.lesLivres = controller.GetAllLivres();
            frmMediatek.RemplirLivresListeComplete();



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
                    txbLivresImage.Text = openFileDialog.FileName;

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
