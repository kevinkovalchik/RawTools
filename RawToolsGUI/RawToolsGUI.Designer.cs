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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RawToolsGUI));
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
            this.checkBoxFaims = new System.Windows.Forms.CheckBox();
            this.checkBoxDataOutputDirectory = new System.Windows.Forms.CheckBox();
            this.textBoxDataOutputDir = new System.Windows.Forms.TextBox();
            this.buttonDataOutputDir = new System.Windows.Forms.Button();
            this.ckbxOutputChromatograms = new System.Windows.Forms.CheckBox();
            this.ckbxOutputMetrics = new System.Windows.Forms.CheckBox();
            this.ckbxOutputParse = new System.Windows.Forms.CheckBox();
            this.ckbxOutputMGF = new System.Windows.Forms.CheckBox();
            this.ckbxOutputQuant = new System.Windows.Forms.CheckBox();
            this.groupBoxCommonOptions = new System.Windows.Forms.GroupBox();
            this.comboBoxMinCharge = new System.Windows.Forms.ComboBox();
            this.labelMinCharge = new System.Windows.Forms.Label();
            this.labelMaxCharge = new System.Windows.Forms.Label();
            this.comboBoxMaxCharge = new System.Windows.Forms.ComboBox();
            this.checkBoxRefinePrecursor = new System.Windows.Forms.CheckBox();
            this.groupBoxQcOptions = new System.Windows.Forms.GroupBox();
            this.labelPeptideMods = new System.Windows.Forms.Label();
            this.buttonPeptideMods = new System.Windows.Forms.Button();
            this.textBoxQcDataDirectory = new System.Windows.Forms.TextBox();
            this.buttonQcDataDirectory = new System.Windows.Forms.Button();
            this.textBoxNumSpectra = new System.Windows.Forms.TextBox();
            this.labelNumSpectra = new System.Windows.Forms.Label();
            this.textBoxXTandemDir = new System.Windows.Forms.TextBox();
            this.buttonXTandemDir = new System.Windows.Forms.Button();
            this.textBoxFastaFile = new System.Windows.Forms.TextBox();
            this.buttonFastaFile = new System.Windows.Forms.Button();
            this.radioButtonSearchXTandem = new System.Windows.Forms.RadioButton();
            this.radioButtonSearchNone = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxReporterNumberMissingFilter = new System.Windows.Forms.TextBox();
            this.textBoxReporterIntensityFilter = new System.Windows.Forms.TextBox();
            this.labelReporterIonIntensityFilter = new System.Windows.Forms.Label();
            this.labelReporterIonMissingFilter = new System.Windows.Forms.Label();
            this.checkBoxReporterIonFiltering = new System.Windows.Forms.CheckBox();
            this.groupBoxChromatograms = new System.Windows.Forms.GroupBox();
            this.checkBoxChroBP = new System.Windows.Forms.CheckBox();
            this.checkBoxChroTIC = new System.Windows.Forms.CheckBox();
            this.checkBoxChroMs2 = new System.Windows.Forms.CheckBox();
            this.checkBoxChroMs1 = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.comboBoxLabelingReagents = new System.Windows.Forms.ComboBox();
            this.textBoxMgfLowMass = new System.Windows.Forms.TextBox();
            this.checkBoxMgfLowMass = new System.Windows.Forms.CheckBox();
            this.buttonGo = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBoxQuantOpt = new System.Windows.Forms.GroupBox();
            this.checkBoxReporterFilterMGF = new System.Windows.Forms.CheckBox();
            this.checkBoxReporterFilterMatrix = new System.Windows.Forms.CheckBox();
            this.labelReporterIonFilteringApplyTo = new System.Windows.Forms.Label();
            this.groupBoxMgfOpts = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemNewParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemOpenParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSaveParameters = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxMode.SuspendLayout();
            this.groupBoxRawFiles.SuspendLayout();
            this.groupBoxDataOutput.SuspendLayout();
            this.groupBoxCommonOptions.SuspendLayout();
            this.groupBoxQcOptions.SuspendLayout();
            this.groupBoxChromatograms.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBoxQuantOpt.SuspendLayout();
            this.groupBoxMgfOpts.SuspendLayout();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMode
            // 
            this.groupBoxMode.Controls.Add(this.checkBoxModeQC);
            this.groupBoxMode.Controls.Add(this.checkBoxModeParse);
            this.groupBoxMode.Location = new System.Drawing.Point(2, 2);
            this.groupBoxMode.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxMode.Name = "groupBoxMode";
            this.groupBoxMode.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxMode.Size = new System.Drawing.Size(68, 69);
            this.groupBoxMode.TabIndex = 0;
            this.groupBoxMode.TabStop = false;
            this.groupBoxMode.Text = "Mode";
            // 
            // checkBoxModeQC
            // 
            this.checkBoxModeQC.AutoSize = true;
            this.checkBoxModeQC.Location = new System.Drawing.Point(4, 42);
            this.checkBoxModeQC.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxModeQC.Name = "checkBoxModeQC";
            this.checkBoxModeQC.Size = new System.Drawing.Size(41, 17);
            this.checkBoxModeQC.TabIndex = 1;
            this.checkBoxModeQC.Text = "QC";
            this.toolTip1.SetToolTip(this.checkBoxModeQC, "Enables quality control (QC) mode of RawTools.\r\nIf this is selected, you will onl" +
        "y be able to choose\r\na raw file directory to parse, not individual files.");
            this.checkBoxModeQC.UseVisualStyleBackColor = true;
            this.checkBoxModeQC.CheckedChanged += new System.EventHandler(this.checkBoxModeQC_CheckedChanged);
            // 
            // checkBoxModeParse
            // 
            this.checkBoxModeParse.AutoSize = true;
            this.checkBoxModeParse.Location = new System.Drawing.Point(4, 18);
            this.checkBoxModeParse.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxModeParse.Name = "checkBoxModeParse";
            this.checkBoxModeParse.Size = new System.Drawing.Size(53, 17);
            this.checkBoxModeParse.TabIndex = 0;
            this.checkBoxModeParse.Text = "Parse";
            this.toolTip1.SetToolTip(this.checkBoxModeParse, "Enables parsing of quant and meta data. Select this\r\nfor options under the \"Parse" +
        " Data Output\" section.");
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
            this.groupBoxRawFiles.Location = new System.Drawing.Point(74, 2);
            this.groupBoxRawFiles.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxRawFiles.Name = "groupBoxRawFiles";
            this.groupBoxRawFiles.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxRawFiles.Size = new System.Drawing.Size(490, 69);
            this.groupBoxRawFiles.TabIndex = 1;
            this.groupBoxRawFiles.TabStop = false;
            this.groupBoxRawFiles.Text = "Raw Files";
            // 
            // radioButtonSelectFiles
            // 
            this.radioButtonSelectFiles.AutoSize = true;
            this.radioButtonSelectFiles.Location = new System.Drawing.Point(4, 46);
            this.radioButtonSelectFiles.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonSelectFiles.Name = "radioButtonSelectFiles";
            this.radioButtonSelectFiles.Size = new System.Drawing.Size(14, 13);
            this.radioButtonSelectFiles.TabIndex = 2;
            this.toolTip1.SetToolTip(this.radioButtonSelectFiles, "Select one or more Thermo raw files to process.");
            this.radioButtonSelectFiles.UseVisualStyleBackColor = true;
            // 
            // radioButtonSelectDirectory
            // 
            this.radioButtonSelectDirectory.AutoSize = true;
            this.radioButtonSelectDirectory.Checked = true;
            this.radioButtonSelectDirectory.Location = new System.Drawing.Point(4, 20);
            this.radioButtonSelectDirectory.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonSelectDirectory.Name = "radioButtonSelectDirectory";
            this.radioButtonSelectDirectory.Size = new System.Drawing.Size(14, 13);
            this.radioButtonSelectDirectory.TabIndex = 2;
            this.radioButtonSelectDirectory.TabStop = true;
            this.toolTip1.SetToolTip(this.radioButtonSelectDirectory, "Choose a directory in which to process all Thermo raw files.");
            this.radioButtonSelectDirectory.UseVisualStyleBackColor = true;
            this.radioButtonSelectDirectory.CheckedChanged += new System.EventHandler(this.radioButtonSelectDirectory_CheckedChanged);
            // 
            // textBoxRawFiles
            // 
            this.textBoxRawFiles.Enabled = false;
            this.textBoxRawFiles.Location = new System.Drawing.Point(122, 41);
            this.textBoxRawFiles.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxRawFiles.Name = "textBoxRawFiles";
            this.textBoxRawFiles.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBoxRawFiles.Size = new System.Drawing.Size(357, 20);
            this.textBoxRawFiles.TabIndex = 2;
            this.toolTip1.SetToolTip(this.textBoxRawFiles, "Select one or more Thermo raw files to process.");
            // 
            // textBoxRawFileDirectory
            // 
            this.textBoxRawFileDirectory.Location = new System.Drawing.Point(122, 18);
            this.textBoxRawFileDirectory.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxRawFileDirectory.Name = "textBoxRawFileDirectory";
            this.textBoxRawFileDirectory.Size = new System.Drawing.Size(357, 20);
            this.textBoxRawFileDirectory.TabIndex = 3;
            this.toolTip1.SetToolTip(this.textBoxRawFileDirectory, "Choose a directory in which to process all Thermo raw files.");
            // 
            // buttonSelectFiles
            // 
            this.buttonSelectFiles.Enabled = false;
            this.buttonSelectFiles.Location = new System.Drawing.Point(22, 41);
            this.buttonSelectFiles.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSelectFiles.Name = "buttonSelectFiles";
            this.buttonSelectFiles.Size = new System.Drawing.Size(96, 19);
            this.buttonSelectFiles.TabIndex = 3;
            this.buttonSelectFiles.Text = "Select File(s)";
            this.toolTip1.SetToolTip(this.buttonSelectFiles, "Select one or more Thermo raw files to process.");
            this.buttonSelectFiles.UseVisualStyleBackColor = true;
            this.buttonSelectFiles.Click += new System.EventHandler(this.buttonSelectFiles_Click);
            // 
            // buttonSelectDirectory
            // 
            this.buttonSelectDirectory.Location = new System.Drawing.Point(22, 18);
            this.buttonSelectDirectory.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSelectDirectory.Name = "buttonSelectDirectory";
            this.buttonSelectDirectory.Size = new System.Drawing.Size(96, 19);
            this.buttonSelectDirectory.TabIndex = 2;
            this.buttonSelectDirectory.Text = "Select Directory";
            this.toolTip1.SetToolTip(this.buttonSelectDirectory, "Choose a directory in which to process all Thermo raw files.");
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
            this.groupBoxDataOutput.Controls.Add(this.checkBoxFaims);
            this.groupBoxDataOutput.Controls.Add(this.checkBoxDataOutputDirectory);
            this.groupBoxDataOutput.Controls.Add(this.textBoxDataOutputDir);
            this.groupBoxDataOutput.Controls.Add(this.buttonDataOutputDir);
            this.groupBoxDataOutput.Controls.Add(this.ckbxOutputChromatograms);
            this.groupBoxDataOutput.Controls.Add(this.ckbxOutputMetrics);
            this.groupBoxDataOutput.Controls.Add(this.ckbxOutputParse);
            this.groupBoxDataOutput.Controls.Add(this.ckbxOutputMGF);
            this.groupBoxDataOutput.Enabled = false;
            this.groupBoxDataOutput.Location = new System.Drawing.Point(2, 75);
            this.groupBoxDataOutput.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxDataOutput.Name = "groupBoxDataOutput";
            this.groupBoxDataOutput.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxDataOutput.Size = new System.Drawing.Size(250, 118);
            this.groupBoxDataOutput.TabIndex = 2;
            this.groupBoxDataOutput.TabStop = false;
            this.groupBoxDataOutput.Text = "Parse Data Output";
            // 
            // checkBoxFaims
            // 
            this.checkBoxFaims.AutoSize = true;
            this.checkBoxFaims.Location = new System.Drawing.Point(94, 60);
            this.checkBoxFaims.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxFaims.Name = "checkBoxFaims";
            this.checkBoxFaims.Size = new System.Drawing.Size(115, 17);
            this.checkBoxFaims.TabIndex = 13;
            this.checkBoxFaims.Text = "MGF by FAIMS CV";
            this.toolTip1.SetToolTip(this.checkBoxFaims, "Generate MGF output for individual CV values from FAIMS data.");
            this.checkBoxFaims.UseVisualStyleBackColor = true;
            this.checkBoxFaims.CheckedChanged += new System.EventHandler(this.ckbxOutputFaims_CheckedChanged);
            // 
            // checkBoxDataOutputDirectory
            // 
            this.checkBoxDataOutputDirectory.AutoSize = true;
            this.checkBoxDataOutputDirectory.Location = new System.Drawing.Point(94, 39);
            this.checkBoxDataOutputDirectory.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxDataOutputDirectory.Name = "checkBoxDataOutputDirectory";
            this.checkBoxDataOutputDirectory.Size = new System.Drawing.Size(154, 17);
            this.checkBoxDataOutputDirectory.TabIndex = 12;
            this.checkBoxDataOutputDirectory.Text = "Output to different directory";
            this.toolTip1.SetToolTip(this.checkBoxDataOutputDirectory, "Select if you want the output files to go in a directory\r\ndifferent from the raw " +
        "file location. You must then use\r\nthe \"Output Directory\" button or text box to s" +
        "pecify the\r\ndirectory.");
            this.checkBoxDataOutputDirectory.UseVisualStyleBackColor = true;
            this.checkBoxDataOutputDirectory.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // textBoxDataOutputDir
            // 
            this.textBoxDataOutputDir.Enabled = false;
            this.textBoxDataOutputDir.Location = new System.Drawing.Point(112, 83);
            this.textBoxDataOutputDir.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxDataOutputDir.Name = "textBoxDataOutputDir";
            this.textBoxDataOutputDir.Size = new System.Drawing.Size(134, 20);
            this.textBoxDataOutputDir.TabIndex = 11;
            this.toolTip1.SetToolTip(this.textBoxDataOutputDir, "Choose a directory in which to save all output files.");
            // 
            // buttonDataOutputDir
            // 
            this.buttonDataOutputDir.Enabled = false;
            this.buttonDataOutputDir.Location = new System.Drawing.Point(4, 83);
            this.buttonDataOutputDir.Margin = new System.Windows.Forms.Padding(2);
            this.buttonDataOutputDir.Name = "buttonDataOutputDir";
            this.buttonDataOutputDir.Size = new System.Drawing.Size(104, 19);
            this.buttonDataOutputDir.TabIndex = 10;
            this.buttonDataOutputDir.Text = "Output Directory";
            this.toolTip1.SetToolTip(this.buttonDataOutputDir, "Choose a directory in which to save all output files.\r\nIf no directory is selecte" +
        "d, the files will be written\r\nto the directory which contains the respective raw" +
        "\r\nfile.");
            this.buttonDataOutputDir.UseVisualStyleBackColor = true;
            this.buttonDataOutputDir.Click += new System.EventHandler(this.buttonDataOutputDir_Click);
            // 
            // ckbxOutputChromatograms
            // 
            this.ckbxOutputChromatograms.AutoSize = true;
            this.ckbxOutputChromatograms.Location = new System.Drawing.Point(94, 17);
            this.ckbxOutputChromatograms.Margin = new System.Windows.Forms.Padding(2);
            this.ckbxOutputChromatograms.Name = "ckbxOutputChromatograms";
            this.ckbxOutputChromatograms.Size = new System.Drawing.Size(99, 17);
            this.ckbxOutputChromatograms.TabIndex = 4;
            this.ckbxOutputChromatograms.Text = "Chromatograms";
            this.toolTip1.SetToolTip(this.ckbxOutputChromatograms, "Generate chromatograms. You must choose further\r\noptions under \"Chromatogram Opti" +
        "ons\".");
            this.ckbxOutputChromatograms.UseVisualStyleBackColor = true;
            this.ckbxOutputChromatograms.CheckedChanged += new System.EventHandler(this.ckbxOutputChromatograms_CheckedChanged);
            // 
            // ckbxOutputMetrics
            // 
            this.ckbxOutputMetrics.AutoSize = true;
            this.ckbxOutputMetrics.Location = new System.Drawing.Point(4, 61);
            this.ckbxOutputMetrics.Margin = new System.Windows.Forms.Padding(2);
            this.ckbxOutputMetrics.Name = "ckbxOutputMetrics";
            this.ckbxOutputMetrics.Size = new System.Drawing.Size(60, 17);
            this.ckbxOutputMetrics.TabIndex = 3;
            this.ckbxOutputMetrics.Text = "Metrics";
            this.toolTip1.SetToolTip(this.ckbxOutputMetrics, "Calculate descriptive metrics from the scan meta data\r\nand write them to a text f" +
        "ile.");
            this.ckbxOutputMetrics.UseVisualStyleBackColor = true;
            // 
            // ckbxOutputParse
            // 
            this.ckbxOutputParse.AutoSize = true;
            this.ckbxOutputParse.Location = new System.Drawing.Point(4, 39);
            this.ckbxOutputParse.Margin = new System.Windows.Forms.Padding(2);
            this.ckbxOutputParse.Name = "ckbxOutputParse";
            this.ckbxOutputParse.Size = new System.Drawing.Size(84, 17);
            this.ckbxOutputParse.TabIndex = 1;
            this.ckbxOutputParse.Text = "Parse Matrix";
            this.toolTip1.SetToolTip(this.ckbxOutputParse, "Create a matrix containing scan meta data and,\r\noptionally, quantification data f" +
        "rom the\r\nraw file.");
            this.ckbxOutputParse.UseVisualStyleBackColor = true;
            this.ckbxOutputParse.CheckedChanged += new System.EventHandler(this.ckbxOutputParse_CheckedChanged);
            // 
            // ckbxOutputMGF
            // 
            this.ckbxOutputMGF.AutoSize = true;
            this.ckbxOutputMGF.Location = new System.Drawing.Point(4, 17);
            this.ckbxOutputMGF.Margin = new System.Windows.Forms.Padding(2);
            this.ckbxOutputMGF.Name = "ckbxOutputMGF";
            this.ckbxOutputMGF.Size = new System.Drawing.Size(49, 17);
            this.ckbxOutputMGF.TabIndex = 0;
            this.ckbxOutputMGF.Text = "MGF";
            this.toolTip1.SetToolTip(this.ckbxOutputMGF, "Create an MGF file of all MS2 scans in the raw file.");
            this.ckbxOutputMGF.UseVisualStyleBackColor = true;
            this.ckbxOutputMGF.CheckedChanged += new System.EventHandler(this.ckbxOutputMGF_CheckedChanged);
            // 
            // ckbxOutputQuant
            // 
            this.ckbxOutputQuant.AutoSize = true;
            this.ckbxOutputQuant.Location = new System.Drawing.Point(4, 17);
            this.ckbxOutputQuant.Margin = new System.Windows.Forms.Padding(2);
            this.ckbxOutputQuant.Name = "ckbxOutputQuant";
            this.ckbxOutputQuant.Size = new System.Drawing.Size(132, 17);
            this.ckbxOutputQuant.TabIndex = 2;
            this.ckbxOutputQuant.Text = "Quantify Reporter Ions";
            this.toolTip1.SetToolTip(this.ckbxOutputQuant, "Select to quantify reporter ions and include in the\r\nparse matrix output.");
            this.ckbxOutputQuant.UseVisualStyleBackColor = true;
            this.ckbxOutputQuant.CheckedChanged += new System.EventHandler(this.ckbxOutputQuant_CheckedChanged);
            // 
            // groupBoxCommonOptions
            // 
            this.groupBoxCommonOptions.Controls.Add(this.comboBoxMinCharge);
            this.groupBoxCommonOptions.Controls.Add(this.labelMinCharge);
            this.groupBoxCommonOptions.Controls.Add(this.labelMaxCharge);
            this.groupBoxCommonOptions.Controls.Add(this.comboBoxMaxCharge);
            this.groupBoxCommonOptions.Controls.Add(this.checkBoxRefinePrecursor);
            this.groupBoxCommonOptions.Enabled = false;
            this.groupBoxCommonOptions.Location = new System.Drawing.Point(256, 75);
            this.groupBoxCommonOptions.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxCommonOptions.Name = "groupBoxCommonOptions";
            this.groupBoxCommonOptions.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxCommonOptions.Size = new System.Drawing.Size(308, 118);
            this.groupBoxCommonOptions.TabIndex = 3;
            this.groupBoxCommonOptions.TabStop = false;
            this.groupBoxCommonOptions.Text = "Common Options";
            // 
            // comboBoxMinCharge
            // 
            this.comboBoxMinCharge.Enabled = false;
            this.comboBoxMinCharge.FormattingEnabled = true;
            this.comboBoxMinCharge.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.comboBoxMinCharge.Location = new System.Drawing.Point(118, 37);
            this.comboBoxMinCharge.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxMinCharge.Name = "comboBoxMinCharge";
            this.comboBoxMinCharge.Size = new System.Drawing.Size(80, 21);
            this.comboBoxMinCharge.TabIndex = 9;
            this.toolTip1.SetToolTip(this.comboBoxMinCharge, resources.GetString("comboBoxMinCharge.ToolTip"));
            this.comboBoxMinCharge.SelectedIndexChanged += new System.EventHandler(this.comboBoxMinCharge_SelectedIndexChanged);
            this.comboBoxMinCharge.Enter += new System.EventHandler(this.comboBoxMinCharge_Enter);
            this.comboBoxMinCharge.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBoxMinCharge_KeyPress);
            // 
            // labelMinCharge
            // 
            this.labelMinCharge.AutoSize = true;
            this.labelMinCharge.Enabled = false;
            this.labelMinCharge.Location = new System.Drawing.Point(24, 39);
            this.labelMinCharge.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMinCharge.Name = "labelMinCharge";
            this.labelMinCharge.Size = new System.Drawing.Size(88, 13);
            this.labelMinCharge.TabIndex = 3;
            this.labelMinCharge.Text = "Minimum Charge:";
            this.toolTip1.SetToolTip(this.labelMinCharge, resources.GetString("labelMinCharge.ToolTip"));
            // 
            // labelMaxCharge
            // 
            this.labelMaxCharge.AutoSize = true;
            this.labelMaxCharge.Enabled = false;
            this.labelMaxCharge.Location = new System.Drawing.Point(24, 64);
            this.labelMaxCharge.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMaxCharge.Name = "labelMaxCharge";
            this.labelMaxCharge.Size = new System.Drawing.Size(91, 13);
            this.labelMaxCharge.TabIndex = 2;
            this.labelMaxCharge.Text = "Maximum Charge:";
            this.toolTip1.SetToolTip(this.labelMaxCharge, resources.GetString("labelMaxCharge.ToolTip"));
            // 
            // comboBoxMaxCharge
            // 
            this.comboBoxMaxCharge.Enabled = false;
            this.comboBoxMaxCharge.FormattingEnabled = true;
            this.comboBoxMaxCharge.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.comboBoxMaxCharge.Location = new System.Drawing.Point(118, 62);
            this.comboBoxMaxCharge.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxMaxCharge.Name = "comboBoxMaxCharge";
            this.comboBoxMaxCharge.Size = new System.Drawing.Size(80, 21);
            this.comboBoxMaxCharge.TabIndex = 1;
            this.toolTip1.SetToolTip(this.comboBoxMaxCharge, resources.GetString("comboBoxMaxCharge.ToolTip"));
            this.comboBoxMaxCharge.SelectedIndexChanged += new System.EventHandler(this.comboBoxMaxCharge_SelectedIndexChanged);
            this.comboBoxMaxCharge.Enter += new System.EventHandler(this.comboBoxMaxCharge_Enter);
            this.comboBoxMaxCharge.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBoxMaxCharge_KeyPress);
            // 
            // checkBoxRefinePrecursor
            // 
            this.checkBoxRefinePrecursor.AutoSize = true;
            this.checkBoxRefinePrecursor.Location = new System.Drawing.Point(4, 17);
            this.checkBoxRefinePrecursor.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxRefinePrecursor.Name = "checkBoxRefinePrecursor";
            this.checkBoxRefinePrecursor.Size = new System.Drawing.Size(191, 17);
            this.checkBoxRefinePrecursor.TabIndex = 0;
            this.checkBoxRefinePrecursor.Text = "Refine Precursor Mass and Charge";
            this.toolTip1.SetToolTip(this.checkBoxRefinePrecursor, resources.GetString("checkBoxRefinePrecursor.ToolTip"));
            this.checkBoxRefinePrecursor.UseVisualStyleBackColor = true;
            this.checkBoxRefinePrecursor.CheckedChanged += new System.EventHandler(this.checkBoxRefinePrecursor_CheckedChanged);
            // 
            // groupBoxQcOptions
            // 
            this.groupBoxQcOptions.Controls.Add(this.labelPeptideMods);
            this.groupBoxQcOptions.Controls.Add(this.buttonPeptideMods);
            this.groupBoxQcOptions.Controls.Add(this.textBoxQcDataDirectory);
            this.groupBoxQcOptions.Controls.Add(this.buttonQcDataDirectory);
            this.groupBoxQcOptions.Controls.Add(this.textBoxNumSpectra);
            this.groupBoxQcOptions.Controls.Add(this.labelNumSpectra);
            this.groupBoxQcOptions.Controls.Add(this.textBoxXTandemDir);
            this.groupBoxQcOptions.Controls.Add(this.buttonXTandemDir);
            this.groupBoxQcOptions.Controls.Add(this.textBoxFastaFile);
            this.groupBoxQcOptions.Controls.Add(this.buttonFastaFile);
            this.groupBoxQcOptions.Controls.Add(this.radioButtonSearchXTandem);
            this.groupBoxQcOptions.Controls.Add(this.radioButtonSearchNone);
            this.groupBoxQcOptions.Controls.Add(this.label2);
            this.groupBoxQcOptions.Enabled = false;
            this.groupBoxQcOptions.Location = new System.Drawing.Point(2, 197);
            this.groupBoxQcOptions.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxQcOptions.Name = "groupBoxQcOptions";
            this.groupBoxQcOptions.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxQcOptions.Size = new System.Drawing.Size(562, 158);
            this.groupBoxQcOptions.TabIndex = 4;
            this.groupBoxQcOptions.TabStop = false;
            this.groupBoxQcOptions.Text = "QC Options";
            // 
            // labelPeptideMods
            // 
            this.labelPeptideMods.AutoSize = true;
            this.labelPeptideMods.Enabled = false;
            this.labelPeptideMods.Location = new System.Drawing.Point(248, 124);
            this.labelPeptideMods.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPeptideMods.Name = "labelPeptideMods";
            this.labelPeptideMods.Size = new System.Drawing.Size(111, 13);
            this.labelPeptideMods.TabIndex = 23;
            this.labelPeptideMods.Text = "Peptide Modifications:";
            // 
            // buttonPeptideMods
            // 
            this.buttonPeptideMods.Enabled = false;
            this.buttonPeptideMods.Location = new System.Drawing.Point(362, 121);
            this.buttonPeptideMods.Margin = new System.Windows.Forms.Padding(2);
            this.buttonPeptideMods.Name = "buttonPeptideMods";
            this.buttonPeptideMods.Size = new System.Drawing.Size(185, 19);
            this.buttonPeptideMods.TabIndex = 9;
            this.buttonPeptideMods.Text = "Add/Review Peptide Modifications";
            this.toolTip1.SetToolTip(this.buttonPeptideMods, "Choose peptide modifications for the database search.");
            this.buttonPeptideMods.UseVisualStyleBackColor = true;
            this.buttonPeptideMods.Click += new System.EventHandler(this.buttonPeptideMods_Click);
            // 
            // textBoxQcDataDirectory
            // 
            this.textBoxQcDataDirectory.Location = new System.Drawing.Point(122, 17);
            this.textBoxQcDataDirectory.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxQcDataDirectory.Name = "textBoxQcDataDirectory";
            this.textBoxQcDataDirectory.Size = new System.Drawing.Size(426, 20);
            this.textBoxQcDataDirectory.TabIndex = 22;
            this.toolTip1.SetToolTip(this.textBoxQcDataDirectory, "Select a directory in which to store the QC data.");
            // 
            // buttonQcDataDirectory
            // 
            this.buttonQcDataDirectory.Location = new System.Drawing.Point(4, 17);
            this.buttonQcDataDirectory.Margin = new System.Windows.Forms.Padding(2);
            this.buttonQcDataDirectory.Name = "buttonQcDataDirectory";
            this.buttonQcDataDirectory.Size = new System.Drawing.Size(113, 19);
            this.buttonQcDataDirectory.TabIndex = 21;
            this.buttonQcDataDirectory.Text = "QC Data Directory";
            this.toolTip1.SetToolTip(this.buttonQcDataDirectory, "Select a directory in which to store the QC data.");
            this.buttonQcDataDirectory.UseVisualStyleBackColor = true;
            this.buttonQcDataDirectory.Click += new System.EventHandler(this.buttonQcDataDirectory_Click);
            // 
            // textBoxNumSpectra
            // 
            this.textBoxNumSpectra.Enabled = false;
            this.textBoxNumSpectra.Location = new System.Drawing.Point(158, 121);
            this.textBoxNumSpectra.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxNumSpectra.Name = "textBoxNumSpectra";
            this.textBoxNumSpectra.Size = new System.Drawing.Size(76, 20);
            this.textBoxNumSpectra.TabIndex = 20;
            this.textBoxNumSpectra.Text = "10000";
            this.toolTip1.SetToolTip(this.textBoxNumSpectra, "Choose the number of spectra to use for the database\r\nsearch. These are chosen fr" +
        "om all MS2 spectra in the\r\nraw file using a uniformly distributed random selecti" +
        "on\r\nprocess.");
            this.textBoxNumSpectra.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumSpectra_KeyPress);
            // 
            // labelNumSpectra
            // 
            this.labelNumSpectra.AutoSize = true;
            this.labelNumSpectra.Enabled = false;
            this.labelNumSpectra.Location = new System.Drawing.Point(7, 124);
            this.labelNumSpectra.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelNumSpectra.Name = "labelNumSpectra";
            this.labelNumSpectra.Size = new System.Drawing.Size(148, 13);
            this.labelNumSpectra.TabIndex = 19;
            this.labelNumSpectra.Text = "Number of Spectra to Search:";
            // 
            // textBoxXTandemDir
            // 
            this.textBoxXTandemDir.Enabled = false;
            this.textBoxXTandemDir.Location = new System.Drawing.Point(122, 62);
            this.textBoxXTandemDir.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxXTandemDir.Name = "textBoxXTandemDir";
            this.textBoxXTandemDir.Size = new System.Drawing.Size(429, 20);
            this.textBoxXTandemDir.TabIndex = 16;
            this.toolTip1.SetToolTip(this.textBoxXTandemDir, "Select the directory which contains the X! Tandem\r\nexecutable.");
            // 
            // buttonXTandemDir
            // 
            this.buttonXTandemDir.Enabled = false;
            this.buttonXTandemDir.Location = new System.Drawing.Point(4, 61);
            this.buttonXTandemDir.Margin = new System.Windows.Forms.Padding(2);
            this.buttonXTandemDir.Name = "buttonXTandemDir";
            this.buttonXTandemDir.Size = new System.Drawing.Size(113, 19);
            this.buttonXTandemDir.TabIndex = 15;
            this.buttonXTandemDir.Text = "X! Tandem Directory";
            this.toolTip1.SetToolTip(this.buttonXTandemDir, "Select the directory which contains the X! Tandem\r\nexecutable.");
            this.buttonXTandemDir.UseVisualStyleBackColor = true;
            this.buttonXTandemDir.Click += new System.EventHandler(this.buttonXTandemDir_Click);
            // 
            // textBoxFastaFile
            // 
            this.textBoxFastaFile.Enabled = false;
            this.textBoxFastaFile.Location = new System.Drawing.Point(122, 84);
            this.textBoxFastaFile.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxFastaFile.Name = "textBoxFastaFile";
            this.textBoxFastaFile.Size = new System.Drawing.Size(429, 20);
            this.textBoxFastaFile.TabIndex = 8;
            this.toolTip1.SetToolTip(this.textBoxFastaFile, "Select the FASTA file you wish to use for the database\r\nsearch.");
            // 
            // buttonFastaFile
            // 
            this.buttonFastaFile.Enabled = false;
            this.buttonFastaFile.Location = new System.Drawing.Point(4, 84);
            this.buttonFastaFile.Margin = new System.Windows.Forms.Padding(2);
            this.buttonFastaFile.Name = "buttonFastaFile";
            this.buttonFastaFile.Size = new System.Drawing.Size(113, 19);
            this.buttonFastaFile.TabIndex = 12;
            this.buttonFastaFile.Text = "Select Fasta File";
            this.toolTip1.SetToolTip(this.buttonFastaFile, "Select the FASTA file you wish to use for the database\r\nsearch.");
            this.buttonFastaFile.UseVisualStyleBackColor = true;
            this.buttonFastaFile.Click += new System.EventHandler(this.buttonFastaFile_Click);
            // 
            // radioButtonSearchXTandem
            // 
            this.radioButtonSearchXTandem.AutoSize = true;
            this.radioButtonSearchXTandem.Location = new System.Drawing.Point(142, 43);
            this.radioButtonSearchXTandem.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonSearchXTandem.Name = "radioButtonSearchXTandem";
            this.radioButtonSearchXTandem.Size = new System.Drawing.Size(77, 17);
            this.radioButtonSearchXTandem.TabIndex = 10;
            this.radioButtonSearchXTandem.Text = "X! Tandem";
            this.radioButtonSearchXTandem.UseVisualStyleBackColor = true;
            this.radioButtonSearchXTandem.CheckedChanged += new System.EventHandler(this.radioButtonSearchXTandem_CheckedChanged);
            // 
            // radioButtonSearchNone
            // 
            this.radioButtonSearchNone.AutoSize = true;
            this.radioButtonSearchNone.Checked = true;
            this.radioButtonSearchNone.Location = new System.Drawing.Point(90, 43);
            this.radioButtonSearchNone.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonSearchNone.Name = "radioButtonSearchNone";
            this.radioButtonSearchNone.Size = new System.Drawing.Size(51, 17);
            this.radioButtonSearchNone.TabIndex = 9;
            this.radioButtonSearchNone.TabStop = true;
            this.radioButtonSearchNone.Text = "None";
            this.radioButtonSearchNone.UseVisualStyleBackColor = true;
            this.radioButtonSearchNone.CheckedChanged += new System.EventHandler(this.radioButtonSearchNone_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Search Engine:";
            // 
            // textBoxReporterNumberMissingFilter
            // 
            this.textBoxReporterNumberMissingFilter.Enabled = false;
            this.textBoxReporterNumberMissingFilter.Location = new System.Drawing.Point(256, 59);
            this.textBoxReporterNumberMissingFilter.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxReporterNumberMissingFilter.Name = "textBoxReporterNumberMissingFilter";
            this.textBoxReporterNumberMissingFilter.Size = new System.Drawing.Size(55, 20);
            this.textBoxReporterNumberMissingFilter.TabIndex = 12;
            this.textBoxReporterNumberMissingFilter.Text = "0";
            this.textBoxReporterNumberMissingFilter.Visible = false;
            this.textBoxReporterNumberMissingFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxReporterNumberMissingFilter_KeyPress);
            // 
            // textBoxReporterIntensityFilter
            // 
            this.textBoxReporterIntensityFilter.Enabled = false;
            this.textBoxReporterIntensityFilter.Location = new System.Drawing.Point(256, 37);
            this.textBoxReporterIntensityFilter.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxReporterIntensityFilter.Name = "textBoxReporterIntensityFilter";
            this.textBoxReporterIntensityFilter.Size = new System.Drawing.Size(55, 20);
            this.textBoxReporterIntensityFilter.TabIndex = 11;
            this.textBoxReporterIntensityFilter.Text = "0";
            this.textBoxReporterIntensityFilter.Visible = false;
            this.textBoxReporterIntensityFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxReporterIntensityFilter_KeyPress);
            // 
            // labelReporterIonIntensityFilter
            // 
            this.labelReporterIonIntensityFilter.AutoSize = true;
            this.labelReporterIonIntensityFilter.Enabled = false;
            this.labelReporterIonIntensityFilter.Location = new System.Drawing.Point(159, 37);
            this.labelReporterIonIntensityFilter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelReporterIonIntensityFilter.Name = "labelReporterIonIntensityFilter";
            this.labelReporterIonIntensityFilter.Size = new System.Drawing.Size(87, 13);
            this.labelReporterIonIntensityFilter.TabIndex = 9;
            this.labelReporterIonIntensityFilter.Text = "Median Intensity:";
            this.labelReporterIonIntensityFilter.Visible = false;
            // 
            // labelReporterIonMissingFilter
            // 
            this.labelReporterIonMissingFilter.AutoSize = true;
            this.labelReporterIonMissingFilter.Enabled = false;
            this.labelReporterIonMissingFilter.Location = new System.Drawing.Point(159, 62);
            this.labelReporterIonMissingFilter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelReporterIonMissingFilter.Name = "labelReporterIonMissingFilter";
            this.labelReporterIonMissingFilter.Size = new System.Drawing.Size(94, 13);
            this.labelReporterIonMissingFilter.TabIndex = 10;
            this.labelReporterIonMissingFilter.Text = "Missing Reporters:";
            this.labelReporterIonMissingFilter.Visible = false;
            // 
            // checkBoxReporterIonFiltering
            // 
            this.checkBoxReporterIonFiltering.AutoSize = true;
            this.checkBoxReporterIonFiltering.Enabled = false;
            this.checkBoxReporterIonFiltering.Location = new System.Drawing.Point(139, 17);
            this.checkBoxReporterIonFiltering.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxReporterIonFiltering.Name = "checkBoxReporterIonFiltering";
            this.checkBoxReporterIonFiltering.Size = new System.Drawing.Size(157, 17);
            this.checkBoxReporterIonFiltering.TabIndex = 0;
            this.checkBoxReporterIonFiltering.Text = "Reporter Ion-Based Filtering";
            this.checkBoxReporterIonFiltering.UseVisualStyleBackColor = true;
            this.checkBoxReporterIonFiltering.Visible = false;
            this.checkBoxReporterIonFiltering.CheckedChanged += new System.EventHandler(this.checkBoxReporterIonFiltering_CheckedChanged);
            // 
            // groupBoxChromatograms
            // 
            this.groupBoxChromatograms.Controls.Add(this.checkBoxChroBP);
            this.groupBoxChromatograms.Controls.Add(this.checkBoxChroTIC);
            this.groupBoxChromatograms.Controls.Add(this.checkBoxChroMs2);
            this.groupBoxChromatograms.Controls.Add(this.checkBoxChroMs1);
            this.groupBoxChromatograms.Enabled = false;
            this.groupBoxChromatograms.Location = new System.Drawing.Point(2, 439);
            this.groupBoxChromatograms.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxChromatograms.Name = "groupBoxChromatograms";
            this.groupBoxChromatograms.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxChromatograms.Size = new System.Drawing.Size(137, 65);
            this.groupBoxChromatograms.TabIndex = 7;
            this.groupBoxChromatograms.TabStop = false;
            this.groupBoxChromatograms.Text = "Chromatogram Options";
            // 
            // checkBoxChroBP
            // 
            this.checkBoxChroBP.AutoSize = true;
            this.checkBoxChroBP.Location = new System.Drawing.Point(52, 39);
            this.checkBoxChroBP.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxChroBP.Name = "checkBoxChroBP";
            this.checkBoxChroBP.Size = new System.Drawing.Size(40, 17);
            this.checkBoxChroBP.TabIndex = 3;
            this.checkBoxChroBP.Text = "BP";
            this.toolTip1.SetToolTip(this.checkBoxChroBP, "Write a base peak chromatogram to disk. You must also\r\none or more of MS1 and MS2" +
        ".");
            this.checkBoxChroBP.UseVisualStyleBackColor = true;
            // 
            // checkBoxChroTIC
            // 
            this.checkBoxChroTIC.AutoSize = true;
            this.checkBoxChroTIC.Location = new System.Drawing.Point(52, 17);
            this.checkBoxChroTIC.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxChroTIC.Name = "checkBoxChroTIC";
            this.checkBoxChroTIC.Size = new System.Drawing.Size(43, 17);
            this.checkBoxChroTIC.TabIndex = 2;
            this.checkBoxChroTIC.Text = "TIC";
            this.toolTip1.SetToolTip(this.checkBoxChroTIC, "Write a total ion current chromatogram to disk. You must also\r\none or more of MS1" +
        " and MS2.");
            this.checkBoxChroTIC.UseVisualStyleBackColor = true;
            // 
            // checkBoxChroMs2
            // 
            this.checkBoxChroMs2.AutoSize = true;
            this.checkBoxChroMs2.Location = new System.Drawing.Point(4, 39);
            this.checkBoxChroMs2.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxChroMs2.Name = "checkBoxChroMs2";
            this.checkBoxChroMs2.Size = new System.Drawing.Size(48, 17);
            this.checkBoxChroMs2.TabIndex = 1;
            this.checkBoxChroMs2.Text = "MS2";
            this.toolTip1.SetToolTip(this.checkBoxChroMs2, "Write a MS2 chromatogram to disk. You must also\r\none or more of TIC and BP.");
            this.checkBoxChroMs2.UseVisualStyleBackColor = true;
            // 
            // checkBoxChroMs1
            // 
            this.checkBoxChroMs1.AutoSize = true;
            this.checkBoxChroMs1.Location = new System.Drawing.Point(4, 17);
            this.checkBoxChroMs1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxChroMs1.Name = "checkBoxChroMs1";
            this.checkBoxChroMs1.Size = new System.Drawing.Size(48, 17);
            this.checkBoxChroMs1.TabIndex = 0;
            this.checkBoxChroMs1.Text = "MS1";
            this.toolTip1.SetToolTip(this.checkBoxChroMs1, "Write a MS1 chromatogram to disk. You must also\r\none or more of TIC and BP.");
            this.checkBoxChroMs1.UseVisualStyleBackColor = true;
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
            "TMT16",
            "TMT18",
            "iTRAQ4",
            "iTRAQ8",
            "Custom"});
            this.comboBoxLabelingReagents.Location = new System.Drawing.Point(21, 39);
            this.comboBoxLabelingReagents.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxLabelingReagents.Name = "comboBoxLabelingReagents";
            this.comboBoxLabelingReagents.Size = new System.Drawing.Size(92, 21);
            this.comboBoxLabelingReagents.TabIndex = 17;
            this.toolTip1.SetToolTip(this.comboBoxLabelingReagents, "Select a labeling reagent from the drop down list.");
            this.comboBoxLabelingReagents.SelectedIndexChanged += new System.EventHandler(this.comboBoxLabelingReagents_SelectedIndexChanged);
            this.comboBoxLabelingReagents.Enter += new System.EventHandler(this.comboBoxLabelingReagents_Enter);
            // 
            // textBoxMgfLowMass
            // 
            this.textBoxMgfLowMass.Enabled = false;
            this.textBoxMgfLowMass.Location = new System.Drawing.Point(139, 15);
            this.textBoxMgfLowMass.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxMgfLowMass.Name = "textBoxMgfLowMass";
            this.textBoxMgfLowMass.Size = new System.Drawing.Size(76, 20);
            this.textBoxMgfLowMass.TabIndex = 2;
            this.textBoxMgfLowMass.Text = "0";
            this.toolTip1.SetToolTip(this.textBoxMgfLowMass, resources.GetString("textBoxMgfLowMass.ToolTip"));
            this.textBoxMgfLowMass.TextChanged += new System.EventHandler(this.textBoxMgfLowMass_TextChanged);
            this.textBoxMgfLowMass.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMgfLowMass_KeyPress);
            // 
            // checkBoxMgfLowMass
            // 
            this.checkBoxMgfLowMass.AutoSize = true;
            this.checkBoxMgfLowMass.Location = new System.Drawing.Point(4, 17);
            this.checkBoxMgfLowMass.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxMgfLowMass.Name = "checkBoxMgfLowMass";
            this.checkBoxMgfLowMass.Size = new System.Drawing.Size(135, 17);
            this.checkBoxMgfLowMass.TabIndex = 0;
            this.checkBoxMgfLowMass.Text = "Low Mass Cutoff (m/z):";
            this.toolTip1.SetToolTip(this.checkBoxMgfLowMass, resources.GetString("checkBoxMgfLowMass.ToolTip"));
            this.checkBoxMgfLowMass.UseVisualStyleBackColor = true;
            this.checkBoxMgfLowMass.CheckedChanged += new System.EventHandler(this.checkBoxMgfLowMass_CheckedChanged);
            // 
            // buttonGo
            // 
            this.buttonGo.Location = new System.Drawing.Point(2, 7);
            this.buttonGo.Margin = new System.Windows.Forms.Padding(2);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(416, 55);
            this.buttonGo.TabIndex = 0;
            this.buttonGo.Text = "Go!";
            this.toolTip1.SetToolTip(this.buttonGo, "Start processing!");
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this.groupBoxMode);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxRawFiles);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxDataOutput);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxCommonOptions);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxQcOptions);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxQuantOpt);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxMgfOpts);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxChromatograms);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(8, 29);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel1.MaximumSize = new System.Drawing.Size(586, 585);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(586, 519);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // groupBoxQuantOpt
            // 
            this.groupBoxQuantOpt.Controls.Add(this.checkBoxReporterFilterMGF);
            this.groupBoxQuantOpt.Controls.Add(this.ckbxOutputQuant);
            this.groupBoxQuantOpt.Controls.Add(this.checkBoxReporterFilterMatrix);
            this.groupBoxQuantOpt.Controls.Add(this.comboBoxLabelingReagents);
            this.groupBoxQuantOpt.Controls.Add(this.labelReporterIonFilteringApplyTo);
            this.groupBoxQuantOpt.Controls.Add(this.checkBoxReporterIonFiltering);
            this.groupBoxQuantOpt.Controls.Add(this.textBoxReporterNumberMissingFilter);
            this.groupBoxQuantOpt.Controls.Add(this.labelReporterIonMissingFilter);
            this.groupBoxQuantOpt.Controls.Add(this.textBoxReporterIntensityFilter);
            this.groupBoxQuantOpt.Controls.Add(this.labelReporterIonIntensityFilter);
            this.groupBoxQuantOpt.Enabled = false;
            this.groupBoxQuantOpt.Location = new System.Drawing.Point(2, 359);
            this.groupBoxQuantOpt.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxQuantOpt.Name = "groupBoxQuantOpt";
            this.groupBoxQuantOpt.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxQuantOpt.Size = new System.Drawing.Size(320, 76);
            this.groupBoxQuantOpt.TabIndex = 16;
            this.groupBoxQuantOpt.TabStop = false;
            this.groupBoxQuantOpt.Text = "Quantification Options";
            // 
            // checkBoxReporterFilterMGF
            // 
            this.checkBoxReporterFilterMGF.AutoSize = true;
            this.checkBoxReporterFilterMGF.Enabled = false;
            this.checkBoxReporterFilterMGF.Location = new System.Drawing.Point(266, 82);
            this.checkBoxReporterFilterMGF.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxReporterFilterMGF.Name = "checkBoxReporterFilterMGF";
            this.checkBoxReporterFilterMGF.Size = new System.Drawing.Size(49, 17);
            this.checkBoxReporterFilterMGF.TabIndex = 15;
            this.checkBoxReporterFilterMGF.Text = "MGF";
            this.checkBoxReporterFilterMGF.UseVisualStyleBackColor = true;
            this.checkBoxReporterFilterMGF.Visible = false;
            // 
            // checkBoxReporterFilterMatrix
            // 
            this.checkBoxReporterFilterMatrix.AutoSize = true;
            this.checkBoxReporterFilterMatrix.Enabled = false;
            this.checkBoxReporterFilterMatrix.Location = new System.Drawing.Point(211, 82);
            this.checkBoxReporterFilterMatrix.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxReporterFilterMatrix.Name = "checkBoxReporterFilterMatrix";
            this.checkBoxReporterFilterMatrix.Size = new System.Drawing.Size(54, 17);
            this.checkBoxReporterFilterMatrix.TabIndex = 14;
            this.checkBoxReporterFilterMatrix.Text = "Matrix";
            this.checkBoxReporterFilterMatrix.UseVisualStyleBackColor = true;
            this.checkBoxReporterFilterMatrix.Visible = false;
            // 
            // labelReporterIonFilteringApplyTo
            // 
            this.labelReporterIonFilteringApplyTo.AutoSize = true;
            this.labelReporterIonFilteringApplyTo.Enabled = false;
            this.labelReporterIonFilteringApplyTo.Location = new System.Drawing.Point(159, 82);
            this.labelReporterIonFilteringApplyTo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelReporterIonFilteringApplyTo.Name = "labelReporterIonFilteringApplyTo";
            this.labelReporterIonFilteringApplyTo.Size = new System.Drawing.Size(48, 13);
            this.labelReporterIonFilteringApplyTo.TabIndex = 13;
            this.labelReporterIonFilteringApplyTo.Text = "Apply to:";
            this.labelReporterIonFilteringApplyTo.Visible = false;
            // 
            // groupBoxMgfOpts
            // 
            this.groupBoxMgfOpts.Controls.Add(this.textBoxMgfLowMass);
            this.groupBoxMgfOpts.Controls.Add(this.checkBoxMgfLowMass);
            this.groupBoxMgfOpts.Enabled = false;
            this.groupBoxMgfOpts.Location = new System.Drawing.Point(326, 359);
            this.groupBoxMgfOpts.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxMgfOpts.Name = "groupBoxMgfOpts";
            this.groupBoxMgfOpts.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxMgfOpts.Size = new System.Drawing.Size(238, 76);
            this.groupBoxMgfOpts.TabIndex = 17;
            this.groupBoxMgfOpts.TabStop = false;
            this.groupBoxMgfOpts.Text = "MGF Options";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonGo);
            this.panel1.Location = new System.Drawing.Point(143, 439);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(421, 65);
            this.panel1.TabIndex = 8;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(8, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(585, 24);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileStripMenuItem
            // 
            this.fileStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemNewParameters,
            this.toolStripMenuItemOpenParameters,
            this.toolStripMenuItemSaveParameters,
            this.exitToolStripMenuItemExit});
            this.fileStripMenuItem.Name = "fileStripMenuItem";
            this.fileStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileStripMenuItem.Text = "File";
            // 
            // toolStripMenuItemNewParameters
            // 
            this.toolStripMenuItemNewParameters.Name = "toolStripMenuItemNewParameters";
            this.toolStripMenuItemNewParameters.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItemNewParameters.Text = "New Parameters";
            this.toolStripMenuItemNewParameters.Click += new System.EventHandler(this.toolStripMenuItemNewParameters_Click);
            // 
            // toolStripMenuItemOpenParameters
            // 
            this.toolStripMenuItemOpenParameters.Name = "toolStripMenuItemOpenParameters";
            this.toolStripMenuItemOpenParameters.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItemOpenParameters.Text = "Open Parameters";
            this.toolStripMenuItemOpenParameters.Click += new System.EventHandler(this.toolStripMenuItemOpenParameters_Click);
            // 
            // toolStripMenuItemSaveParameters
            // 
            this.toolStripMenuItemSaveParameters.Name = "toolStripMenuItemSaveParameters";
            this.toolStripMenuItemSaveParameters.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItemSaveParameters.Text = "Save Parameters";
            this.toolStripMenuItemSaveParameters.Click += new System.EventHandler(this.toolStripMenuItemSaveParameters_Click);
            // 
            // exitToolStripMenuItemExit
            // 
            this.exitToolStripMenuItemExit.Name = "exitToolStripMenuItemExit";
            this.exitToolStripMenuItemExit.Size = new System.Drawing.Size(165, 22);
            this.exitToolStripMenuItemExit.Text = "Exit";
            this.exitToolStripMenuItemExit.Click += new System.EventHandler(this.exitToolStripMenuItemExit_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.aboutToolStripMenuItem.Text = "About RawTools";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // RawToolsGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(601, 552);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RawToolsGUI";
            this.Padding = new System.Windows.Forms.Padding(8, 0, 8, 12);
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
            this.groupBoxQuantOpt.ResumeLayout(false);
            this.groupBoxQuantOpt.PerformLayout();
            this.groupBoxMgfOpts.ResumeLayout(false);
            this.groupBoxMgfOpts.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.RadioButton radioButtonSearchXTandem;
        private System.Windows.Forms.RadioButton radioButtonSearchNone;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFastaFile;
        private System.Windows.Forms.Button buttonFastaFile;
        private System.Windows.Forms.TextBox textBoxXTandemDir;
        private System.Windows.Forms.Button buttonXTandemDir;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label labelNumSpectra;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox textBoxNumSpectra;
        private System.Windows.Forms.ComboBox comboBoxMinCharge;
        private System.Windows.Forms.Label labelMinCharge;
        private System.Windows.Forms.TextBox textBoxDataOutputDir;
        private System.Windows.Forms.Button buttonDataOutputDir;
        private System.Windows.Forms.CheckBox checkBoxDataOutputDirectory;
        private System.Windows.Forms.CheckBox checkBoxChroBP;
        private System.Windows.Forms.CheckBox checkBoxChroTIC;
        private System.Windows.Forms.CheckBox checkBoxChroMs2;
        private System.Windows.Forms.CheckBox checkBoxChroMs1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonGo;
        private System.Windows.Forms.TextBox textBoxReporterNumberMissingFilter;
        private System.Windows.Forms.TextBox textBoxReporterIntensityFilter;
        private System.Windows.Forms.Label labelReporterIonIntensityFilter;
        private System.Windows.Forms.Label labelReporterIonMissingFilter;
        private System.Windows.Forms.CheckBox checkBoxReporterIonFiltering;
        private System.Windows.Forms.CheckBox checkBoxReporterFilterMGF;
        private System.Windows.Forms.CheckBox checkBoxReporterFilterMatrix;
        private System.Windows.Forms.Label labelReporterIonFilteringApplyTo;
        private System.Windows.Forms.GroupBox groupBoxQuantOpt;
        private System.Windows.Forms.ComboBox comboBoxLabelingReagents;
        private System.Windows.Forms.GroupBox groupBoxMgfOpts;
        private System.Windows.Forms.TextBox textBoxMgfLowMass;
        private System.Windows.Forms.CheckBox checkBoxMgfLowMass;
        private System.Windows.Forms.TextBox textBoxQcDataDirectory;
        private System.Windows.Forms.Button buttonQcDataDirectory;
        private System.Windows.Forms.Label labelPeptideMods;
        private System.Windows.Forms.Button buttonPeptideMods;

        private PeptideModifications peptideModifications;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNewParameters;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpenParameters;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSaveParameters;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItemExit;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxFaims;
    }
}

