namespace RawToolsGUI
{
    partial class RawToolsGUI
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
            this.groupBoxMode = new System.Windows.Forms.GroupBox();
            this.checkBoxModeQC = new System.Windows.Forms.CheckBox();
            this.checkBoxModeParse = new System.Windows.Forms.CheckBox();
            this.groupBoxRawFiles = new System.Windows.Forms.GroupBox();
            this.buttonSelectFiles = new System.Windows.Forms.Button();
            this.buttonSelectDirectory = new System.Windows.Forms.Button();
            this.selectRawFiles = new System.Windows.Forms.OpenFileDialog();
            this.txtbxRawFiles = new System.Windows.Forms.TextBox();
            this.txtbxRawFileDirectory = new System.Windows.Forms.TextBox();
            this.radioButtonSelectDirectory = new System.Windows.Forms.RadioButton();
            this.radioButtonSelectFiles = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckbxOutputMGF = new System.Windows.Forms.CheckBox();
            this.ckbxOutputParse = new System.Windows.Forms.CheckBox();
            this.ckbxOutputQuant = new System.Windows.Forms.CheckBox();
            this.ckbxOutputMetrics = new System.Windows.Forms.CheckBox();
            this.ckbxOutputChromatograms = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ckbxRefinePrecursor = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBoxMode.SuspendLayout();
            this.groupBoxRawFiles.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMode
            // 
            this.groupBoxMode.Controls.Add(this.checkBoxModeQC);
            this.groupBoxMode.Controls.Add(this.checkBoxModeParse);
            this.groupBoxMode.Location = new System.Drawing.Point(12, 12);
            this.groupBoxMode.Name = "groupBoxMode";
            this.groupBoxMode.Size = new System.Drawing.Size(90, 85);
            this.groupBoxMode.TabIndex = 0;
            this.groupBoxMode.TabStop = false;
            this.groupBoxMode.Text = "Mode";
            // 
            // checkBoxModeQC
            // 
            this.checkBoxModeQC.AutoSize = true;
            this.checkBoxModeQC.Location = new System.Drawing.Point(6, 48);
            this.checkBoxModeQC.Name = "checkBoxModeQC";
            this.checkBoxModeQC.Size = new System.Drawing.Size(50, 21);
            this.checkBoxModeQC.TabIndex = 1;
            this.checkBoxModeQC.Text = "QC";
            this.checkBoxModeQC.UseVisualStyleBackColor = true;
            // 
            // checkBoxModeParse
            // 
            this.checkBoxModeParse.AutoSize = true;
            this.checkBoxModeParse.Location = new System.Drawing.Point(6, 21);
            this.checkBoxModeParse.Name = "checkBoxModeParse";
            this.checkBoxModeParse.Size = new System.Drawing.Size(67, 21);
            this.checkBoxModeParse.TabIndex = 0;
            this.checkBoxModeParse.Text = "Parse";
            this.checkBoxModeParse.UseVisualStyleBackColor = true;
            // 
            // groupBoxRawFiles
            // 
            this.groupBoxRawFiles.Controls.Add(this.radioButtonSelectFiles);
            this.groupBoxRawFiles.Controls.Add(this.radioButtonSelectDirectory);
            this.groupBoxRawFiles.Controls.Add(this.txtbxRawFiles);
            this.groupBoxRawFiles.Controls.Add(this.txtbxRawFileDirectory);
            this.groupBoxRawFiles.Controls.Add(this.buttonSelectFiles);
            this.groupBoxRawFiles.Controls.Add(this.buttonSelectDirectory);
            this.groupBoxRawFiles.Location = new System.Drawing.Point(108, 12);
            this.groupBoxRawFiles.Name = "groupBoxRawFiles";
            this.groupBoxRawFiles.Size = new System.Drawing.Size(654, 85);
            this.groupBoxRawFiles.TabIndex = 1;
            this.groupBoxRawFiles.TabStop = false;
            this.groupBoxRawFiles.Text = "Raw Files";
            // 
            // buttonSelectFiles
            // 
            this.buttonSelectFiles.Location = new System.Drawing.Point(29, 48);
            this.buttonSelectFiles.Name = "buttonSelectFiles";
            this.buttonSelectFiles.Size = new System.Drawing.Size(128, 23);
            this.buttonSelectFiles.TabIndex = 3;
            this.buttonSelectFiles.Text = "Select File(s)";
            this.buttonSelectFiles.UseVisualStyleBackColor = true;
            this.buttonSelectFiles.Click += new System.EventHandler(this.buttonSelectFiles_Click);
            // 
            // buttonSelectDirectory
            // 
            this.buttonSelectDirectory.Location = new System.Drawing.Point(29, 22);
            this.buttonSelectDirectory.Name = "buttonSelectDirectory";
            this.buttonSelectDirectory.Size = new System.Drawing.Size(128, 23);
            this.buttonSelectDirectory.TabIndex = 2;
            this.buttonSelectDirectory.Text = "Select Directory";
            this.buttonSelectDirectory.UseVisualStyleBackColor = true;
            this.buttonSelectDirectory.Click += new System.EventHandler(this.buttonSelectDirectory_Click);
            // 
            // selectRawFiles
            // 
            this.selectRawFiles.FileName = "selectRawFiles";
            this.selectRawFiles.Multiselect = true;
            // 
            // txtbxRawFiles
            // 
            this.txtbxRawFiles.Location = new System.Drawing.Point(163, 48);
            this.txtbxRawFiles.Name = "txtbxRawFiles";
            this.txtbxRawFiles.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtbxRawFiles.Size = new System.Drawing.Size(475, 22);
            this.txtbxRawFiles.TabIndex = 2;
            // 
            // txtbxRawFileDirectory
            // 
            this.txtbxRawFileDirectory.Location = new System.Drawing.Point(163, 23);
            this.txtbxRawFileDirectory.Name = "txtbxRawFileDirectory";
            this.txtbxRawFileDirectory.Size = new System.Drawing.Size(475, 22);
            this.txtbxRawFileDirectory.TabIndex = 3;
            // 
            // radioButtonSelectDirectory
            // 
            this.radioButtonSelectDirectory.AutoSize = true;
            this.radioButtonSelectDirectory.Location = new System.Drawing.Point(6, 25);
            this.radioButtonSelectDirectory.Name = "radioButtonSelectDirectory";
            this.radioButtonSelectDirectory.Size = new System.Drawing.Size(17, 16);
            this.radioButtonSelectDirectory.TabIndex = 2;
            this.radioButtonSelectDirectory.TabStop = true;
            this.radioButtonSelectDirectory.UseVisualStyleBackColor = true;
            // 
            // radioButtonSelectFiles
            // 
            this.radioButtonSelectFiles.AutoSize = true;
            this.radioButtonSelectFiles.Location = new System.Drawing.Point(6, 51);
            this.radioButtonSelectFiles.Name = "radioButtonSelectFiles";
            this.radioButtonSelectFiles.Size = new System.Drawing.Size(17, 16);
            this.radioButtonSelectFiles.TabIndex = 2;
            this.radioButtonSelectFiles.TabStop = true;
            this.radioButtonSelectFiles.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckbxOutputChromatograms);
            this.groupBox1.Controls.Add(this.ckbxOutputMetrics);
            this.groupBox1.Controls.Add(this.ckbxOutputQuant);
            this.groupBox1.Controls.Add(this.ckbxOutputParse);
            this.groupBox1.Controls.Add(this.ckbxOutputMGF);
            this.groupBox1.Location = new System.Drawing.Point(12, 103);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(333, 120);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Output";
            // 
            // ckbxOutputMGF
            // 
            this.ckbxOutputMGF.AutoSize = true;
            this.ckbxOutputMGF.Location = new System.Drawing.Point(6, 21);
            this.ckbxOutputMGF.Name = "ckbxOutputMGF";
            this.ckbxOutputMGF.Size = new System.Drawing.Size(60, 21);
            this.ckbxOutputMGF.TabIndex = 0;
            this.ckbxOutputMGF.Text = "MGF";
            this.ckbxOutputMGF.UseVisualStyleBackColor = true;
            // 
            // ckbxOutputParse
            // 
            this.ckbxOutputParse.AutoSize = true;
            this.ckbxOutputParse.Location = new System.Drawing.Point(6, 48);
            this.ckbxOutputParse.Name = "ckbxOutputParse";
            this.ckbxOutputParse.Size = new System.Drawing.Size(108, 21);
            this.ckbxOutputParse.TabIndex = 1;
            this.ckbxOutputParse.Text = "Parse Matrix";
            this.ckbxOutputParse.UseVisualStyleBackColor = true;
            // 
            // ckbxOutputQuant
            // 
            this.ckbxOutputQuant.AutoSize = true;
            this.ckbxOutputQuant.Location = new System.Drawing.Point(6, 75);
            this.ckbxOutputQuant.Name = "ckbxOutputQuant";
            this.ckbxOutputQuant.Size = new System.Drawing.Size(103, 21);
            this.ckbxOutputQuant.TabIndex = 2;
            this.ckbxOutputQuant.Text = "Quant Data";
            this.ckbxOutputQuant.UseVisualStyleBackColor = true;
            // 
            // ckbxOutputMetrics
            // 
            this.ckbxOutputMetrics.AutoSize = true;
            this.ckbxOutputMetrics.Location = new System.Drawing.Point(125, 21);
            this.ckbxOutputMetrics.Name = "ckbxOutputMetrics";
            this.ckbxOutputMetrics.Size = new System.Drawing.Size(75, 21);
            this.ckbxOutputMetrics.TabIndex = 3;
            this.ckbxOutputMetrics.Text = "Metrics";
            this.ckbxOutputMetrics.UseVisualStyleBackColor = true;
            // 
            // ckbxOutputChromatograms
            // 
            this.ckbxOutputChromatograms.AutoSize = true;
            this.ckbxOutputChromatograms.Location = new System.Drawing.Point(125, 48);
            this.ckbxOutputChromatograms.Name = "ckbxOutputChromatograms";
            this.ckbxOutputChromatograms.Size = new System.Drawing.Size(130, 21);
            this.ckbxOutputChromatograms.TabIndex = 4;
            this.ckbxOutputChromatograms.Text = "Chromatograms";
            this.ckbxOutputChromatograms.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Controls.Add(this.ckbxRefinePrecursor);
            this.groupBox2.Location = new System.Drawing.Point(351, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(411, 120);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Common Options";
            // 
            // ckbxRefinePrecursor
            // 
            this.ckbxRefinePrecursor.AutoSize = true;
            this.ckbxRefinePrecursor.Location = new System.Drawing.Point(6, 21);
            this.ckbxRefinePrecursor.Name = "ckbxRefinePrecursor";
            this.ckbxRefinePrecursor.Size = new System.Drawing.Size(252, 21);
            this.ckbxRefinePrecursor.TabIndex = 0;
            this.ckbxRefinePrecursor.Text = "Refine Precursor Mass and Charge";
            this.ckbxRefinePrecursor.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.comboBox1.Location = new System.Drawing.Point(152, 48);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(106, 24);
            this.comboBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Maximum charge:";
            // 
            // groupBox3
            // 
            this.groupBox3.Location = new System.Drawing.Point(12, 229);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(750, 171);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "QC Options";
            // 
            // groupBox5
            // 
            this.groupBox5.Location = new System.Drawing.Point(18, 461);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(734, 79);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "MGF options";
            // 
            // groupBox6
            // 
            this.groupBox6.Location = new System.Drawing.Point(12, 564);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(750, 132);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Chromatogram Options";
            // 
            // RawToolsGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 729);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxRawFiles);
            this.Controls.Add(this.groupBoxMode);
            this.Name = "RawToolsGUI";
            this.Text = "RawToolsGUI";
            this.groupBoxMode.ResumeLayout(false);
            this.groupBoxMode.PerformLayout();
            this.groupBoxRawFiles.ResumeLayout(false);
            this.groupBoxRawFiles.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxMode;
        private System.Windows.Forms.CheckBox checkBoxModeQC;
        private System.Windows.Forms.CheckBox checkBoxModeParse;
        private System.Windows.Forms.GroupBox groupBoxRawFiles;
        private System.Windows.Forms.Button buttonSelectFiles;
        private System.Windows.Forms.Button buttonSelectDirectory;
        private System.Windows.Forms.OpenFileDialog selectRawFiles;
        private System.Windows.Forms.RadioButton radioButtonSelectFiles;
        private System.Windows.Forms.RadioButton radioButtonSelectDirectory;
        private System.Windows.Forms.TextBox txtbxRawFiles;
        private System.Windows.Forms.TextBox txtbxRawFileDirectory;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckbxOutputChromatograms;
        private System.Windows.Forms.CheckBox ckbxOutputMetrics;
        private System.Windows.Forms.CheckBox ckbxOutputQuant;
        private System.Windows.Forms.CheckBox ckbxOutputParse;
        private System.Windows.Forms.CheckBox ckbxOutputMGF;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox ckbxRefinePrecursor;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
    }
}

