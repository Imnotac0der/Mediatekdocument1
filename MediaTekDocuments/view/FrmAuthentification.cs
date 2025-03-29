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
    /// Formulaire d'Authentification
    /// </summary>
    public partial class FrmAuthentification : Form
    {
        // Création de la variable controller
        private FrmMediatekController controller;

        /// <summary>
        /// Initialisation de la fenêtre
        /// </summary>
        public FrmAuthentification()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
        }

        /// <summary>
        /// Bouton qui permet la connexion de l'utilisateur ou non
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Vérifier si les champs ne sont pas vides
                if(txbLogin.Text != "" || txbPwd.Text != "")
                {
                    // Appeler la méthode IsConnected pour vérifier que la saisi est correct
                    if (controller.IsConnected(txbLogin.Text, txbPwd.Text))
                    {
                        // Stocker l'utilisateur
                        Utilisateur utilisateurConnecte = controller.GetUtilisateur(txbLogin.Text);

                        if(utilisateurConnecte == null)
                        {
                            lblError.Text = "Identifiants incorrects.";
                            lblError.Visible = true;
                        }

                        // Vérifier le bon id de service
                        else if(utilisateurConnecte.IdService == "00001" || utilisateurConnecte.IdService == "00002")
                        {
                            // Si 00001, rediriger vers la fenêtre Abonnements
                            if(utilisateurConnecte.IdService == "00001")
                            {
                                this.Hide();
                                FrmAbonnements frmAbonnements = new FrmAbonnements(utilisateurConnecte);
                                frmAbonnements.ShowDialog();
                                this.Close();
                            }

                            // Sinon, rediriger directement vers la fenêtre principale
                            else
                            {
                                this.Hide();
                                FrmMediatek frmMediatek = new FrmMediatek(utilisateurConnecte);
                                frmMediatek.ShowDialog();
                                this.Close();
                            }
                        }
                        else
                        {
                            lblError.Text = "Votre service n'a pas le droit d'accéder aux fonctionnalités de l'application.";
                            lblError.Visible = true;
                        }
                    }
                    else
                    {
                        lblError.Text = "Identifiants incorrects.";
                        lblError.Visible = true;
                    }
                }
                else
                {
                    lblError.Text = "Veuillez saisir correctement tous les champs.";
                    lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
