
namespace MediaTekDocuments.view
{
    partial class FrmAbonnements
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpAbonnements = new System.Windows.Forms.GroupBox();
            this.btnLivresAnnulRayons = new System.Windows.Forms.Button();
            this.btnlivresAnnulPublics = new System.Windows.Forms.Button();
            this.btnLivresAnnulGenres = new System.Windows.Forms.Button();
            this.dgvAbonnementsListe = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.grpAbonnements.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAbonnementsListe)).BeginInit();
            this.SuspendLayout();
            // 
            // grpAbonnements
            // 
            this.grpAbonnements.Controls.Add(this.button1);
            this.grpAbonnements.Controls.Add(this.label1);
            this.grpAbonnements.Controls.Add(this.btnLivresAnnulRayons);
            this.grpAbonnements.Controls.Add(this.btnlivresAnnulPublics);
            this.grpAbonnements.Controls.Add(this.btnLivresAnnulGenres);
            this.grpAbonnements.Controls.Add(this.dgvAbonnementsListe);
            this.grpAbonnements.Location = new System.Drawing.Point(6, 8);
            this.grpAbonnements.Name = "grpAbonnements";
            this.grpAbonnements.Size = new System.Drawing.Size(626, 259);
            this.grpAbonnements.TabIndex = 19;
            this.grpAbonnements.TabStop = false;
            this.grpAbonnements.Text = "Expiration des abonnements";
            // 
            // btnLivresAnnulRayons
            // 
            this.btnLivresAnnulRayons.Location = new System.Drawing.Point(833, 104);
            this.btnLivresAnnulRayons.Name = "btnLivresAnnulRayons";
            this.btnLivresAnnulRayons.Size = new System.Drawing.Size(22, 22);
            this.btnLivresAnnulRayons.TabIndex = 16;
            this.btnLivresAnnulRayons.Text = "X";
            this.btnLivresAnnulRayons.UseVisualStyleBackColor = true;
            // 
            // btnlivresAnnulPublics
            // 
            this.btnlivresAnnulPublics.Location = new System.Drawing.Point(833, 60);
            this.btnlivresAnnulPublics.Name = "btnlivresAnnulPublics";
            this.btnlivresAnnulPublics.Size = new System.Drawing.Size(22, 22);
            this.btnlivresAnnulPublics.TabIndex = 15;
            this.btnlivresAnnulPublics.Text = "X";
            this.btnlivresAnnulPublics.UseVisualStyleBackColor = true;
            // 
            // btnLivresAnnulGenres
            // 
            this.btnLivresAnnulGenres.Location = new System.Drawing.Point(833, 17);
            this.btnLivresAnnulGenres.Name = "btnLivresAnnulGenres";
            this.btnLivresAnnulGenres.Size = new System.Drawing.Size(22, 22);
            this.btnLivresAnnulGenres.TabIndex = 11;
            this.btnLivresAnnulGenres.Text = "X";
            this.btnLivresAnnulGenres.UseVisualStyleBackColor = true;
            // 
            // dgvAbonnementsListe
            // 
            this.dgvAbonnementsListe.AllowUserToAddRows = false;
            this.dgvAbonnementsListe.AllowUserToDeleteRows = false;
            this.dgvAbonnementsListe.AllowUserToResizeColumns = false;
            this.dgvAbonnementsListe.AllowUserToResizeRows = false;
            this.dgvAbonnementsListe.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAbonnementsListe.Location = new System.Drawing.Point(6, 19);
            this.dgvAbonnementsListe.MultiSelect = false;
            this.dgvAbonnementsListe.Name = "dgvAbonnementsListe";
            this.dgvAbonnementsListe.ReadOnly = true;
            this.dgvAbonnementsListe.RowHeadersVisible = false;
            this.dgvAbonnementsListe.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAbonnementsListe.Size = new System.Drawing.Size(613, 200);
            this.dgvAbonnementsListe.TabIndex = 4;
            this.dgvAbonnementsListe.SelectionChanged += new System.EventHandler(this.dgvAbonnementsListe_SelectionChanged);
            this.dgvAbonnementsListe.Enter += new System.EventHandler(this.dgvLivresListe_Enter);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(540, 227);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 22);
            this.button1.TabIndex = 17;
            this.button1.Text = "Fermer";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 232);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(302, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Attention, ces abonnements expieront dans moins de 30 jours !";
            // 
            // FrmAbonnements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 270);
            this.Controls.Add(this.grpAbonnements);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAbonnements";
            this.Text = "FrmAbonnements";
            this.Load += new System.EventHandler(this.FrmAbonnements_Load);
            this.grpAbonnements.ResumeLayout(false);
            this.grpAbonnements.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAbonnementsListe)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpAbonnements;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLivresAnnulRayons;
        private System.Windows.Forms.Button btnlivresAnnulPublics;
        private System.Windows.Forms.Button btnLivresAnnulGenres;
        private System.Windows.Forms.DataGridView dgvAbonnementsListe;
        private System.Windows.Forms.Button button1;
    }
}