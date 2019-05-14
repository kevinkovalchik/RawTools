namespace RawToolsViz
{
    partial class Launcher
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
            this.tableLayoutPanelStartScreen = new System.Windows.Forms.TableLayoutPanel();
            this.loadQcButton = new System.Windows.Forms.Button();
            this.loadParseButton = new System.Windows.Forms.Button();
            this.loadChroButton = new System.Windows.Forms.Button();
            this.loadRawButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.tableLayoutPanelStartScreen.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelStartScreen
            // 
            this.tableLayoutPanelStartScreen.ColumnCount = 3;
            this.tableLayoutPanelStartScreen.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanelStartScreen.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelStartScreen.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanelStartScreen.Controls.Add(this.loadQcButton, 1, 1);
            this.tableLayoutPanelStartScreen.Controls.Add(this.loadParseButton, 1, 2);
            this.tableLayoutPanelStartScreen.Controls.Add(this.loadChroButton, 1, 3);
            this.tableLayoutPanelStartScreen.Controls.Add(this.loadRawButton, 1, 4);
            this.tableLayoutPanelStartScreen.Controls.Add(this.exitButton, 1, 5);
            this.tableLayoutPanelStartScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelStartScreen.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelStartScreen.Name = "tableLayoutPanelStartScreen";
            this.tableLayoutPanelStartScreen.RowCount = 7;
            this.tableLayoutPanelStartScreen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanelStartScreen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.00032F));
            this.tableLayoutPanelStartScreen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.00032F));
            this.tableLayoutPanelStartScreen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.00032F));
            this.tableLayoutPanelStartScreen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.99872F));
            this.tableLayoutPanelStartScreen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.00032F));
            this.tableLayoutPanelStartScreen.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanelStartScreen.Size = new System.Drawing.Size(332, 459);
            this.tableLayoutPanelStartScreen.TabIndex = 0;
            // 
            // loadQcButton
            // 
            this.loadQcButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadQcButton.Location = new System.Drawing.Point(18, 18);
            this.loadQcButton.Margin = new System.Windows.Forms.Padding(6);
            this.loadQcButton.Name = "loadQcButton";
            this.loadQcButton.Size = new System.Drawing.Size(296, 75);
            this.loadQcButton.TabIndex = 0;
            this.loadQcButton.Text = "Load QC file";
            this.loadQcButton.UseVisualStyleBackColor = true;
            this.loadQcButton.Click += new System.EventHandler(this.loadQcButton_Click);
            // 
            // loadParseButton
            // 
            this.loadParseButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadParseButton.Location = new System.Drawing.Point(18, 105);
            this.loadParseButton.Margin = new System.Windows.Forms.Padding(6);
            this.loadParseButton.Name = "loadParseButton";
            this.loadParseButton.Size = new System.Drawing.Size(296, 75);
            this.loadParseButton.TabIndex = 1;
            this.loadParseButton.Text = "Load parse results file";
            this.loadParseButton.UseVisualStyleBackColor = true;
            // 
            // loadChroButton
            // 
            this.loadChroButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadChroButton.Location = new System.Drawing.Point(18, 192);
            this.loadChroButton.Margin = new System.Windows.Forms.Padding(6);
            this.loadChroButton.Name = "loadChroButton";
            this.loadChroButton.Size = new System.Drawing.Size(296, 75);
            this.loadChroButton.TabIndex = 2;
            this.loadChroButton.Text = "Load chromatogram file";
            this.loadChroButton.UseVisualStyleBackColor = true;
            // 
            // loadRawButton
            // 
            this.loadRawButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadRawButton.Location = new System.Drawing.Point(18, 279);
            this.loadRawButton.Margin = new System.Windows.Forms.Padding(6);
            this.loadRawButton.Name = "loadRawButton";
            this.loadRawButton.Size = new System.Drawing.Size(296, 74);
            this.loadRawButton.TabIndex = 3;
            this.loadRawButton.Text = "Load Thermo .raw file";
            this.loadRawButton.UseVisualStyleBackColor = true;
            // 
            // exitButton
            // 
            this.exitButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exitButton.Location = new System.Drawing.Point(18, 365);
            this.exitButton.Margin = new System.Windows.Forms.Padding(6);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(296, 75);
            this.exitButton.TabIndex = 4;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 459);
            this.Controls.Add(this.tableLayoutPanelStartScreen);
            this.Name = "Launcher";
            this.Text = "RawToolsVizLauncher";
            this.tableLayoutPanelStartScreen.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelStartScreen;
        private System.Windows.Forms.Button loadQcButton;
        private System.Windows.Forms.Button loadParseButton;
        private System.Windows.Forms.Button loadChroButton;
        private System.Windows.Forms.Button loadRawButton;
        private System.Windows.Forms.Button exitButton;
    }
}