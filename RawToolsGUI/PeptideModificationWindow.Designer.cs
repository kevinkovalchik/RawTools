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
            this.components = new System.ComponentModel.Container();
            this.buttonAddFixedMod = new System.Windows.Forms.Button();
            this.buttonRemoveFixedMod = new System.Windows.Forms.Button();
            this.buttonEditModsOK = new System.Windows.Forms.Button();
            this.buttonEditModsCancel = new System.Windows.Forms.Button();
            this.dataGridViewModifications = new System.Windows.Forms.DataGridView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ModMass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModAA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FixedMod = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModifications)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonAddFixedMod
            // 
            this.buttonAddFixedMod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddFixedMod.Location = new System.Drawing.Point(17, 351);
            this.buttonAddFixedMod.Name = "buttonAddFixedMod";
            this.buttonAddFixedMod.Size = new System.Drawing.Size(164, 27);
            this.buttonAddFixedMod.TabIndex = 4;
            this.buttonAddFixedMod.Text = "Add Fixed Modification";
            this.toolTip1.SetToolTip(this.buttonAddFixedMod, "Add an additional fixed modification to the table.");
            this.buttonAddFixedMod.UseVisualStyleBackColor = true;
            this.buttonAddFixedMod.Click += new System.EventHandler(this.buttonAddFixedMod_Click);
            // 
            // buttonRemoveFixedMod
            // 
            this.buttonRemoveFixedMod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveFixedMod.Location = new System.Drawing.Point(195, 351);
            this.buttonRemoveFixedMod.Name = "buttonRemoveFixedMod";
            this.buttonRemoveFixedMod.Size = new System.Drawing.Size(209, 27);
            this.buttonRemoveFixedMod.TabIndex = 5;
            this.buttonRemoveFixedMod.Text = "Remove Selected Fixed Mod";
            this.toolTip1.SetToolTip(this.buttonRemoveFixedMod, "Remove the selected fixed modifiation from the table.");
            this.buttonRemoveFixedMod.UseVisualStyleBackColor = true;
            this.buttonRemoveFixedMod.Click += new System.EventHandler(this.buttonRemoveFixedMod_Click);
            // 
            // buttonEditModsOK
            // 
            this.buttonEditModsOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditModsOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonEditModsOK.Location = new System.Drawing.Point(431, 342);
            this.buttonEditModsOK.Name = "buttonEditModsOK";
            this.buttonEditModsOK.Size = new System.Drawing.Size(93, 45);
            this.buttonEditModsOK.TabIndex = 6;
            this.buttonEditModsOK.Text = "OK";
            this.buttonEditModsOK.UseVisualStyleBackColor = true;
            // 
            // buttonEditModsCancel
            // 
            this.buttonEditModsCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditModsCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonEditModsCancel.Location = new System.Drawing.Point(538, 342);
            this.buttonEditModsCancel.Name = "buttonEditModsCancel";
            this.buttonEditModsCancel.Size = new System.Drawing.Size(93, 45);
            this.buttonEditModsCancel.TabIndex = 7;
            this.buttonEditModsCancel.Text = "Cancel";
            this.buttonEditModsCancel.UseVisualStyleBackColor = true;
            // 
            // dataGridViewModifications
            // 
            this.dataGridViewModifications.AllowUserToAddRows = false;
            this.dataGridViewModifications.AllowUserToDeleteRows = false;
            this.dataGridViewModifications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewModifications.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewModifications.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewModifications.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ModMass,
            this.ModAA,
            this.FixedMod});
            this.dataGridViewModifications.Location = new System.Drawing.Point(10, 10);
            this.dataGridViewModifications.MultiSelect = false;
            this.dataGridViewModifications.Name = "dataGridViewModifications";
            this.dataGridViewModifications.RowTemplate.Height = 24;
            this.dataGridViewModifications.Size = new System.Drawing.Size(624, 312);
            this.dataGridViewModifications.TabIndex = 3;
            this.dataGridViewModifications.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridViewModifications_CellValidating);
            // 
            // ModMass
            // 
            this.ModMass.HeaderText = "Modification Mass";
            this.ModMass.Name = "ModMass";
            // 
            // ModAA
            // 
            this.ModAA.HeaderText = "Amino Acid";
            this.ModAA.Name = "ModAA";
            // 
            // FixedMod
            // 
            this.FixedMod.HeaderText = "Fixed";
            this.FixedMod.Name = "FixedMod";
            // 
            // PeptideModificationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 404);
            this.Controls.Add(this.buttonEditModsCancel);
            this.Controls.Add(this.buttonEditModsOK);
            this.Controls.Add(this.buttonRemoveFixedMod);
            this.Controls.Add(this.dataGridViewModifications);
            this.Controls.Add(this.buttonAddFixedMod);
            this.MinimumSize = new System.Drawing.Size(666, 451);
            this.Name = "PeptideModificationForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Add/Review Peptide Modifications";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewModifications)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonAddFixedMod;
        private System.Windows.Forms.Button buttonRemoveFixedMod;
        private System.Windows.Forms.Button buttonEditModsOK;
        private System.Windows.Forms.Button buttonEditModsCancel;
        public System.Windows.Forms.DataGridView dataGridViewModifications;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModMass;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModAA;
        private System.Windows.Forms.DataGridViewCheckBoxColumn FixedMod;
    }
}