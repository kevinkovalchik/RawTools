namespace RawToolsViz
{
    partial class RawFileViz
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RawFileViz));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.topTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.divider1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.spectrumLabel = new System.Windows.Forms.Label();
            this.totalScansLabel = new System.Windows.Forms.Label();
            this.nextScanButton = new System.Windows.Forms.Button();
            this.previousScanButton = new System.Windows.Forms.Button();
            this.scanNumber = new System.Windows.Forms.TextBox();
            this.yAxisLabel = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.xAxisLabel = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.exportAsComboBox = new System.Windows.Forms.ComboBox();
            this.exportHeightValue = new System.Windows.Forms.TextBox();
            this.exportWidthValue = new System.Windows.Forms.TextBox();
            this.exportHeight = new System.Windows.Forms.Label();
            this.exportWidth = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.exportButton = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.yMaxFixedValue = new System.Windows.Forms.TextBox();
            this.yMaxFixed = new System.Windows.Forms.RadioButton();
            this.yMaxAuto = new System.Windows.Forms.RadioButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.yMinFixedValue = new System.Windows.Forms.TextBox();
            this.yMinFixed = new System.Windows.Forms.RadioButton();
            this.yMinAuto = new System.Windows.Forms.RadioButton();
            this.yAxisMax = new System.Windows.Forms.Label();
            this.yAxisMin = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.plotAreaTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.scanFilterTextBox = new System.Windows.Forms.TextBox();
            this.plotViewMassSpectrum = new OxyPlot.WindowsForms.PlotView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.trailerExtraTextBox = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.instLogTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.plotViewChromatogram = new OxyPlot.WindowsForms.PlotView();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.chroMassTolmDa = new System.Windows.Forms.RadioButton();
            this.chroMassTolPPM = new System.Windows.Forms.RadioButton();
            this.chroMassTolDa = new System.Windows.Forms.RadioButton();
            this.chroXICToleranceTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ChroXICmzTextBox = new System.Windows.Forms.TextBox();
            this.ChroXICmzLabel = new System.Windows.Forms.Label();
            this.ChroXICRadioButton = new System.Windows.Forms.RadioButton();
            this.ChroBPRadioButton = new System.Windows.Forms.RadioButton();
            this.ChroTICRadioButton = new System.Windows.Forms.RadioButton();
            this.ChroMsLevelComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            this.topTableLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.plotAreaTableLayoutPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1145, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(108, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(116, 26);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // topTableLayoutPanel
            // 
            this.topTableLayoutPanel.ColumnCount = 1;
            this.topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topTableLayoutPanel.Controls.Add(this.panel1, 0, 1);
            this.topTableLayoutPanel.Controls.Add(this.panel2, 0, 3);
            this.topTableLayoutPanel.Controls.Add(this.splitContainer1, 0, 2);
            this.topTableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.topTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topTableLayoutPanel.Location = new System.Drawing.Point(0, 28);
            this.topTableLayoutPanel.Name = "topTableLayoutPanel";
            this.topTableLayoutPanel.RowCount = 4;
            this.topTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.topTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.topTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.topTableLayoutPanel.Size = new System.Drawing.Size(1145, 737);
            this.topTableLayoutPanel.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.divider1);
            this.panel1.Controls.Add(this.tableLayoutPanel3);
            this.panel1.Controls.Add(this.yAxisLabel);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.xAxisLabel);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 153);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3, 6, 6, 3);
            this.panel1.Size = new System.Drawing.Size(1139, 44);
            this.panel1.TabIndex = 1;
            // 
            // divider1
            // 
            this.divider1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.divider1.Location = new System.Drawing.Point(-3, 38);
            this.divider1.Name = "divider1";
            this.divider1.Size = new System.Drawing.Size(1145, 2);
            this.divider1.TabIndex = 14;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 89F));
            this.tableLayoutPanel3.Controls.Add(this.spectrumLabel, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.totalScansLabel, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.nextScanButton, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.previousScanButton, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.scanNumber, 2, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(301, 26);
            this.tableLayoutPanel3.TabIndex = 13;
            // 
            // spectrumLabel
            // 
            this.spectrumLabel.AutoSize = true;
            this.spectrumLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spectrumLabel.Location = new System.Drawing.Point(3, 0);
            this.spectrumLabel.Name = "spectrumLabel";
            this.spectrumLabel.Size = new System.Drawing.Size(74, 26);
            this.spectrumLabel.TabIndex = 10;
            this.spectrumLabel.Text = "Spectrum";
            this.spectrumLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // totalScansLabel
            // 
            this.totalScansLabel.AutoSize = true;
            this.totalScansLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalScansLabel.Location = new System.Drawing.Point(215, 0);
            this.totalScansLabel.Name = "totalScansLabel";
            this.totalScansLabel.Size = new System.Drawing.Size(83, 26);
            this.totalScansLabel.TabIndex = 9;
            this.totalScansLabel.Text = "/ 100";
            this.totalScansLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nextScanButton
            // 
            this.nextScanButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nextScanButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.nextScanButton.Location = new System.Drawing.Point(180, 0);
            this.nextScanButton.Margin = new System.Windows.Forms.Padding(0);
            this.nextScanButton.Name = "nextScanButton";
            this.nextScanButton.Size = new System.Drawing.Size(32, 26);
            this.nextScanButton.TabIndex = 11;
            this.nextScanButton.Text = ">";
            this.toolTip1.SetToolTip(this.nextScanButton, "Next scan");
            this.nextScanButton.UseVisualStyleBackColor = true;
            this.nextScanButton.Click += new System.EventHandler(this.nextScanButton_Click);
            // 
            // previousScanButton
            // 
            this.previousScanButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previousScanButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.previousScanButton.Location = new System.Drawing.Point(80, 0);
            this.previousScanButton.Margin = new System.Windows.Forms.Padding(0);
            this.previousScanButton.Name = "previousScanButton";
            this.previousScanButton.Size = new System.Drawing.Size(32, 26);
            this.previousScanButton.TabIndex = 12;
            this.previousScanButton.Text = "<";
            this.previousScanButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.previousScanButton, "Previous scan");
            this.previousScanButton.UseVisualStyleBackColor = true;
            this.previousScanButton.Click += new System.EventHandler(this.previousScanButton_Click);
            // 
            // scanNumber
            // 
            this.scanNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scanNumber.Location = new System.Drawing.Point(112, 0);
            this.scanNumber.Margin = new System.Windows.Forms.Padding(0);
            this.scanNumber.Name = "scanNumber";
            this.scanNumber.Size = new System.Drawing.Size(68, 22);
            this.scanNumber.TabIndex = 8;
            this.toolTip1.SetToolTip(this.scanNumber, "Selected scan");
            this.scanNumber.TextChanged += new System.EventHandler(this.scanNumber_TextChanged);
            // 
            // yAxisLabel
            // 
            this.yAxisLabel.Location = new System.Drawing.Point(746, 5);
            this.yAxisLabel.Name = "yAxisLabel";
            this.yAxisLabel.Size = new System.Drawing.Size(271, 22);
            this.yAxisLabel.TabIndex = 7;
            this.yAxisLabel.TextChanged += new System.EventHandler(this.yAxisLabel_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(686, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 17);
            this.label6.TabIndex = 6;
            this.label6.Text = "y-label:";
            // 
            // xAxisLabel
            // 
            this.xAxisLabel.Location = new System.Drawing.Point(387, 5);
            this.xAxisLabel.Name = "xAxisLabel";
            this.xAxisLabel.Size = new System.Drawing.Size(271, 22);
            this.xAxisLabel.TabIndex = 5;
            this.xAxisLabel.TextChanged += new System.EventHandler(this.xAxisLabel_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(328, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "x-label:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.exportAsComboBox);
            this.panel2.Controls.Add(this.exportHeightValue);
            this.panel2.Controls.Add(this.exportWidthValue);
            this.panel2.Controls.Add(this.exportHeight);
            this.panel2.Controls.Add(this.exportWidth);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.exportButton);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.yAxisMax);
            this.panel2.Controls.Add(this.yAxisMin);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 650);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1139, 84);
            this.panel2.TabIndex = 2;
            // 
            // exportAsComboBox
            // 
            this.exportAsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exportAsComboBox.FormattingEnabled = true;
            this.exportAsComboBox.Items.AddRange(new object[] {
            "PNG",
            "SVG",
            "PDF"});
            this.exportAsComboBox.Location = new System.Drawing.Point(422, 15);
            this.exportAsComboBox.Name = "exportAsComboBox";
            this.exportAsComboBox.Size = new System.Drawing.Size(171, 24);
            this.exportAsComboBox.TabIndex = 21;
            // 
            // exportHeightValue
            // 
            this.exportHeightValue.Location = new System.Drawing.Point(541, 54);
            this.exportHeightValue.Name = "exportHeightValue";
            this.exportHeightValue.Size = new System.Drawing.Size(53, 22);
            this.exportHeightValue.TabIndex = 20;
            this.exportHeightValue.Text = "540";
            this.exportHeightValue.TextChanged += new System.EventHandler(this.exportHeightValue_TextChanged);
            // 
            // exportWidthValue
            // 
            this.exportWidthValue.Location = new System.Drawing.Point(422, 54);
            this.exportWidthValue.Name = "exportWidthValue";
            this.exportWidthValue.Size = new System.Drawing.Size(54, 22);
            this.exportWidthValue.TabIndex = 19;
            this.exportWidthValue.Text = "800";
            this.exportWidthValue.TextChanged += new System.EventHandler(this.exportWidthValue_TextChanged);
            // 
            // exportHeight
            // 
            this.exportHeight.AutoSize = true;
            this.exportHeight.Location = new System.Drawing.Point(482, 54);
            this.exportHeight.Name = "exportHeight";
            this.exportHeight.Size = new System.Drawing.Size(53, 17);
            this.exportHeight.TabIndex = 18;
            this.exportHeight.Text = "Height:";
            // 
            // exportWidth
            // 
            this.exportWidth.AutoSize = true;
            this.exportWidth.Location = new System.Drawing.Point(369, 56);
            this.exportWidth.Name = "exportWidth";
            this.exportWidth.Size = new System.Drawing.Size(48, 17);
            this.exportWidth.TabIndex = 17;
            this.exportWidth.Text = "Width:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(346, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Export as:";
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(600, 12);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(155, 66);
            this.exportButton.TabIndex = 12;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.yMaxFixedValue);
            this.panel4.Controls.Add(this.yMaxFixed);
            this.panel4.Controls.Add(this.yMaxAuto);
            this.panel4.Location = new System.Drawing.Point(69, 12);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(248, 30);
            this.panel4.TabIndex = 11;
            // 
            // yMaxFixedValue
            // 
            this.yMaxFixedValue.Enabled = false;
            this.yMaxFixedValue.Location = new System.Drawing.Point(139, 3);
            this.yMaxFixedValue.Name = "yMaxFixedValue";
            this.yMaxFixedValue.Size = new System.Drawing.Size(100, 22);
            this.yMaxFixedValue.TabIndex = 3;
            this.yMaxFixedValue.Text = "NaN";
            this.yMaxFixedValue.TextChanged += new System.EventHandler(this.yMaxFixedValue_TextChanged);
            this.yMaxFixedValue.Leave += new System.EventHandler(this.yMaxFixedValue_Leave);
            // 
            // yMaxFixed
            // 
            this.yMaxFixed.AutoSize = true;
            this.yMaxFixed.Location = new System.Drawing.Point(67, 3);
            this.yMaxFixed.Name = "yMaxFixed";
            this.yMaxFixed.Size = new System.Drawing.Size(66, 21);
            this.yMaxFixed.TabIndex = 1;
            this.yMaxFixed.Text = "Fixed:";
            this.yMaxFixed.UseVisualStyleBackColor = true;
            // 
            // yMaxAuto
            // 
            this.yMaxAuto.AutoSize = true;
            this.yMaxAuto.Checked = true;
            this.yMaxAuto.Location = new System.Drawing.Point(3, 3);
            this.yMaxAuto.Name = "yMaxAuto";
            this.yMaxAuto.Size = new System.Drawing.Size(58, 21);
            this.yMaxAuto.TabIndex = 0;
            this.yMaxAuto.TabStop = true;
            this.yMaxAuto.Text = "Auto";
            this.yMaxAuto.UseVisualStyleBackColor = true;
            this.yMaxAuto.CheckedChanged += new System.EventHandler(this.yMaxAuto_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.yMinFixedValue);
            this.panel3.Controls.Add(this.yMinFixed);
            this.panel3.Controls.Add(this.yMinAuto);
            this.panel3.Location = new System.Drawing.Point(69, 48);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(248, 30);
            this.panel3.TabIndex = 10;
            // 
            // yMinFixedValue
            // 
            this.yMinFixedValue.Enabled = false;
            this.yMinFixedValue.Location = new System.Drawing.Point(139, 3);
            this.yMinFixedValue.Name = "yMinFixedValue";
            this.yMinFixedValue.Size = new System.Drawing.Size(100, 22);
            this.yMinFixedValue.TabIndex = 2;
            this.yMinFixedValue.Text = "NaN";
            this.yMinFixedValue.TextChanged += new System.EventHandler(this.yMinFixedValue_TextChanged);
            this.yMinFixedValue.Leave += new System.EventHandler(this.yMinFixedValue_Leave);
            // 
            // yMinFixed
            // 
            this.yMinFixed.AutoSize = true;
            this.yMinFixed.Location = new System.Drawing.Point(67, 3);
            this.yMinFixed.Name = "yMinFixed";
            this.yMinFixed.Size = new System.Drawing.Size(66, 21);
            this.yMinFixed.TabIndex = 1;
            this.yMinFixed.Text = "Fixed:";
            this.yMinFixed.UseVisualStyleBackColor = true;
            // 
            // yMinAuto
            // 
            this.yMinAuto.AutoSize = true;
            this.yMinAuto.Checked = true;
            this.yMinAuto.Location = new System.Drawing.Point(3, 3);
            this.yMinAuto.Name = "yMinAuto";
            this.yMinAuto.Size = new System.Drawing.Size(58, 21);
            this.yMinAuto.TabIndex = 0;
            this.yMinAuto.TabStop = true;
            this.yMinAuto.Text = "Auto";
            this.yMinAuto.UseVisualStyleBackColor = true;
            this.yMinAuto.CheckedChanged += new System.EventHandler(this.yMinAuto_CheckedChanged);
            // 
            // yAxisMax
            // 
            this.yAxisMax.AutoSize = true;
            this.yAxisMax.Location = new System.Drawing.Point(14, 17);
            this.yAxisMax.Name = "yAxisMax";
            this.yAxisMax.Size = new System.Drawing.Size(49, 17);
            this.yAxisMax.TabIndex = 9;
            this.yAxisMax.Text = "y-max:";
            // 
            // yAxisMin
            // 
            this.yAxisMin.AutoSize = true;
            this.yAxisMin.Location = new System.Drawing.Point(17, 52);
            this.yAxisMin.Name = "yAxisMin";
            this.yAxisMin.Size = new System.Drawing.Size(46, 17);
            this.yAxisMin.TabIndex = 8;
            this.yAxisMin.Text = "y-min:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 203);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.plotAreaTableLayoutPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1139, 441);
            this.splitContainer1.SplitterDistance = 777;
            this.splitContainer1.TabIndex = 3;
            // 
            // plotAreaTableLayoutPanel
            // 
            this.plotAreaTableLayoutPanel.ColumnCount = 1;
            this.plotAreaTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.plotAreaTableLayoutPanel.Controls.Add(this.scanFilterTextBox, 0, 0);
            this.plotAreaTableLayoutPanel.Controls.Add(this.plotViewMassSpectrum, 0, 1);
            this.plotAreaTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotAreaTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.plotAreaTableLayoutPanel.Name = "plotAreaTableLayoutPanel";
            this.plotAreaTableLayoutPanel.RowCount = 2;
            this.plotAreaTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.plotAreaTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.plotAreaTableLayoutPanel.Size = new System.Drawing.Size(777, 441);
            this.plotAreaTableLayoutPanel.TabIndex = 0;
            // 
            // scanFilterTextBox
            // 
            this.scanFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scanFilterTextBox.BackColor = System.Drawing.Color.White;
            this.scanFilterTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.scanFilterTextBox.Location = new System.Drawing.Point(3, 3);
            this.scanFilterTextBox.Multiline = true;
            this.scanFilterTextBox.Name = "scanFilterTextBox";
            this.scanFilterTextBox.ReadOnly = true;
            this.scanFilterTextBox.ShortcutsEnabled = false;
            this.scanFilterTextBox.Size = new System.Drawing.Size(771, 34);
            this.scanFilterTextBox.TabIndex = 0;
            this.scanFilterTextBox.WordWrap = false;
            // 
            // plotViewMassSpectrum
            // 
            this.plotViewMassSpectrum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotViewMassSpectrum.Location = new System.Drawing.Point(3, 43);
            this.plotViewMassSpectrum.Name = "plotViewMassSpectrum";
            this.plotViewMassSpectrum.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plotViewMassSpectrum.Size = new System.Drawing.Size(771, 395);
            this.plotViewMassSpectrum.TabIndex = 1;
            this.plotViewMassSpectrum.Text = "plotView1";
            this.toolTip1.SetToolTip(this.plotViewMassSpectrum, resources.GetString("plotViewMassSpectrum.ToolTip"));
            this.plotViewMassSpectrum.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plotViewMassSpectrum.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plotViewMassSpectrum.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.MinimumSize = new System.Drawing.Size(320, 433);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(358, 441);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.trailerExtraTextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(350, 412);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Trailer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // trailerExtraTextBox
            // 
            this.trailerExtraTextBox.BackColor = System.Drawing.Color.White;
            this.trailerExtraTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.trailerExtraTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trailerExtraTextBox.Location = new System.Drawing.Point(3, 3);
            this.trailerExtraTextBox.Multiline = true;
            this.trailerExtraTextBox.Name = "trailerExtraTextBox";
            this.trailerExtraTextBox.ReadOnly = true;
            this.trailerExtraTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.trailerExtraTextBox.Size = new System.Drawing.Size(344, 406);
            this.trailerExtraTextBox.TabIndex = 1;
            this.trailerExtraTextBox.WordWrap = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.instLogTextBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(350, 412);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Status log";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // instLogTextBox
            // 
            this.instLogTextBox.BackColor = System.Drawing.Color.White;
            this.instLogTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.instLogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instLogTextBox.Location = new System.Drawing.Point(0, 0);
            this.instLogTextBox.Multiline = true;
            this.instLogTextBox.Name = "instLogTextBox";
            this.instLogTextBox.ReadOnly = true;
            this.instLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.instLogTextBox.Size = new System.Drawing.Size(350, 412);
            this.instLogTextBox.TabIndex = 2;
            this.instLogTextBox.WordWrap = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Controls.Add(this.plotViewChromatogram, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1139, 144);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // plotViewChromatogram
            // 
            this.plotViewChromatogram.Cursor = System.Windows.Forms.Cursors.Cross;
            this.plotViewChromatogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotViewChromatogram.Location = new System.Drawing.Point(3, 3);
            this.plotViewChromatogram.Name = "plotViewChromatogram";
            this.plotViewChromatogram.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plotViewChromatogram.Size = new System.Drawing.Size(933, 138);
            this.plotViewChromatogram.TabIndex = 2;
            this.plotViewChromatogram.Text = "plotView1";
            this.toolTip1.SetToolTip(this.plotViewChromatogram, resources.GetString("plotViewChromatogram.ToolTip"));
            this.plotViewChromatogram.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plotViewChromatogram.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plotViewChromatogram.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Controls.Add(this.chroXICToleranceTextBox);
            this.panel5.Controls.Add(this.label2);
            this.panel5.Controls.Add(this.ChroXICmzTextBox);
            this.panel5.Controls.Add(this.ChroXICmzLabel);
            this.panel5.Controls.Add(this.ChroXICRadioButton);
            this.panel5.Controls.Add(this.ChroBPRadioButton);
            this.panel5.Controls.Add(this.ChroTICRadioButton);
            this.panel5.Controls.Add(this.ChroMsLevelComboBox);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(942, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(194, 138);
            this.panel5.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.chroMassTolmDa);
            this.panel6.Controls.Add(this.chroMassTolPPM);
            this.panel6.Controls.Add(this.chroMassTolDa);
            this.panel6.Location = new System.Drawing.Point(24, 109);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(170, 26);
            this.panel6.TabIndex = 9;
            // 
            // chroMassTolmDa
            // 
            this.chroMassTolmDa.AutoSize = true;
            this.chroMassTolmDa.Location = new System.Drawing.Point(118, 2);
            this.chroMassTolmDa.Name = "chroMassTolmDa";
            this.chroMassTolmDa.Size = new System.Drawing.Size(58, 21);
            this.chroMassTolmDa.TabIndex = 1;
            this.chroMassTolmDa.Text = "mDa";
            this.chroMassTolmDa.UseVisualStyleBackColor = true;
            this.chroMassTolmDa.CheckedChanged += new System.EventHandler(this.chromatogramTolType_checkChanged);
            // 
            // chroMassTolPPM
            // 
            this.chroMassTolPPM.AutoSize = true;
            this.chroMassTolPPM.Location = new System.Drawing.Point(3, 2);
            this.chroMassTolPPM.Name = "chroMassTolPPM";
            this.chroMassTolPPM.Size = new System.Drawing.Size(56, 21);
            this.chroMassTolPPM.TabIndex = 0;
            this.chroMassTolPPM.Text = "ppm";
            this.chroMassTolPPM.UseVisualStyleBackColor = true;
            this.chroMassTolPPM.CheckedChanged += new System.EventHandler(this.chromatogramTolType_checkChanged);
            // 
            // chroMassTolDa
            // 
            this.chroMassTolDa.AutoSize = true;
            this.chroMassTolDa.Checked = true;
            this.chroMassTolDa.Location = new System.Drawing.Point(65, 2);
            this.chroMassTolDa.Name = "chroMassTolDa";
            this.chroMassTolDa.Size = new System.Drawing.Size(47, 21);
            this.chroMassTolDa.TabIndex = 0;
            this.chroMassTolDa.TabStop = true;
            this.chroMassTolDa.Text = "Da";
            this.chroMassTolDa.UseVisualStyleBackColor = true;
            this.chroMassTolDa.CheckedChanged += new System.EventHandler(this.chromatogramTolType_checkChanged);
            // 
            // chroXICToleranceTextBox
            // 
            this.chroXICToleranceTextBox.Location = new System.Drawing.Point(103, 83);
            this.chroXICToleranceTextBox.Name = "chroXICToleranceTextBox";
            this.chroXICToleranceTextBox.Size = new System.Drawing.Size(85, 22);
            this.chroXICToleranceTextBox.TabIndex = 8;
            this.chroXICToleranceTextBox.TextChanged += new System.EventHandler(this.chroXICToleranceTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Tolerance:";
            // 
            // ChroXICmzTextBox
            // 
            this.ChroXICmzTextBox.Location = new System.Drawing.Point(103, 55);
            this.ChroXICmzTextBox.Name = "ChroXICmzTextBox";
            this.ChroXICmzTextBox.Size = new System.Drawing.Size(85, 22);
            this.ChroXICmzTextBox.TabIndex = 6;
            this.ChroXICmzTextBox.TextChanged += new System.EventHandler(this.ChroXICmzTextBox_TextChanged);
            // 
            // ChroXICmzLabel
            // 
            this.ChroXICmzLabel.AutoSize = true;
            this.ChroXICmzLabel.Location = new System.Drawing.Point(3, 58);
            this.ChroXICmzLabel.Name = "ChroXICmzLabel";
            this.ChroXICmzLabel.Size = new System.Drawing.Size(94, 17);
            this.ChroXICmzLabel.TabIndex = 5;
            this.ChroXICmzLabel.Text = "Extracted ion:";
            // 
            // ChroXICRadioButton
            // 
            this.ChroXICRadioButton.AutoSize = true;
            this.ChroXICRadioButton.Location = new System.Drawing.Point(115, 30);
            this.ChroXICRadioButton.Name = "ChroXICRadioButton";
            this.ChroXICRadioButton.Size = new System.Drawing.Size(50, 21);
            this.ChroXICRadioButton.TabIndex = 4;
            this.ChroXICRadioButton.Text = "XIC";
            this.ChroXICRadioButton.UseVisualStyleBackColor = true;
            this.ChroXICRadioButton.CheckedChanged += new System.EventHandler(this.chromatogramType_checkChanged);
            // 
            // ChroBPRadioButton
            // 
            this.ChroBPRadioButton.AutoSize = true;
            this.ChroBPRadioButton.Location = new System.Drawing.Point(62, 30);
            this.ChroBPRadioButton.Name = "ChroBPRadioButton";
            this.ChroBPRadioButton.Size = new System.Drawing.Size(47, 21);
            this.ChroBPRadioButton.TabIndex = 3;
            this.ChroBPRadioButton.Text = "BP";
            this.ChroBPRadioButton.UseVisualStyleBackColor = true;
            this.ChroBPRadioButton.CheckedChanged += new System.EventHandler(this.chromatogramType_checkChanged);
            // 
            // ChroTICRadioButton
            // 
            this.ChroTICRadioButton.AutoSize = true;
            this.ChroTICRadioButton.Checked = true;
            this.ChroTICRadioButton.Location = new System.Drawing.Point(6, 30);
            this.ChroTICRadioButton.Name = "ChroTICRadioButton";
            this.ChroTICRadioButton.Size = new System.Drawing.Size(50, 21);
            this.ChroTICRadioButton.TabIndex = 2;
            this.ChroTICRadioButton.TabStop = true;
            this.ChroTICRadioButton.Text = "TIC";
            this.ChroTICRadioButton.UseVisualStyleBackColor = true;
            this.ChroTICRadioButton.CheckedChanged += new System.EventHandler(this.chromatogramType_checkChanged);
            // 
            // ChroMsLevelComboBox
            // 
            this.ChroMsLevelComboBox.BackColor = System.Drawing.Color.White;
            this.ChroMsLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChroMsLevelComboBox.FormattingEnabled = true;
            this.ChroMsLevelComboBox.Items.AddRange(new object[] {
            "MS1",
            "MS2",
            "MS3"});
            this.ChroMsLevelComboBox.Location = new System.Drawing.Point(74, 0);
            this.ChroMsLevelComboBox.Name = "ChroMsLevelComboBox";
            this.ChroMsLevelComboBox.Size = new System.Drawing.Size(114, 24);
            this.ChroMsLevelComboBox.TabIndex = 1;
            this.ChroMsLevelComboBox.SelectionChangeCommitted += new System.EventHandler(this.ChroMsLevelComboBox_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "MS level:";
            // 
            // RawFileViz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1145, 765);
            this.Controls.Add(this.topTableLayoutPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1000, 694);
            this.Name = "RawFileViz";
            this.Text = "ParseDataViz";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.topTableLayoutPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.plotAreaTableLayoutPanel.ResumeLayout(false);
            this.plotAreaTableLayoutPanel.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel topTableLayoutPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox yMaxFixedValue;
        private System.Windows.Forms.RadioButton yMaxFixed;
        private System.Windows.Forms.RadioButton yMaxAuto;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox yMinFixedValue;
        private System.Windows.Forms.RadioButton yMinFixed;
        private System.Windows.Forms.RadioButton yMinAuto;
        private System.Windows.Forms.Label yAxisMax;
        private System.Windows.Forms.Label yAxisMin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Label exportWidth;
        private System.Windows.Forms.TextBox exportHeightValue;
        private System.Windows.Forms.TextBox exportWidthValue;
        private System.Windows.Forms.Label exportHeight;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox exportAsComboBox;
        private System.Windows.Forms.TextBox yAxisLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox xAxisLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label totalScansLabel;
        private System.Windows.Forms.TextBox scanNumber;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private OxyPlot.WindowsForms.PlotView plotViewMassSpectrum;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox scanFilterTextBox;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox trailerExtraTextBox;
        private System.Windows.Forms.TextBox instLogTextBox;
        private System.Windows.Forms.TableLayoutPanel plotAreaTableLayoutPanel;
        private System.Windows.Forms.Button previousScanButton;
        private System.Windows.Forms.Button nextScanButton;
        private System.Windows.Forms.Label spectrumLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label divider1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ChroMsLevelComboBox;
        private System.Windows.Forms.RadioButton chroMassTolPPM;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.RadioButton chroMassTolDa;
        private System.Windows.Forms.TextBox chroXICToleranceTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ChroXICmzTextBox;
        private System.Windows.Forms.Label ChroXICmzLabel;
        private System.Windows.Forms.RadioButton ChroXICRadioButton;
        private System.Windows.Forms.RadioButton ChroBPRadioButton;
        private System.Windows.Forms.RadioButton ChroTICRadioButton;
        private OxyPlot.WindowsForms.PlotView plotViewChromatogram;
        private System.Windows.Forms.RadioButton chroMassTolmDa;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

