namespace RawToolsGUI
{
    partial class PeptideModificationForm
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
            this.buttonAddFixedMod = new System.Windows.Forms.Button();
            this.buttonRemoveFixedMod = new System.Windows.Forms.Button();
            this.buttonEditModsOK = new System.Windows.Forms.Button();
            this.buttonEditModsCancel = new System.Windows.Forms.Button();
            this.UseMod = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ModAA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModMass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewModifications = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModifications)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAddFixedMod
            // 
            this.buttonAddFixedMod.Location = new System.Drawing.Point(12, 343);
            this.buttonAddFixedMod.Name = "buttonAddFixedMod";
            this.buttonAddFixedMod.Size = new System.Drawing.Size(256, 23);
            this.buttonAddFixedMod.TabIndex = 4;
            this.buttonAddFixedMod.Text = "Add Fixed Modification";
            this.buttonAddFixedMod.UseVisualStyleBackColor = true;
            this.buttonAddFixedMod.Click += new System.EventHandler(this.buttonAddFixedMod_Click);
            // 
            // buttonRemoveFixedMod
            // 
            this.buttonRemoveFixedMod.Location = new System.Drawing.Point(274, 343);
            this.buttonRemoveFixedMod.Name = "buttonRemoveFixedMod";
            this.buttonRemoveFixedMod.Size = new System.Drawing.Size(250, 23);
            this.buttonRemoveFixedMod.TabIndex = 5;
            this.buttonRemoveFixedMod.Text = "Remove Selected Fixed Mod";
            this.buttonRemoveFixedMod.UseVisualStyleBackColor = true;
            this.buttonRemoveFixedMod.Click += new System.EventHandler(this.buttonRemoveFixedMod_Click);
            // 
            // buttonEditModsOK
            // 
            this.buttonEditModsOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonEditModsOK.Location = new System.Drawing.Point(292, 416);
            this.buttonEditModsOK.Name = "buttonEditModsOK";
            this.buttonEditModsOK.Size = new System.Drawing.Size(101, 45);
            this.buttonEditModsOK.TabIndex = 6;
            this.buttonEditModsOK.Text = "OK";
            this.buttonEditModsOK.UseVisualStyleBackColor = true;
            // 
            // buttonEditModsCancel
            // 
            this.buttonEditModsCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonEditModsCancel.Location = new System.Drawing.Point(399, 416);
            this.buttonEditModsCancel.Name = "buttonEditModsCancel";
            this.buttonEditModsCancel.Size = new System.Drawing.Size(101, 45);
            this.buttonEditModsCancel.TabIndex = 7;
            this.buttonEditModsCancel.Text = "Cancel";
            this.buttonEditModsCancel.UseVisualStyleBackColor = true;
            // 
            // UseMod
            // 
            this.UseMod.HeaderText = "Use";
            this.UseMod.Name = "UseMod";
            // 
            // ModAA
            // 
            this.ModAA.HeaderText = "Amino Acid";
            this.ModAA.Name = "ModAA";
            // 
            // ModMass
            // 
            this.ModMass.HeaderText = "Modification Mass";
            this.ModMass.Name = "ModMass";
            // 
            // ModType
            // 
            this.ModType.HeaderText = "Type";
            this.ModType.Name = "ModType";
            this.ModType.ReadOnly = true;
            // 
            // dataGridViewModifications
            // 
            this.dataGridViewModifications.AllowUserToAddRows = false;
            this.dataGridViewModifications.AllowUserToDeleteRows = false;
            this.dataGridViewModifications.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewModifications.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewModifications.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ModType,
            this.ModMass,
            this.ModAA,
            this.UseMod});
            this.dataGridViewModifications.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewModifications.Name = "dataGridViewModifications";
            this.dataGridViewModifications.RowTemplate.Height = 24;
            this.dataGridViewModifications.Size = new System.Drawing.Size(512, 325);
            this.dataGridViewModifications.TabIndex = 3;
            // 
            // PeptideModificationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 484);
            this.Controls.Add(this.buttonEditModsCancel);
            this.Controls.Add(this.buttonEditModsOK);
            this.Controls.Add(this.buttonRemoveFixedMod);
            this.Controls.Add(this.dataGridViewModifications);
            this.Controls.Add(this.buttonAddFixedMod);
            this.Name = "PeptideModificationForm";
            this.Text = "Add/Review Peptide Modifications";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModifications)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonAddFixedMod;
        private System.Windows.Forms.Button buttonRemoveFixedMod;
        private System.Windows.Forms.Button buttonEditModsOK;
        private System.Windows.Forms.Button buttonEditModsCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UseMod;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModAA;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModMass;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModType;
        public System.Windows.Forms.DataGridView dataGridViewModifications;
    }
}