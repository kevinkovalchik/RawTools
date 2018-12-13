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
            this.components = new System.ComponentModel.Container();
            this.groupBoxMode = new System.Windows.Forms.GroupBox();
            this.checkBoxModeQC = new System.Windows.Forms.CheckBox();
            this.checkBoxModeParse = new System.Windows.Forms.CheckBox();
            this.groupBoxRawFiles = new System.Windows.Forms.GroupBox();
            this.radioButtonSelectFiles = new System.Windows.Forms.RadioButton();
            this.radioButtonSelectDirectory = new System.Windows.Forms.RadioButton();
            this.textBoxRawFiles = new System.Windows.Forms.TextBox();
            this.textBoxRawFileDirectory = new System.Windows.Forms.TextBox();
            this.buttonSelectFiles = new System.Windows.Forms.Button();
            this.buttonSelectDirectory = new System.Windows.Forms.Button();
            this.selectRawFiles = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckbxOutputChromatograms = new System.Windows.Forms.CheckBox();
            this.ckbxOutputMetrics = new System.Windows.Forms.CheckBox();
            this.ckbxOutputQuant = new System.Windows.Forms.CheckBox();
            this.ckbxOutputParse = new System.Windows.Forms.CheckBox();
            this.ckbxOutputMGF = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelMaxCharge = new System.Windows.Forms.Label();
            this.comboBoxMaxCharge = new System.Windows.Forms.ComboBox();
            this.checkBoxRefinePrecursor = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxNumSpectra = new System.Windows.Forms.TextBox();
            this.labelNumSpectra = new System.Windows.Forms.Label();
            this.checkBoxAutoSearchIdentipy = new System.Windows.Forms.CheckBox();
            this.textBoxIdentipyScript = new System.Windows.Forms.TextBox();
            this.textBoxPythonExe = new System.Windows.Forms.TextBox();
            this.textBoxXTandemDir = new System.Windows.Forms.TextBox();
            this.buttonXTandemDir = new System.Windows.Forms.Button();
            this.textBoxFastaFile = new System.Windows.Forms.TextBox();
            this.buttonIdentipyScript = new System.Windows.Forms.Button();
            this.buttonPythonExe = new System.Windows.Forms.Button();
            this.buttonFastaFile = new System.Windows.Forms.Button();
            this.radioButtonSearchIdentipy = new System.Windows.Forms.RadioButton();
            this.radioButtonSearchXTandem = new System.Windows.Forms.RadioButton();
            this.radioButtonSearchNone = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelMinCharge = new System.Windows.Forms.Label();
            this.comboBoxMinCharge = new System.Windows.Forms.ComboBox();
            this.groupBoxMode.SuspendLayout();
            this.groupBoxRawFiles.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMode
            // 
            this.groupBoxMode.Controls.Add(this.checkBoxModeQC);
            this.groupBoxMode.Controls.Add(this.checkBoxModeParse);
            this.groupBoxMode.Location = new System.Drawing.Point(3, 3);
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
            this.checkBoxModeQC.CheckedChanged += new System.EventHandler(this.checkBoxModeQC_CheckedChanged);
            // 
            // checkBoxModeParse
            // 
            this.checkBoxModeParse.AutoSize = true;
            this.checkBoxModeParse.Checked = true;
            this.checkBoxModeParse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxModeParse.Location = new System.Drawing.Point(6, 21);
            this.checkBoxModeParse.Name = "checkBoxModeParse";
            this.checkBoxModeParse.Size = new System.Drawing.Size(67, 21);
            this.checkBoxModeParse.TabIndex = 0;
            this.checkBoxModeParse.Text = "Parse";
            this.checkBoxModeParse.UseVisualStyleBackColor = true;
            this.checkBoxModeParse.CheckedChanged += new System.EventHandler(this.checkBoxModeParse_CheckedChanged);
            // 
            // groupBoxRawFiles
            // 
            this.groupBoxRawFiles.Controls.Add(this.radioButtonSelectFiles);
            this.groupBoxRawFiles.Controls.Add(this.radioButtonSelectDirectory);
            this.groupBoxRawFiles.Controls.Add(this.textBoxRawFiles);
            this.groupBoxRawFiles.Controls.Add(this.textBoxRawFileDirectory);
            this.groupBoxRawFiles.Controls.Add(this.buttonSelectFiles);
            this.groupBoxRawFiles.Controls.Add(this.buttonSelectDirectory);
            this.groupBoxRawFiles.Location = new System.Drawing.Point(99, 3);
            this.groupBoxRawFiles.Name = "groupBoxRawFiles";
            this.groupBoxRawFiles.Size = new System.Drawing.Size(654, 85);
            this.groupBoxRawFiles.TabIndex = 1;
            this.groupBoxRawFiles.TabStop = false;
            this.groupBoxRawFiles.Text = "Raw Files";
            // 
            // radioButtonSelectFiles
            // 
            this.radioButtonSelectFiles.AutoSize = true;
            this.radioButtonSelectFiles.Location = new System.Drawing.Point(6, 57);
            this.radioButtonSelectFiles.Name = "radioButtonSelectFiles";
            this.radioButtonSelectFiles.Size = new System.Drawing.Size(17, 16);
            this.radioButtonSelectFiles.TabIndex = 2;
            this.radioButtonSelectFiles.UseVisualStyleBackColor = true;
            // 
            // radioButtonSelectDirectory
            // 
            this.radioButtonSelectDirectory.AutoSize = true;
            this.radioButtonSelectDirectory.Checked = true;
            this.radioButtonSelectDirectory.Location = new System.Drawing.Point(6, 25);
            this.radioButtonSelectDirectory.Name = "radioButtonSelectDirectory";
            this.radioButtonSelectDirectory.Size = new System.Drawing.Size(17, 16);
            this.radioButtonSelectDirectory.TabIndex = 2;
            this.radioButtonSelectDirectory.TabStop = true;
            this.radioButtonSelectDirectory.UseVisualStyleBackColor = true;
            this.radioButtonSelectDirectory.CheckedChanged += new System.EventHandler(this.radioButtonSelectDirectory_CheckedChanged);
            // 
            // textBoxRawFiles
            // 
            this.textBoxRawFiles.Enabled = false;
            this.textBoxRawFiles.Location = new System.Drawing.Point(163, 51);
            this.textBoxRawFiles.Name = "textBoxRawFiles";
            this.textBoxRawFiles.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBoxRawFiles.Size = new System.Drawing.Size(475, 22);
            this.textBoxRawFiles.TabIndex = 2;
            // 
            // textBoxRawFileDirectory
            // 
            this.textBoxRawFileDirectory.Location = new System.Drawing.Point(163, 23);
            this.textBoxRawFileDirectory.Name = "textBoxRawFileDirectory";
            this.textBoxRawFileDirectory.Size = new System.Drawing.Size(475, 22);
            this.textBoxRawFileDirectory.TabIndex = 3;
            // 
            // buttonSelectFiles
            // 
            this.buttonSelectFiles.Enabled = false;
            this.buttonSelectFiles.Location = new System.Drawing.Point(29, 51);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckbxOutputChromatograms);
            this.groupBox1.Controls.Add(this.ckbxOutputMetrics);
            this.groupBox1.Controls.Add(this.ckbxOutputQuant);
            this.groupBox1.Controls.Add(this.ckbxOutputParse);
            this.groupBox1.Controls.Add(this.ckbxOutputMGF);
            this.groupBox1.Location = new System.Drawing.Point(3, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(333, 120);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Output";
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxMinCharge);
            this.groupBox2.Controls.Add(this.labelMinCharge);
            this.groupBox2.Controls.Add(this.labelMaxCharge);
            this.groupBox2.Controls.Add(this.comboBoxMaxCharge);
            this.groupBox2.Controls.Add(this.checkBoxRefinePrecursor);
            this.groupBox2.Location = new System.Drawing.Point(342, 94);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(411, 120);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Common Options";
            // 
            // labelMaxCharge
            // 
            this.labelMaxCharge.AutoSize = true;
            this.labelMaxCharge.Location = new System.Drawing.Point(32, 79);
            this.labelMaxCharge.Name = "labelMaxCharge";
            this.labelMaxCharge.Size = new System.Drawing.Size(118, 17);
            this.labelMaxCharge.TabIndex = 2;
            this.labelMaxCharge.Text = "Maximum charge:";
            // 
            // comboBoxMaxCharge
            // 
            this.comboBoxMaxCharge.FormattingEnabled = true;
            this.comboBoxMaxCharge.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.comboBoxMaxCharge.Location = new System.Drawing.Point(152, 75);
            this.comboBoxMaxCharge.Name = "comboBoxMaxCharge";
            this.comboBoxMaxCharge.Size = new System.Drawing.Size(106, 24);
            this.comboBoxMaxCharge.TabIndex = 1;
            this.comboBoxMaxCharge.SelectedIndexChanged += new System.EventHandler(this.comboBoxMaxCharge_SelectedIndexChanged);
            this.comboBoxMaxCharge.Enter += new System.EventHandler(this.comboBoxMaxCharge_Enter);
            // 
            // checkBoxRefinePrecursor
            // 
            this.checkBoxRefinePrecursor.AutoSize = true;
            this.checkBoxRefinePrecursor.Checked = true;
            this.checkBoxRefinePrecursor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRefinePrecursor.Location = new System.Drawing.Point(6, 21);
            this.checkBoxRefinePrecursor.Name = "checkBoxRefinePrecursor";
            this.checkBoxRefinePrecursor.Size = new System.Drawing.Size(252, 21);
            this.checkBoxRefinePrecursor.TabIndex = 0;
            this.checkBoxRefinePrecursor.Text = "Refine Precursor Mass and Charge";
            this.checkBoxRefinePrecursor.UseVisualStyleBackColor = true;
            this.checkBoxRefinePrecursor.CheckedChanged += new System.EventHandler(this.checkBoxRefinePrecursor_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxNumSpectra);
            this.groupBox3.Controls.Add(this.labelNumSpectra);
            this.groupBox3.Controls.Add(this.checkBoxAutoSearchIdentipy);
            this.groupBox3.Controls.Add(this.textBoxIdentipyScript);
            this.groupBox3.Controls.Add(this.textBoxPythonExe);
            this.groupBox3.Controls.Add(this.textBoxXTandemDir);
            this.groupBox3.Controls.Add(this.buttonXTandemDir);
            this.groupBox3.Controls.Add(this.textBoxFastaFile);
            this.groupBox3.Controls.Add(this.buttonIdentipyScript);
            this.groupBox3.Controls.Add(this.buttonPythonExe);
            this.groupBox3.Controls.Add(this.buttonFastaFile);
            this.groupBox3.Controls.Add(this.radioButtonSearchIdentipy);
            this.groupBox3.Controls.Add(this.radioButtonSearchXTandem);
            this.groupBox3.Controls.Add(this.radioButtonSearchNone);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(3, 220);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(750, 207);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "QC Options";
            // 
            // textBoxNumSpectra
            // 
            this.textBoxNumSpectra.Enabled = false;
            this.textBoxNumSpectra.Location = new System.Drawing.Point(362, 169);
            this.textBoxNumSpectra.Name = "textBoxNumSpectra";
            this.textBoxNumSpectra.Size = new System.Drawing.Size(100, 22);
            this.textBoxNumSpectra.TabIndex = 20;
            // 
            // labelNumSpectra
            // 
            this.labelNumSpectra.AutoSize = true;
            this.labelNumSpectra.Enabled = false;
            this.labelNumSpectra.Location = new System.Drawing.Point(160, 172);
            this.labelNumSpectra.Name = "labelNumSpectra";
            this.labelNumSpectra.Size = new System.Drawing.Size(196, 17);
            this.labelNumSpectra.TabIndex = 19;
            this.labelNumSpectra.Text = "Number of Spectra to Search:";
            // 
            // checkBoxAutoSearchIdentipy
            // 
            this.checkBoxAutoSearchIdentipy.AutoSize = true;
            this.checkBoxAutoSearchIdentipy.Checked = true;
            this.checkBoxAutoSearchIdentipy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoSearchIdentipy.Enabled = false;
            this.checkBoxAutoSearchIdentipy.Location = new System.Drawing.Point(374, 25);
            this.checkBoxAutoSearchIdentipy.Name = "checkBoxAutoSearchIdentipy";
            this.checkBoxAutoSearchIdentipy.Size = new System.Drawing.Size(286, 21);
            this.checkBoxAutoSearchIdentipy.TabIndex = 8;
            this.checkBoxAutoSearchIdentipy.Text = "Let RawTools search for Python/IdentiPy";
            this.checkBoxAutoSearchIdentipy.UseVisualStyleBackColor = true;
            this.checkBoxAutoSearchIdentipy.CheckedChanged += new System.EventHandler(this.checkBoxAutoSearchIdentipy_CheckedChanged);
            // 
            // textBoxIdentipyScript
            // 
            this.textBoxIdentipyScript.Enabled = false;
            this.textBoxIdentipyScript.Location = new System.Drawing.Point(163, 105);
            this.textBoxIdentipyScript.Name = "textBoxIdentipyScript";
            this.textBoxIdentipyScript.Size = new System.Drawing.Size(571, 22);
            this.textBoxIdentipyScript.TabIndex = 18;
            // 
            // textBoxPythonExe
            // 
            this.textBoxPythonExe.Enabled = false;
            this.textBoxPythonExe.Location = new System.Drawing.Point(163, 77);
            this.textBoxPythonExe.Name = "textBoxPythonExe";
            this.textBoxPythonExe.Size = new System.Drawing.Size(571, 22);
            this.textBoxPythonExe.TabIndex = 17;
            // 
            // textBoxXTandemDir
            // 
            this.textBoxXTandemDir.Enabled = false;
            this.textBoxXTandemDir.Location = new System.Drawing.Point(163, 48);
            this.textBoxXTandemDir.Name = "textBoxXTandemDir";
            this.textBoxXTandemDir.Size = new System.Drawing.Size(571, 22);
            this.textBoxXTandemDir.TabIndex = 16;
            // 
            // buttonXTandemDir
            // 
            this.buttonXTandemDir.Enabled = false;
            this.buttonXTandemDir.Location = new System.Drawing.Point(6, 47);
            this.buttonXTandemDir.Name = "buttonXTandemDir";
            this.buttonXTandemDir.Size = new System.Drawing.Size(151, 23);
            this.buttonXTandemDir.TabIndex = 15;
            this.buttonXTandemDir.Text = "X! Tandem Directory";
            this.toolTip1.SetToolTip(this.buttonXTandemDir, "Select the directory containing the X! Tandem executable (usually tandem.exe).");
            this.buttonXTandemDir.UseVisualStyleBackColor = true;
            this.buttonXTandemDir.Click += new System.EventHandler(this.buttonXTandemDir_Click);
            // 
            // textBoxFastaFile
            // 
            this.textBoxFastaFile.Enabled = false;
            this.textBoxFastaFile.Location = new System.Drawing.Point(163, 134);
            this.textBoxFastaFile.Name = "textBoxFastaFile";
            this.textBoxFastaFile.Size = new System.Drawing.Size(571, 22);
            this.textBoxFastaFile.TabIndex = 8;
            // 
            // buttonIdentipyScript
            // 
            this.buttonIdentipyScript.Enabled = false;
            this.buttonIdentipyScript.Location = new System.Drawing.Point(6, 105);
            this.buttonIdentipyScript.Name = "buttonIdentipyScript";
            this.buttonIdentipyScript.Size = new System.Drawing.Size(151, 23);
            this.buttonIdentipyScript.TabIndex = 14;
            this.buttonIdentipyScript.Text = "IdentiPy Script";
            this.buttonIdentipyScript.UseVisualStyleBackColor = true;
            this.buttonIdentipyScript.Click += new System.EventHandler(this.buttonIdentipyScript_Click);
            // 
            // buttonPythonExe
            // 
            this.buttonPythonExe.Enabled = false;
            this.buttonPythonExe.Location = new System.Drawing.Point(6, 76);
            this.buttonPythonExe.Name = "buttonPythonExe";
            this.buttonPythonExe.Size = new System.Drawing.Size(151, 23);
            this.buttonPythonExe.TabIndex = 13;
            this.buttonPythonExe.Text = "Python Executable";
            this.toolTip1.SetToolTip(this.buttonPythonExe, "Select the Python executable you wish to use (if you have multiple  Python\r\ninsta" +
        "llations, ensure this is the one with IdentiPy installed).");
            this.buttonPythonExe.UseVisualStyleBackColor = true;
            this.buttonPythonExe.Click += new System.EventHandler(this.buttonPythonExe_Click);
            // 
            // buttonFastaFile
            // 
            this.buttonFastaFile.Enabled = false;
            this.buttonFastaFile.Location = new System.Drawing.Point(6, 134);
            this.buttonFastaFile.Name = "buttonFastaFile";
            this.buttonFastaFile.Size = new System.Drawing.Size(151, 23);
            this.buttonFastaFile.TabIndex = 12;
            this.buttonFastaFile.Text = "Select Fasta File";
            this.buttonFastaFile.UseVisualStyleBackColor = true;
            this.buttonFastaFile.Click += new System.EventHandler(this.buttonFastaFile_Click);
            // 
            // radioButtonSearchIdentipy
            // 
            this.radioButtonSearchIdentipy.AutoSize = true;
            this.radioButtonSearchIdentipy.Location = new System.Drawing.Point(292, 25);
            this.radioButtonSearchIdentipy.Name = "radioButtonSearchIdentipy";
            this.radioButtonSearchIdentipy.Size = new System.Drawing.Size(79, 21);
            this.radioButtonSearchIdentipy.TabIndex = 11;
            this.radioButtonSearchIdentipy.Text = "IdentiPy";
            this.radioButtonSearchIdentipy.UseVisualStyleBackColor = true;
            this.radioButtonSearchIdentipy.CheckedChanged += new System.EventHandler(this.radioButtonSearchIdentipy_CheckedChanged);
            // 
            // radioButtonSearchXTandem
            // 
            this.radioButtonSearchXTandem.AutoSize = true;
            this.radioButtonSearchXTandem.Location = new System.Drawing.Point(189, 25);
            this.radioButtonSearchXTandem.Name = "radioButtonSearchXTandem";
            this.radioButtonSearchXTandem.Size = new System.Drawing.Size(97, 21);
            this.radioButtonSearchXTandem.TabIndex = 10;
            this.radioButtonSearchXTandem.Text = "X! Tandem";
            this.radioButtonSearchXTandem.UseVisualStyleBackColor = true;
            this.radioButtonSearchXTandem.CheckedChanged += new System.EventHandler(this.radioButtonSearchXTandem_CheckedChanged);
            // 
            // radioButtonSearchNone
            // 
            this.radioButtonSearchNone.AutoSize = true;
            this.radioButtonSearchNone.Checked = true;
            this.radioButtonSearchNone.Location = new System.Drawing.Point(120, 25);
            this.radioButtonSearchNone.Name = "radioButtonSearchNone";
            this.radioButtonSearchNone.Size = new System.Drawing.Size(63, 21);
            this.radioButtonSearchNone.TabIndex = 9;
            this.radioButtonSearchNone.TabStop = true;
            this.radioButtonSearchNone.Text = "None";
            this.radioButtonSearchNone.UseVisualStyleBackColor = true;
            this.radioButtonSearchNone.CheckedChanged += new System.EventHandler(this.radioButtonSearchNone_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Search Engine:";
            // 
            // groupBox5
            // 
            this.groupBox5.Location = new System.Drawing.Point(3, 433);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(750, 79);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "MGF options";
            // 
            // groupBox6
            // 
            this.groupBox6.Location = new System.Drawing.Point(3, 518);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(750, 132);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Chromatogram Options";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.groupBoxMode);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxRawFiles);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.groupBox2);
            this.flowLayoutPanel1.Controls.Add(this.groupBox3);
            this.flowLayoutPanel1.Controls.Add(this.groupBox5);
            this.flowLayoutPanel1.Controls.Add(this.groupBox6);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(773, 693);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // labelMinCharge
            // 
            this.labelMinCharge.AutoSize = true;
            this.labelMinCharge.Location = new System.Drawing.Point(32, 48);
            this.labelMinCharge.Name = "labelMinCharge";
            this.labelMinCharge.Size = new System.Drawing.Size(117, 17);
            this.labelMinCharge.TabIndex = 3;
            this.labelMinCharge.Text = "Minimum Charge:";
            // 
            // comboBoxMinCharge
            // 
            this.comboBoxMinCharge.FormattingEnabled = true;
            this.comboBoxMinCharge.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.comboBoxMinCharge.Location = new System.Drawing.Point(152, 45);
            this.comboBoxMinCharge.Name = "comboBoxMinCharge";
            this.comboBoxMinCharge.Size = new System.Drawing.Size(106, 24);
            this.comboBoxMinCharge.TabIndex = 9;
            this.comboBoxMinCharge.SelectedIndexChanged += new System.EventHandler(this.comboBoxMinCharge_SelectedIndexChanged);
            this.comboBoxMinCharge.Enter += new System.EventHandler(this.comboBoxMinCharge_Enter);
            // 
            // RawToolsGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 729);
            this.Controls.Add(this.flowLayoutPanel1);
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox textBoxRawFiles;
        private System.Windows.Forms.TextBox textBoxRawFileDirectory;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckbxOutputChromatograms;
        private System.Windows.Forms.CheckBox ckbxOutputMetrics;
        private System.Windows.Forms.CheckBox ckbxOutputQuant;
        private System.Windows.Forms.CheckBox ckbxOutputParse;
        private System.Windows.Forms.CheckBox ckbxOutputMGF;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelMaxCharge;
        private System.Windows.Forms.ComboBox comboBoxMaxCharge;
        private System.Windows.Forms.CheckBox checkBoxRefinePrecursor;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton radioButtonSearchIdentipy;
        private System.Windows.Forms.RadioButton radioButtonSearchXTandem;
        private System.Windows.Forms.RadioButton radioButtonSearchNone;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFastaFile;
        private System.Windows.Forms.Button buttonIdentipyScript;
        private System.Windows.Forms.Button buttonPythonExe;
        private System.Windows.Forms.Button buttonFastaFile;
        private System.Windows.Forms.TextBox textBoxXTandemDir;
        private System.Windows.Forms.Button buttonXTandemDir;
        private System.Windows.Forms.TextBox textBoxIdentipyScript;
        private System.Windows.Forms.TextBox textBoxPythonExe;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBoxAutoSearchIdentipy;
        private System.Windows.Forms.Label labelNumSpectra;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox textBoxNumSpectra;
        private System.Windows.Forms.ComboBox comboBoxMinCharge;
        private System.Windows.Forms.Label labelMinCharge;
    }
}

