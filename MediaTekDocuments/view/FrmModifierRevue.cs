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
    public partial class FrmModifierRevue : Form
    {
        private Revue revue;
        private BindingSource bdgGenres = new BindingSource();
        private BindingSource bdgPublics = new BindingSource();
        private BindingSource bdgRayons = new BindingSource();
        private FrmMediatekController controller = new FrmMediatekController();
        private FrmMediatek frmMediatek;
        public FrmModifierRevue(Revue revue, FrmMediatek frmMediatek)
        {
            InitializeComponent();
            this.frmMediatek = frmMediatek;
            this.revue = revue;
            txbId.Text = revue.Id.ToString();
            txbPeriodicite.Text = revue.Periodicite;
            txbRevueTitre.Text = revue.Titre;
            txbDispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevueImage.Text = revue.Image;
        }

        private void FrmModifierRevue_Load(object sender, EventArgs e)
        {
            try
            {
                // Charger et afficher l'image dans le PictureBox
                pcbLivresImage.Image = new Bitmap(txbRevueImage.Text);
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
            SetSelectedComboBox(cb_genre, revue.Genre);
            SetSelectedComboBox(cb_public, revue.Public);
            SetSelectedComboBox(cb_rayon, revue.Rayon);
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

        private void btn_modifierRevue_Click(object sender, EventArgs e)
        {
            ModifierRevue();

            frmMediatek.lesRevues = controller.GetAllRevues();
            frmMediatek.RemplirRevuesListeComplete();
        }

        private void ModifierRevue()
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

            Document documentModifie = new Document(
                txbId.Text,
                txbRevueTitre.Text,
                txbRevueImage.Text,
                controller.GetIdByNameOfGenre(cb_genre.Text), cb_genre.Text,
                controller.GetIdByNameOfPublic(cb_public.Text), cb_public.Text,
                controller.GetIdByNameOfRayon(cb_rayon.Text), cb_rayon.Text
            );

            // Appelle l'API pour ajouter le livre
            bool succes = controller.ModifierRevue(revueModifie, documentModifie);

            if (succes)
            {
                MessageBox.Show("Livre ajouté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Erreur lors de l'ajout du livre.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            frmMediatek.RemplirRevuesListeComplete();
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
