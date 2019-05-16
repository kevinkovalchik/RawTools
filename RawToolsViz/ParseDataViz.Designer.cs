namespace RawToolsViz
{
    partial class ParseDataViz
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
            PresentationControls.CheckBoxProperties checkBoxProperties1 = new PresentationControls.CheckBoxProperties();
            this.plotView1 = new OxyPlot.WindowsForms.PlotView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxComboBox1 = new PresentationControls.CheckBoxComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.exportHeightValue = new System.Windows.Forms.TextBox();
            this.exportWidthValue = new System.Windows.Forms.TextBox();
            this.exportHeight = new System.Windows.Forms.Label();
            this.exportWidth = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.exportAsSvg = new System.Windows.Forms.RadioButton();
            this.exportAsPdf = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
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
            this.logYScaleBase = new System.Windows.Forms.TextBox();
            this.logYScale = new System.Windows.Forms.RadioButton();
            this.linearYScale = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.axisTypeComboBox = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // plotView1
            // 
            this.plotView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotView1.Location = new System.Drawing.Point(3, 43);
            this.plotView1.Name = "plotView1";
            this.plotView1.PanCursor = System.Windows.Forms.Cursors.Hand;
            this.plotView1.Size = new System.Drawing.Size(1083, 486);
            this.plotView1.TabIndex = 0;
            this.plotView1.Text = "plotView1";
            this.plotView1.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
            this.plotView1.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.plotView1.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1089, 28);
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
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.plotView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 28);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1089, 619);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxComboBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3, 6, 6, 3);
            this.panel1.Size = new System.Drawing.Size(1083, 34);
            this.panel1.TabIndex = 1;
            // 
            // checkBoxComboBox1
            // 
            this.checkBoxComboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            checkBoxProperties1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxComboBox1.CheckBoxProperties = checkBoxProperties1;
            this.checkBoxComboBox1.DisplayMemberSingleItem = "";
            this.checkBoxComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.checkBoxComboBox1.FormattingEnabled = true;
            this.checkBoxComboBox1.Location = new System.Drawing.Point(593, 6);
            this.checkBoxComboBox1.Name = "checkBoxComboBox1";
            this.checkBoxComboBox1.Size = new System.Drawing.Size(484, 24);
            this.checkBoxComboBox1.TabIndex = 2;
            this.checkBoxComboBox1.CheckBoxCheckedChanged += new System.EventHandler(this.checkBoxComboBox1_CheckBoxCheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(502, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data to plot:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.exportHeightValue);
            this.panel2.Controls.Add(this.exportWidthValue);
            this.panel2.Controls.Add(this.exportHeight);
            this.panel2.Controls.Add(this.exportWidth);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.yAxisMax);
            this.panel2.Controls.Add(this.yAxisMin);
            this.panel2.Controls.Add(this.logYScaleBase);
            this.panel2.Controls.Add(this.logYScale);
            this.panel2.Controls.Add(this.linearYScale);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.axisTypeComboBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 535);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1083, 81);
            this.panel2.TabIndex = 2;
            // 
            // exportHeightValue
            // 
            this.exportHeightValue.Location = new System.Drawing.Point(863, 50);
            this.exportHeightValue.Name = "exportHeightValue";
            this.exportHeightValue.Size = new System.Drawing.Size(53, 22);
            this.exportHeightValue.TabIndex = 20;
            this.exportHeightValue.Text = "540";
            this.exportHeightValue.TextChanged += new System.EventHandler(this.exportHeightValue_TextChanged);
            // 
            // exportWidthValue
            // 
            this.exportWidthValue.Location = new System.Drawing.Point(744, 50);
            this.exportWidthValue.Name = "exportWidthValue";
            this.exportWidthValue.Size = new System.Drawing.Size(54, 22);
            this.exportWidthValue.TabIndex = 19;
            this.exportWidthValue.Text = "800";
            this.exportWidthValue.TextChanged += new System.EventHandler(this.exportWidthValue_TextChanged);
            // 
            // exportHeight
            // 
            this.exportHeight.AutoSize = true;
            this.exportHeight.Location = new System.Drawing.Point(804, 50);
            this.exportHeight.Name = "exportHeight";
            this.exportHeight.Size = new System.Drawing.Size(53, 17);
            this.exportHeight.TabIndex = 18;
            this.exportHeight.Text = "Height:";
            // 
            // exportWidth
            // 
            this.exportWidth.AutoSize = true;
            this.exportWidth.Location = new System.Drawing.Point(691, 52);
            this.exportWidth.Name = "exportWidth";
            this.exportWidth.Size = new System.Drawing.Size(48, 17);
            this.exportWidth.TabIndex = 17;
            this.exportWidth.Text = "Width:";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.exportAsSvg);
            this.panel5.Controls.Add(this.exportAsPdf);
            this.panel5.Location = new System.Drawing.Point(740, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(125, 36);
            this.panel5.TabIndex = 16;
            // 
            // exportAsSvg
            // 
            this.exportAsSvg.AutoSize = true;
            this.exportAsSvg.Checked = true;
            this.exportAsSvg.Location = new System.Drawing.Point(3, 9);
            this.exportAsSvg.Name = "exportAsSvg";
            this.exportAsSvg.Size = new System.Drawing.Size(55, 21);
            this.exportAsSvg.TabIndex = 14;
            this.exportAsSvg.TabStop = true;
            this.exportAsSvg.Text = ".svg";
            this.exportAsSvg.UseVisualStyleBackColor = true;
            // 
            // exportAsPdf
            // 
            this.exportAsPdf.AutoSize = true;
            this.exportAsPdf.Location = new System.Drawing.Point(64, 9);
            this.exportAsPdf.Name = "exportAsPdf";
            this.exportAsPdf.Size = new System.Drawing.Size(53, 21);
            this.exportAsPdf.TabIndex = 15;
            this.exportAsPdf.TabStop = true;
            this.exportAsPdf.Text = ".pdf";
            this.exportAsPdf.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(668, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Export as:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(922, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 66);
            this.button1.TabIndex = 12;
            this.button1.Text = "Export";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.yMaxFixedValue);
            this.panel4.Controls.Add(this.yMaxFixed);
            this.panel4.Controls.Add(this.yMaxAuto);
            this.panel4.Location = new System.Drawing.Point(391, 8);
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
            this.yMaxFixedValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.yMaxFixedValue_KeyPress);
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
            this.panel3.Location = new System.Drawing.Point(391, 44);
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
            this.yMinFixedValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.yMinFixedValue_KeyPress);
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
            this.yAxisMax.Location = new System.Drawing.Point(336, 13);
            this.yAxisMax.Name = "yAxisMax";
            this.yAxisMax.Size = new System.Drawing.Size(49, 17);
            this.yAxisMax.TabIndex = 9;
            this.yAxisMax.Text = "y-max:";
            // 
            // yAxisMin
            // 
            this.yAxisMin.AutoSize = true;
            this.yAxisMin.Location = new System.Drawing.Point(339, 48);
            this.yAxisMin.Name = "yAxisMin";
            this.yAxisMin.Size = new System.Drawing.Size(46, 17);
            this.yAxisMin.TabIndex = 8;
            this.yAxisMin.Text = "y-min:";
            // 
            // logYScaleBase
            // 
            this.logYScaleBase.Enabled = false;
            this.logYScaleBase.Location = new System.Drawing.Point(274, 46);
            this.logYScaleBase.Name = "logYScaleBase";
            this.logYScaleBase.Size = new System.Drawing.Size(33, 22);
            this.logYScaleBase.TabIndex = 7;
            this.logYScaleBase.Text = "10";
            this.logYScaleBase.TextChanged += new System.EventHandler(this.logYScaleBase_TextChanged);
            // 
            // logYScale
            // 
            this.logYScale.AutoSize = true;
            this.logYScale.Location = new System.Drawing.Point(176, 46);
            this.logYScale.Name = "logYScale";
            this.logYScale.Size = new System.Drawing.Size(92, 21);
            this.logYScale.TabIndex = 6;
            this.logYScale.Text = "Log base:";
            this.logYScale.UseVisualStyleBackColor = true;
            // 
            // linearYScale
            // 
            this.linearYScale.AutoSize = true;
            this.linearYScale.Checked = true;
            this.linearYScale.Location = new System.Drawing.Point(101, 46);
            this.linearYScale.Name = "linearYScale";
            this.linearYScale.Size = new System.Drawing.Size(69, 21);
            this.linearYScale.TabIndex = 5;
            this.linearYScale.TabStop = true;
            this.linearYScale.Text = "Linear";
            this.linearYScale.UseVisualStyleBackColor = true;
            this.linearYScale.CheckedChanged += new System.EventHandler(this.linearYScale_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "y-Axis scale:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "x-Axis type:";
            // 
            // axisTypeComboBox
            // 
            this.axisTypeComboBox.FormattingEnabled = true;
            this.axisTypeComboBox.Items.AddRange(new object[] {
            "Scan number",
            "Retention time (min)"});
            this.axisTypeComboBox.Location = new System.Drawing.Point(101, 10);
            this.axisTypeComboBox.Name = "axisTypeComboBox";
            this.axisTypeComboBox.Size = new System.Drawing.Size(206, 24);
            this.axisTypeComboBox.TabIndex = 2;
            this.axisTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.axisTypeComboBox_SelectedIndexChanged);
            // 
            // ParseDataViz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1089, 647);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1107, 694);
            this.Name = "ParseDataViz";
            this.Text = "ParseDataViz";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OxyPlot.WindowsForms.PlotView plotView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private PresentationControls.CheckBoxComboBox checkBoxComboBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox axisTypeComboBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox logYScaleBase;
        private System.Windows.Forms.RadioButton logYScale;
        private System.Windows.Forms.RadioButton linearYScale;
        private System.Windows.Forms.Label label3;
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label exportWidth;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.RadioButton exportAsSvg;
        private System.Windows.Forms.RadioButton exportAsPdf;
        private System.Windows.Forms.TextBox exportHeightValue;
        private System.Windows.Forms.TextBox exportWidthValue;
        private System.Windows.Forms.Label exportHeight;
    }
}

