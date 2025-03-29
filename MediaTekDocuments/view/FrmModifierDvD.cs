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
    /// Formulaire de modification des Dvd
    /// </summary>
    public partial class FrmModifierDvD : Form
    {
        // Déclaration des variables
        private Dvd dvd;
        private BindingSource bdgGenres = new BindingSource();
        private BindingSource bdgPublics = new BindingSource();
        private BindingSource bdgRayons = new BindingSource();
        private FrmMediatekController controller = new FrmMediatekController();
        private FrmMediatek frmMediatek;

        /// <summary>
        /// Initialisation de la fenêtre
        /// </summary>
        /// <param name="dvd"></param>
        /// <param name="frmMediatek"></param>
        public FrmModifierDvD(Dvd dvd, FrmMediatek frmMediatek)
        {
            InitializeComponent();
            this.frmMediatek = frmMediatek;
            this.dvd = dvd;
            txbId.Text = dvd.Id;
            txbDuree.Text = dvd.Duree.ToString();
            txbRealisateur.Text = dvd.Realisateur;
            txbDvdTitre.Text = dvd.Titre;
            txbSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
        }

        /// <summary>
        /// Sélectionne automatiquement l'élément dans la ComboBox si la valeur correspond
        /// </summary>
        private void SetSelectedComboBox(ComboBox cbx, string value)
        {
            if (cbx.Items.Count > 0)
            {
                foreach (var item in cbx.Items)
                {
                    // Vérifier si l’élément est un objet Categorie et comparer son Libelle
                    if (item is Categorie cat && cat.Libelle.Trim() == value.Trim())
                    {
                        cbx.SelectedItem = item;
                        Console.WriteLine($"✅ {cbx.Name} sélectionne : {cat.Libelle}");
                        return;
                    }
                }

                Console.WriteLine($"⚠️ Valeur '{value}' introuvable dans {cbx.Name}");
            }
            else
            {
                Console.WriteLine($"⚠️ La ComboBox {cbx.Name} est vide !");
            }
        }

        /// <summary>
        /// Actions au chargement de la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmModifierDvD_Load(object sender, EventArgs e)
        {
            try
            {
                // Charger et afficher l'image dans le PictureBox
                pcbLivresImage.Image = new Bitmap(txbDvdImage.Text);
                pcbLivresImage.SizeMode = PictureBoxSizeMode.Zoom; // Ajuster l'image pour qu'elle tienne bien dans le PictureBox
            }
            catch (Exception ex)
            {
                if (pcbLivresImage.Image == null)
                {
                    Console.Write("Le chemin de l'image est null");
                }
                else
                {
                    MessageBox.Show("Erreur lors du chargement de l'image : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }

            var genres = controller.GetAllGenres();
            var publics = controller.GetAllPublics();
            var rayons = controller.GetAllRayons();

            Console.WriteLine($"📌 Genres récupérés : {genres.Count}");
            Console.WriteLine($"📌 Publics récupérés : {publics.Count}");
            Console.WriteLine($"📌 Rayons récupérés : {rayons.Count}");

            // Remplir les ComboBox seulement si elles contiennent des données
            if (genres.Count > 0) RemplirCombo(genres, bdgGenres, cb_genre);
            if (publics.Count > 0) RemplirCombo(publics, bdgPublics, cb_public);
            if (rayons.Count > 0) RemplirCombo(rayons, bdgRayons, cb_rayon);

            // Maintenant que les ComboBox sont remplies, on peut sélectionner les valeurs
            SetSelectedComboBox(cb_genre, dvd.Genre);
            SetSelectedComboBox(cb_public, dvd.Public);
            SetSelectedComboBox(cb_rayon, dvd.Rayon);
        }

        /// <summary>
        /// Actions sur le bouton Parcourir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    txbDvdImage.Text = openFileDialog.FileName;

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

        /// <summary>
        /// Méthode qui remplit les combobox
        /// </summary>
        /// <param name="lesCategories"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
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
        /// Méthode qui permet la modification d'un dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_modifierDvD_Click(object sender, EventArgs e)
        {

            // Vérifie que tous les champs sont remplis
            if (string.IsNullOrWhiteSpace(txbDvdTitre.Text) ||
                cb_genre.SelectedItem == null ||
                cb_public.SelectedItem == null ||
                cb_rayon.SelectedItem == null ||
                txbId == null)
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Dvd dvd = new Dvd(
                txbId.Text,
                txbDvdTitre.Text,
                txbDvdImage.Text,
                int.Parse(txbDuree.Text),
                txbRealisateur.Text,
                txbSynopsis.Text,
                controller.GetIdByNameOfGenre(cb_genre.Text), cb_genre.Text,
                controller.GetIdByNameOfPublic(cb_public.Text), cb_public.Text,
                controller.GetIdByNameOfRayon(cb_rayon.Text), cb_rayon.Text

            );

            Document documentModifie = new Document(
                txbId.Text,
                txbDvdTitre.Text,
                txbDvdImage.Text,
                controller.GetIdByNameOfGenre(cb_genre.Text), cb_genre.Text,
                controller.GetIdByNameOfPublic(cb_public.Text), cb_public.Text,
                controller.GetIdByNameOfRayon(cb_rayon.Text), cb_rayon.Text
            );

            // Appelle l'API pour ajouter le livre
            bool succes = controller.ModifierDvd(dvd, documentModifie);

            if (succes)
            {
                MessageBox.Show("Livre ajouté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout du livre.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            frmMediatek.lesDvd = controller.GetAllDvd();
            frmMediatek.RemplirDvdListeComplete();

        }
    }
}
