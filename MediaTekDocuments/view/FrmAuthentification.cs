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
    public partial class FrmAuthentification : Form
    {
        private FrmMediatekController controller;
        private FrmAbonnements frmAbonnements;
        private FrmMediatek frmMediatek;
        public FrmAuthentification()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(txbLogin.Text != "" || txbPwd.Text != "")
                {
                    if (controller.IsConnected(txbLogin.Text, txbPwd.Text))
                    {
                        // Stocker l'utilisateur
                        Utilisateur utilisateurConnecte = controller.GetUtilisateur(txbLogin.Text);

                        if(utilisateurConnecte.IdService == "00001" || utilisateurConnecte.IdService == "00002")
                        {
                            if(utilisateurConnecte.IdService == "00001")
                            {
                                this.Hide();
                                FrmAbonnements frmAbonnements = new FrmAbonnements(utilisateurConnecte);
                                frmAbonnements.ShowDialog();
                                this.Close();
                            }
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

        private void FrmAuthentification_Load(object sender, EventArgs e)
        {

        }
    }
}
