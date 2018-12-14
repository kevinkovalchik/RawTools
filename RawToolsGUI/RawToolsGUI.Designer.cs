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
            this.groupBoxDataOutput = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBoxDataOutputDir = new System.Windows.Forms.TextBox();
            this.buttonDataOutputDir = new System.Windows.Forms.Button();
            this.ckbxOutputChromatograms = new System.Windows.Forms.CheckBox();
            this.ckbxOutputMetrics = new System.Windows.Forms.CheckBox();
            this.ckbxOutputQuant = new System.Windows.Forms.CheckBox();
            this.ckbxOutputParse = new System.Windows.Forms.CheckBox();
            this.ckbxOutputMGF = new System.Windows.Forms.CheckBox();
            this.groupBoxCommonOptions = new System.Windows.Forms.GroupBox();
            this.comboBoxMinCharge = new System.Windows.Forms.ComboBox();
            this.labelMinCharge = new System.Windows.Forms.Label();
            this.labelMaxCharge = new System.Windows.Forms.Label();
            this.comboBoxMaxCharge = new System.Windows.Forms.ComboBox();
            this.checkBoxRefinePrecursor = new System.Windows.Forms.CheckBox();
            this.groupBoxQcOptions = new System.Windows.Forms.GroupBox();
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
            this.textBoxReporterNumberMissingFilter = new System.Windows.Forms.TextBox();
            this.textBoxReporterIntensityFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxReporterIonFiltering = new System.Windows.Forms.CheckBox();
            this.groupBoxChromatograms = new System.Windows.Forms.GroupBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxReporterFilterMatrix = new System.Windows.Forms.CheckBox();
            this.checkBoxReporterFilterMGF = new System.Windows.Forms.CheckBox();
            this.groupBoxQuantOpt = new System.Windows.Forms.GroupBox();
            this.comboBoxLabelingReagents = new System.Windows.Forms.ComboBox();
            this.groupBoxMgfOpts = new System.Windows.Forms.GroupBox();
            this.groupBoxMode.SuspendLayout();
            this.groupBoxRawFiles.SuspendLayout();
            this.groupBoxDataOutput.SuspendLayout();
            this.groupBoxCommonOptions.SuspendLayout();
            this.groupBoxQcOptions.SuspendLayout();
            this.groupBoxChromatograms.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBoxQuantOpt.SuspendLayout();
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
            // groupBoxDataOutput
            // 
            this.groupBoxDataOutput.Controls.Add(this.checkBox1);
            this.groupBoxDataOutput.Controls.Add(this.textBoxDataOutputDir);
            this.groupBoxDataOutput.Controls.Add(this.buttonDataOutputDir);
            this.groupBoxDataOutput.Controls.Add(this.ckbxOutputChromatograms);
            this.groupBoxDataOutput.Controls.Add(this.ckbxOutputMetrics);
            this.groupBoxDataOutput.Controls.Add(this.ckbxOutputParse);
            this.groupBoxDataOutput.Controls.Add(this.ckbxOutputMGF);
            this.groupBoxDataOutput.Location = new System.Drawing.Point(3, 94);
            this.groupBoxDataOutput.Name = "groupBoxDataOutput";
            this.groupBoxDataOutput.Size = new System.Drawing.Size(333, 145);
            this.groupBoxDataOutput.TabIndex = 2;
            this.groupBoxDataOutput.TabStop = false;
            this.groupBoxDataOutput.Text = "Data Output";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(125, 48);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(204, 21);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "Output to different directory";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // textBoxDataOutputDir
            // 
            this.textBoxDataOutputDir.Enabled = false;
            this.textBoxDataOutputDir.Location = new System.Drawing.Point(150, 102);
            this.textBoxDataOutputDir.Name = "textBoxDataOutputDir";
            this.textBoxDataOutputDir.Size = new System.Drawing.Size(177, 22);
            this.textBoxDataOutputDir.TabIndex = 11;
            // 
            // buttonDataOutputDir
            // 
            this.buttonDataOutputDir.Enabled = false;
            this.buttonDataOutputDir.Location = new System.Drawing.Point(6, 102);
            this.buttonDataOutputDir.Name = "buttonDataOutputDir";
            this.buttonDataOutputDir.Size = new System.Drawing.Size(138, 23);
            this.buttonDataOutputDir.TabIndex = 10;
            this.buttonDataOutputDir.Text = "Output Directory";
            this.buttonDataOutputDir.UseVisualStyleBackColor = true;
            this.buttonDataOutputDir.Click += new System.EventHandler(this.buttonDataOutputDir_Click);
            // 
            // ckbxOutputChromatograms
            // 
            this.ckbxOutputChromatograms.AutoSize = true;
            this.ckbxOutputChromatograms.Location = new System.Drawing.Point(125, 21);
            this.ckbxOutputChromatograms.Name = "ckbxOutputChromatograms";
            this.ckbxOutputChromatograms.Size = new System.Drawing.Size(130, 21);
            this.ckbxOutputChromatograms.TabIndex = 4;
            this.ckbxOutputChromatograms.Text = "Chromatograms";
            this.ckbxOutputChromatograms.UseVisualStyleBackColor = true;
            this.ckbxOutputChromatograms.CheckedChanged += new System.EventHandler(this.ckbxOutputChromatograms_CheckedChanged);
            // 
            // ckbxOutputMetrics
            // 
            this.ckbxOutputMetrics.AutoSize = true;
            this.ckbxOutputMetrics.Location = new System.Drawing.Point(6, 75);
            this.ckbxOutputMetrics.Name = "ckbxOutputMetrics";
            this.ckbxOutputMetrics.Size = new System.Drawing.Size(75, 21);
            this.ckbxOutputMetrics.TabIndex = 3;
            this.ckbxOutputMetrics.Text = "Metrics";
            this.ckbxOutputMetrics.UseVisualStyleBackColor = true;
            // 
            // ckbxOutputQuant
            // 
            this.ckbxOutputQuant.AutoSize = true;
            this.ckbxOutputQuant.Location = new System.Drawing.Point(6, 21);
            this.ckbxOutputQuant.Name = "ckbxOutputQuant";
            this.ckbxOutputQuant.Size = new System.Drawing.Size(173, 21);
            this.ckbxOutputQuant.TabIndex = 2;
            this.ckbxOutputQuant.Text = "Quantify Reporter Ions";
            this.ckbxOutputQuant.UseVisualStyleBackColor = true;
            this.ckbxOutputQuant.CheckedChanged += new System.EventHandler(this.ckbxOutputQuant_CheckedChanged);
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
            this.ckbxOutputParse.CheckedChanged += new System.EventHandler(this.ckbxOutputParse_CheckedChanged);
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
            this.ckbxOutputMGF.CheckedChanged += new System.EventHandler(this.ckbxOutputMGF_CheckedChanged);
            // 
            // groupBoxCommonOptions
            // 
            this.groupBoxCommonOptions.Controls.Add(this.comboBoxMinCharge);
            this.groupBoxCommonOptions.Controls.Add(this.labelMinCharge);
            this.groupBoxCommonOptions.Controls.Add(this.labelMaxCharge);
            this.groupBoxCommonOptions.Controls.Add(this.comboBoxMaxCharge);
            this.groupBoxCommonOptions.Controls.Add(this.checkBoxRefinePrecursor);
            this.groupBoxCommonOptions.Location = new System.Drawing.Point(342, 94);
            this.groupBoxCommonOptions.Name = "groupBoxCommonOptions";
            this.groupBoxCommonOptions.Size = new System.Drawing.Size(411, 145);
            this.groupBoxCommonOptions.TabIndex = 3;
            this.groupBoxCommonOptions.TabStop = false;
            this.groupBoxCommonOptions.Text = "Common Options";
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
            this.comboBoxMinCharge.Location = new System.Drawing.Point(158, 46);
            this.comboBoxMinCharge.Name = "comboBoxMinCharge";
            this.comboBoxMinCharge.Size = new System.Drawing.Size(106, 24);
            this.comboBoxMinCharge.TabIndex = 9;
            this.comboBoxMinCharge.SelectedIndexChanged += new System.EventHandler(this.comboBoxMinCharge_SelectedIndexChanged);
            this.comboBoxMinCharge.Enter += new System.EventHandler(this.comboBoxMinCharge_Enter);
            this.comboBoxMinCharge.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBoxMinCharge_KeyPress);
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
            // labelMaxCharge
            // 
            this.labelMaxCharge.AutoSize = true;
            this.labelMaxCharge.Location = new System.Drawing.Point(32, 79);
            this.labelMaxCharge.Name = "labelMaxCharge";
            this.labelMaxCharge.Size = new System.Drawing.Size(120, 17);
            this.labelMaxCharge.TabIndex = 2;
            this.labelMaxCharge.Text = "Maximum Charge:";
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
            this.comboBoxMaxCharge.Location = new System.Drawing.Point(158, 76);
            this.comboBoxMaxCharge.Name = "comboBoxMaxCharge";
            this.comboBoxMaxCharge.Size = new System.Drawing.Size(106, 24);
            this.comboBoxMaxCharge.TabIndex = 1;
            this.comboBoxMaxCharge.SelectedIndexChanged += new System.EventHandler(this.comboBoxMaxCharge_SelectedIndexChanged);
            this.comboBoxMaxCharge.Enter += new System.EventHandler(this.comboBoxMaxCharge_Enter);
            this.comboBoxMaxCharge.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBoxMaxCharge_KeyPress);
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
            // groupBoxQcOptions
            // 
            this.groupBoxQcOptions.Controls.Add(this.textBoxNumSpectra);
            this.groupBoxQcOptions.Controls.Add(this.labelNumSpectra);
            this.groupBoxQcOptions.Controls.Add(this.checkBoxAutoSearchIdentipy);
            this.groupBoxQcOptions.Controls.Add(this.textBoxIdentipyScript);
            this.groupBoxQcOptions.Controls.Add(this.textBoxPythonExe);
            this.groupBoxQcOptions.Controls.Add(this.textBoxXTandemDir);
            this.groupBoxQcOptions.Controls.Add(this.buttonXTandemDir);
            this.groupBoxQcOptions.Controls.Add(this.textBoxFastaFile);
            this.groupBoxQcOptions.Controls.Add(this.buttonIdentipyScript);
            this.groupBoxQcOptions.Controls.Add(this.buttonPythonExe);
            this.groupBoxQcOptions.Controls.Add(this.buttonFastaFile);
            this.groupBoxQcOptions.Controls.Add(this.radioButtonSearchIdentipy);
            this.groupBoxQcOptions.Controls.Add(this.radioButtonSearchXTandem);
            this.groupBoxQcOptions.Controls.Add(this.radioButtonSearchNone);
            this.groupBoxQcOptions.Controls.Add(this.label2);
            this.groupBoxQcOptions.Location = new System.Drawing.Point(3, 245);
            this.groupBoxQcOptions.Name = "groupBoxQcOptions";
            this.groupBoxQcOptions.Size = new System.Drawing.Size(750, 207);
            this.groupBoxQcOptions.TabIndex = 4;
            this.groupBoxQcOptions.TabStop = false;
            this.groupBoxQcOptions.Text = "QC Options";
            // 
            // textBoxNumSpectra
            // 
            this.textBoxNumSpectra.Enabled = false;
            this.textBoxNumSpectra.Location = new System.Drawing.Point(362, 169);
            this.textBoxNumSpectra.Name = "textBoxNumSpectra";
            this.textBoxNumSpectra.Size = new System.Drawing.Size(100, 22);
            this.textBoxNumSpectra.TabIndex = 20;
            this.textBoxNumSpectra.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumSpectra_KeyPress);
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
            // textBoxReporterNumberMissingFilter
            // 
            this.textBoxReporterNumberMissingFilter.Enabled = false;
            this.textBoxReporterNumberMissingFilter.Location = new System.Drawing.Point(342, 73);
            this.textBoxReporterNumberMissingFilter.Name = "textBoxReporterNumberMissingFilter";
            this.textBoxReporterNumberMissingFilter.Size = new System.Drawing.Size(72, 22);
            this.textBoxReporterNumberMissingFilter.TabIndex = 12;
            this.textBoxReporterNumberMissingFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
            // 
            // textBoxReporterIntensityFilter
            // 
            this.textBoxReporterIntensityFilter.Enabled = false;
            this.textBoxReporterIntensityFilter.Location = new System.Drawing.Point(342, 45);
            this.textBoxReporterIntensityFilter.Name = "textBoxReporterIntensityFilter";
            this.textBoxReporterIntensityFilter.Size = new System.Drawing.Size(72, 22);
            this.textBoxReporterIntensityFilter.TabIndex = 11;
            this.textBoxReporterIntensityFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(212, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "Median Intensity:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(212, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Missing Reporters:";
            // 
            // checkBoxReporterIonFiltering
            // 
            this.checkBoxReporterIonFiltering.AutoSize = true;
            this.checkBoxReporterIonFiltering.Enabled = false;
            this.checkBoxReporterIonFiltering.Location = new System.Drawing.Point(185, 21);
            this.checkBoxReporterIonFiltering.Name = "checkBoxReporterIonFiltering";
            this.checkBoxReporterIonFiltering.Size = new System.Drawing.Size(208, 21);
            this.checkBoxReporterIonFiltering.TabIndex = 0;
            this.checkBoxReporterIonFiltering.Text = "Reporter Ion-Based Filtering";
            this.checkBoxReporterIonFiltering.UseVisualStyleBackColor = true;
            // 
            // groupBoxChromatograms
            // 
            this.groupBoxChromatograms.Controls.Add(this.checkBox5);
            this.groupBoxChromatograms.Controls.Add(this.checkBox4);
            this.groupBoxChromatograms.Controls.Add(this.checkBox3);
            this.groupBoxChromatograms.Controls.Add(this.checkBox2);
            this.groupBoxChromatograms.Enabled = false;
            this.groupBoxChromatograms.Location = new System.Drawing.Point(3, 606);
            this.groupBoxChromatograms.Name = "groupBoxChromatograms";
            this.groupBoxChromatograms.Size = new System.Drawing.Size(183, 80);
            this.groupBoxChromatograms.TabIndex = 7;
            this.groupBoxChromatograms.TabStop = false;
            this.groupBoxChromatograms.Text = "Chromatogram Options";
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(70, 48);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(48, 21);
            this.checkBox5.TabIndex = 3;
            this.checkBox5.Text = "BP";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(70, 21);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(51, 21);
            this.checkBox4.TabIndex = 2;
            this.checkBox4.Text = "TIC";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(6, 48);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(58, 21);
            this.checkBox3.TabIndex = 1;
            this.checkBox3.Text = "MS2";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(6, 21);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(58, 21);
            this.checkBox2.TabIndex = 0;
            this.checkBox2.Text = "MS1";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.groupBoxMode);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxRawFiles);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxDataOutput);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxCommonOptions);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxQcOptions);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxQuantOpt);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxMgfOpts);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxChromatograms);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(773, 704);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(192, 606);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(561, 80);
            this.panel1.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(555, 68);
            this.button1.TabIndex = 0;
            this.button1.Text = "Go!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(212, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Apply to:";
            // 
            // checkBoxReporterFilterMatrix
            // 
            this.checkBoxReporterFilterMatrix.AutoSize = true;
            this.checkBoxReporterFilterMatrix.Enabled = false;
            this.checkBoxReporterFilterMatrix.Location = new System.Drawing.Point(281, 101);
            this.checkBoxReporterFilterMatrix.Name = "checkBoxReporterFilterMatrix";
            this.checkBoxReporterFilterMatrix.Size = new System.Drawing.Size(67, 21);
            this.checkBoxReporterFilterMatrix.TabIndex = 14;
            this.checkBoxReporterFilterMatrix.Text = "Matrix";
            this.checkBoxReporterFilterMatrix.UseVisualStyleBackColor = true;
            // 
            // checkBoxReporterFilterMGF
            // 
            this.checkBoxReporterFilterMGF.AutoSize = true;
            this.checkBoxReporterFilterMGF.Enabled = false;
            this.checkBoxReporterFilterMGF.Location = new System.Drawing.Point(354, 101);
            this.checkBoxReporterFilterMGF.Name = "checkBoxReporterFilterMGF";
            this.checkBoxReporterFilterMGF.Size = new System.Drawing.Size(60, 21);
            this.checkBoxReporterFilterMGF.TabIndex = 15;
            this.checkBoxReporterFilterMGF.Text = "MGF";
            this.checkBoxReporterFilterMGF.UseVisualStyleBackColor = true;
            // 
            // groupBoxQuantOpt
            // 
            this.groupBoxQuantOpt.Controls.Add(this.checkBoxReporterFilterMGF);
            this.groupBoxQuantOpt.Controls.Add(this.ckbxOutputQuant);
            this.groupBoxQuantOpt.Controls.Add(this.checkBoxReporterFilterMatrix);
            this.groupBoxQuantOpt.Controls.Add(this.comboBoxLabelingReagents);
            this.groupBoxQuantOpt.Controls.Add(this.label4);
            this.groupBoxQuantOpt.Controls.Add(this.checkBoxReporterIonFiltering);
            this.groupBoxQuantOpt.Controls.Add(this.textBoxReporterNumberMissingFilter);
            this.groupBoxQuantOpt.Controls.Add(this.label3);
            this.groupBoxQuantOpt.Controls.Add(this.textBoxReporterIntensityFilter);
            this.groupBoxQuantOpt.Controls.Add(this.label1);
            this.groupBoxQuantOpt.Enabled = false;
            this.groupBoxQuantOpt.Location = new System.Drawing.Point(3, 458);
            this.groupBoxQuantOpt.Name = "groupBoxQuantOpt";
            this.groupBoxQuantOpt.Size = new System.Drawing.Size(426, 142);
            this.groupBoxQuantOpt.TabIndex = 16;
            this.groupBoxQuantOpt.TabStop = false;
            this.groupBoxQuantOpt.Text = "Quantification Options";
            // 
            // comboBoxLabelingReagents
            // 
            this.comboBoxLabelingReagents.Enabled = false;
            this.comboBoxLabelingReagents.FormattingEnabled = true;
            this.comboBoxLabelingReagents.Items.AddRange(new object[] {
            "- select -",
            "TMT0",
            "TMT2",
            "TMT6",
            "TMT10",
            "TMT11",
            "iTRAQ4",
            "iTRAQ8",
            "Custom"});
            this.comboBoxLabelingReagents.Location = new System.Drawing.Point(28, 48);
            this.comboBoxLabelingReagents.Name = "comboBoxLabelingReagents";
            this.comboBoxLabelingReagents.Size = new System.Drawing.Size(121, 24);
            this.comboBoxLabelingReagents.TabIndex = 17;
            this.comboBoxLabelingReagents.SelectedIndexChanged += new System.EventHandler(this.comboBoxLabelingReagents_SelectedIndexChanged);
            this.comboBoxLabelingReagents.Enter += new System.EventHandler(this.comboBoxLabelingReagents_Enter);
            // 
            // groupBoxMgfOpts
            // 
            this.groupBoxMgfOpts.Location = new System.Drawing.Point(435, 458);
            this.groupBoxMgfOpts.Name = "groupBoxMgfOpts";
            this.groupBoxMgfOpts.Size = new System.Drawing.Size(318, 142);
            this.groupBoxMgfOpts.TabIndex = 17;
            this.groupBoxMgfOpts.TabStop = false;
            this.groupBoxMgfOpts.Text = "MGF Options";
            // 
            // RawToolsGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 721);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "RawToolsGUI";
            this.Text = "RawToolsGUI";
            this.groupBoxMode.ResumeLayout(false);
            this.groupBoxMode.PerformLayout();
            this.groupBoxRawFiles.ResumeLayout(false);
            this.groupBoxRawFiles.PerformLayout();
            this.groupBoxDataOutput.ResumeLayout(false);
            this.groupBoxDataOutput.PerformLayout();
            this.groupBoxCommonOptions.ResumeLayout(false);
            this.groupBoxCommonOptions.PerformLayout();
            this.groupBoxQcOptions.ResumeLayout(false);
            this.groupBoxQcOptions.PerformLayout();
            this.groupBoxChromatograms.ResumeLayout(false);
            this.groupBoxChromatograms.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBoxQuantOpt.ResumeLayout(false);
            this.groupBoxQuantOpt.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBoxDataOutput;
        private System.Windows.Forms.CheckBox ckbxOutputChromatograms;
        private System.Windows.Forms.CheckBox ckbxOutputMetrics;
        private System.Windows.Forms.CheckBox ckbxOutputQuant;
        private System.Windows.Forms.CheckBox ckbxOutputParse;
        private System.Windows.Forms.CheckBox ckbxOutputMGF;
        private System.Windows.Forms.GroupBox groupBoxCommonOptions;
        private System.Windows.Forms.Label labelMaxCharge;
        private System.Windows.Forms.ComboBox comboBoxMaxCharge;
        private System.Windows.Forms.CheckBox checkBoxRefinePrecursor;
        private System.Windows.Forms.GroupBox groupBoxQcOptions;
        private System.Windows.Forms.GroupBox groupBoxChromatograms;
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
        private System.Windows.Forms.TextBox textBoxDataOutputDir;
        private System.Windows.Forms.Button buttonDataOutputDir;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBoxReporterNumberMissingFilter;
        private System.Windows.Forms.TextBox textBoxReporterIntensityFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxReporterIonFiltering;
        private System.Windows.Forms.CheckBox checkBoxReporterFilterMGF;
        private System.Windows.Forms.CheckBox checkBoxReporterFilterMatrix;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBoxQuantOpt;
        private System.Windows.Forms.ComboBox comboBoxLabelingReagents;
        private System.Windows.Forms.GroupBox groupBoxMgfOpts;
    }
}

