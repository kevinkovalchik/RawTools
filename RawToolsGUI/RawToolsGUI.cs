using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace RawToolsGUI
{
    public partial class RawToolsGUI : Form
    {
        int previousMinChargeIndex;
        int previousMaxChargeIndex;

        public RawToolsGUI()
        {
            InitializeComponent();

            peptideModifications = new PeptideModifications();
            //testDialog = new PeptideModificationForm();

            //comboBoxMinCharge.SelectedIndexChanged -= new System.EventHandler(comboBoxMinCharge_SelectedIndexChanged);
            //comboBoxMaxCharge.SelectedIndexChanged -= new System.EventHandler(comboBoxMaxCharge_SelectedIndexChanged);

            comboBoxMaxCharge.SelectedIndex = 3;
            previousMaxChargeIndex = comboBoxMaxCharge.SelectedIndex;
            comboBoxMinCharge.SelectedIndex = 1;
            previousMinChargeIndex = comboBoxMinCharge.SelectedIndex;


            //comboBoxMinCharge.SelectedIndexChanged += new System.EventHandler(comboBoxMinCharge_SelectedIndexChanged);
            //comboBoxMaxCharge.SelectedIndexChanged += new System.EventHandler(comboBoxMaxCharge_SelectedIndexChanged);

            comboBoxLabelingReagents.SelectedIndex = 0;
        }

        private void buttonSelectFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "*.raw|*.raw";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxRawFiles.Text = String.Join(" ", dlg.FileNames);
            }
        }

        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowser.FolderSelectDialog dlg = new FolderBrowser.FolderSelectDialog();
            dlg.Title = "Select raw file directory";

            //dlg.ShowNewFolderButton = false;

            if (dlg.ShowDialog())
            {
                textBoxRawFileDirectory.Text = dlg.FileName;
            }
        }

        private void buttonQcDataDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowser.FolderSelectDialog dlg = new FolderBrowser.FolderSelectDialog();
            dlg.Title = "Select QC directory";

            if (dlg.ShowDialog())
            {
                textBoxQcDataDirectory.Text = dlg.FileName;
            }
        }

        private void buttonXTandemDir_Click(object sender, EventArgs e)
        {
            FolderBrowser.FolderSelectDialog dlg = new FolderBrowser.FolderSelectDialog();
            dlg.Title = "Select X! Tandem directory";

            if (dlg.ShowDialog())
            {
                textBoxXTandemDir.Text = dlg.FileName;
            }
        }

        private void buttonFastaFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxFastaFile.Text = dlg.FileName;
            }
        }

        private void buttonDataOutputDir_Click(object sender, EventArgs e)
        {
            FolderBrowser.FolderSelectDialog dlg = new FolderBrowser.FolderSelectDialog();
            dlg.Title = "Select parse output directory";

            if (dlg.ShowDialog())
            {
                textBoxDataOutputDir.Text = dlg.FileName;
            }
        }

        private void radioButtonSearchNone_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSearchNone.Checked)
            {
                buttonFastaFile.Enabled = false;
                buttonXTandemDir.Enabled = false;

                textBoxFastaFile.Enabled = false;
                textBoxXTandemDir.Enabled = false;

                labelNumSpectra.Enabled = false;
                textBoxNumSpectra.Enabled = false;

                labelPeptideMods.Enabled = false;
                buttonPeptideMods.Enabled = false;
            }
        }

        private void radioButtonSearchXTandem_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSearchXTandem.Checked)
            {
                buttonFastaFile.Enabled = true;
                buttonXTandemDir.Enabled = true;

                textBoxFastaFile.Enabled = true;
                textBoxXTandemDir.Enabled = true;

                labelNumSpectra.Enabled = true;
                textBoxNumSpectra.Enabled = true;

                labelPeptideMods.Enabled = true;
                buttonPeptideMods.Enabled = true;
            }
        }
        
        private void checkBoxModeParse_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxModeParse.Checked)
            {
                groupBoxDataOutput.Enabled = true;
                groupBoxCommonOptions.Enabled = true;
            }
            else
            {
                groupBoxDataOutput.Enabled = false;
                groupBoxCommonOptions.Enabled = false;
            }
        }

        private void checkBoxModeQC_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxModeQC.Checked)
            {
                radioButtonSelectDirectory.Checked = true;
                radioButtonSelectDirectory.Enabled = true;
                buttonSelectDirectory.Enabled = true;
                textBoxRawFileDirectory.Enabled = true;
                groupBoxCommonOptions.Enabled = true;
                radioButtonSelectFiles.Checked = false;
                radioButtonSelectFiles.Enabled = false;
                buttonSelectFiles.Enabled = false;
                textBoxRawFiles.Enabled = false;

                groupBoxQcOptions.Enabled = true;
            }
            else if (!checkBoxModeQC.Checked)
            {
                radioButtonSelectDirectory.Checked = true;
                radioButtonSelectDirectory.Enabled = true;
                buttonSelectDirectory.Enabled = true;
                textBoxRawFileDirectory.Enabled = true;
                groupBoxCommonOptions.Enabled = false;
                radioButtonSelectFiles.Checked = false;
                radioButtonSelectFiles.Enabled = true;
                buttonSelectFiles.Enabled = false;
                textBoxRawFiles.Enabled = false;

                groupBoxQcOptions.Enabled = false;
            }
        }

        private void radioButtonSelectDirectory_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSelectDirectory.Checked)
            {
                buttonSelectDirectory.Enabled = true;
                textBoxRawFileDirectory.Enabled = true;

                buttonSelectFiles.Enabled = false;
                textBoxRawFiles.Enabled = false;
            }
            else
            {
                buttonSelectDirectory.Enabled = false;
                textBoxRawFileDirectory.Enabled = false;

                buttonSelectFiles.Enabled = true;
                textBoxRawFiles.Enabled = true;
            }
        }

        private void checkBoxRefinePrecursor_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRefinePrecursor.Checked)
            {
                labelMinCharge.Enabled = true;
                labelMaxCharge.Enabled = true;
                comboBoxMaxCharge.Enabled = true;
                comboBoxMinCharge.Enabled = true;
            }
            else
            {
                labelMaxCharge.Enabled = false;
                labelMinCharge.Enabled = false;
                comboBoxMaxCharge.Enabled = false;
                comboBoxMinCharge.Enabled = false;
            }
        }

        private void comboBoxMinCharge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMinCharge.SelectedIndex > comboBoxMaxCharge.SelectedIndex)
            {
                MessageBox.Show("Sorry, minimun considered charge must be smaller than maximum considered charge. It will be reset.", "Warning");
                comboBoxMinCharge.SelectedIndex = previousMinChargeIndex;
            }
        }

        private void comboBoxMaxCharge_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMinCharge.SelectedIndex > comboBoxMaxCharge.SelectedIndex)
            {
                MessageBox.Show("Sorry, minimun considered charge must be smaller than maximum considered charge. It will be reset.", "Warning");
                comboBoxMaxCharge.SelectedIndex = previousMaxChargeIndex;
            }
        }

        private void comboBoxMinCharge_Enter(object sender, EventArgs e)
        {
            previousMinChargeIndex = comboBoxMinCharge.SelectedIndex;
        }

        private void comboBoxMaxCharge_Enter(object sender, EventArgs e)
        {
            previousMaxChargeIndex = comboBoxMaxCharge.SelectedIndex;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDataOutputDirectory.Checked)
            {
                buttonDataOutputDir.Enabled = true;
                textBoxDataOutputDir.Enabled = true;
            }
            else
            {
                buttonDataOutputDir.Enabled = false;
                textBoxDataOutputDir.Enabled = false;
            }
        }

        private void ckbxOutputChromatograms_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbxOutputChromatograms.Checked)
            {
                groupBoxChromatograms.Enabled = true;
            }
            else
            {
                groupBoxChromatograms.Enabled = false;
            }
        }

        private void ckbxOutputMGF_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbxOutputMGF.Checked)
            {
                checkBoxReporterFilterMGF.Enabled = checkBoxReporterIonFiltering.Checked;
                groupBoxMgfOpts.Enabled = true;
                ckbxOutputParse.Checked = true;
            }
            else
            {
                checkBoxReporterFilterMGF.Enabled = false;
                groupBoxMgfOpts.Enabled = false;
            }
        }

        private void ckbxOutputFaims_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFaims.Checked)
            {
                checkBoxReporterFilterMGF.Enabled = checkBoxReporterIonFiltering.Checked;
                groupBoxMgfOpts.Enabled = true;
            }
            else
            {
                checkBoxReporterFilterMGF.Enabled = false;
                groupBoxMgfOpts.Enabled = false;
            }
        }

        private void textBoxReporterIntensityFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            // can only be numerical values
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != 'e'))
            {
                e.Handled = true;
            }

            // only allow one decimal point and one e
            if (((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1)) | ((e.KeyChar == 'e') && ((sender as TextBox).Text.IndexOf('e') > -1)))
            {
                e.Handled = true;
            }
        }

        private void textBoxReporterNumberMissingFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            // can only be numerical values
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxMgfLowMass_KeyPress(object sender, KeyPressEventArgs e)
        {
            // can only be numerical values
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxMgfFilterRelativeIntensity_KeyPress(object sender, KeyPressEventArgs e)
        {
            // can only be numerical values
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxMgfFilterWindowSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            // can only be numerical values
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxNumSpectra_KeyPress(object sender, KeyPressEventArgs e)
        {
            // can only be integer values
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void comboBoxMinCharge_KeyPress(object sender, KeyPressEventArgs e)
        {
            // can only be integer values
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void comboBoxMaxCharge_KeyPress(object sender, KeyPressEventArgs e)
        {
            // can only be integer values
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            string command = String.Empty;
            StringBuilder arguments = new StringBuilder();

            if (Environment.OSVersion.Platform == PlatformID.MacOSX | Environment.OSVersion.Platform == PlatformID.Unix)
            {
                command = "mono";
                arguments.Append("RawTools.exe");
            }
            else
            {
                command = "RawTools.exe";
            }
                        
            if (radioButtonSelectDirectory.Checked)
            {
                if (textBoxRawFileDirectory.Text == "" | textBoxRawFileDirectory.Text == String.Empty)
                {
                    MessageBox.Show("Please select one or more raw files or a raw file directory.", "Error");
                    return;
                }
                arguments.Append($" -d \"{textBoxRawFileDirectory.Text}\"");
            }
            else if (radioButtonSelectFiles.Checked)
            {
                if (textBoxRawFiles.Text == "" | textBoxRawFiles.Text == String.Empty)
                {
                    MessageBox.Show("Please select one or more raw files or a raw file directory.", "Error");
                    return;
                }
                arguments.Append($" -f {textBoxRawFiles.Text}");
            }
            else
            {
                MessageBox.Show("Something went wrong... please select a raw file directory or one or more raw files.", "Error");
                return;
            }

            if (checkBoxModeParse.Checked)
            {

                if (!ckbxOutputMGF.Checked & !ckbxOutputMetrics.Checked & !ckbxOutputChromatograms.Checked &
                    !ckbxOutputParse.Checked & !ckbxOutputQuant.Checked & !checkBoxModeQC.Checked & !checkBoxFaims.Checked)
                {
                    MessageBox.Show("You haven't selected any data output. Please choose something.");
                    return;
                }

                arguments.Append(" -");

                if (ckbxOutputMGF.Checked)
                {
                    arguments.Append("m");
                }
                if (ckbxOutputParse.Checked)
                {
                    arguments.Append("p");
                }
                if (ckbxOutputMetrics.Checked)
                {
                    arguments.Append("x");
                }

                if (checkBoxFaims.Checked)
                {
                    arguments.Append(" -faimsMgf");
                }

                if (checkBoxDataOutputDirectory.Checked)
                {
                    arguments.Append($" -o \"{textBoxDataOutputDir.Text}\"");
                }

                if (ckbxOutputQuant.Checked & ckbxOutputQuant.Enabled)
                {
                    if (comboBoxLabelingReagents.SelectedIndex == 0)
                    {
                        MessageBox.Show("In order to quantify reporter ions you need to select the appropriate labeling reagents.", "Error");
                        return;
                    }
                    arguments.Append($" -q -r {comboBoxLabelingReagents.Text}");
                }

                if (checkBoxMgfLowMass.Checked & checkBoxMgfLowMass.Enabled)
                {
                    arguments.Append($" -c {textBoxMgfLowMass.Text}");
                }
                
                if (ckbxOutputChromatograms.Checked)
                {
                    if (checkBoxChroMs1.Checked | checkBoxChroMs2.Checked | checkBoxChroTIC.Checked | checkBoxChroBP.Checked)
                    {
                        arguments.Append(" -chro ");
                    }

                    if (checkBoxChroMs1.Checked) arguments.Append("1");
                    if (checkBoxChroMs2.Checked) arguments.Append("2");
                    if (checkBoxChroTIC.Checked) arguments.Append("T");
                    if (checkBoxChroBP.Checked) arguments.Append("B");
                }
            }

            if (checkBoxModeQC.Checked)
            {
                arguments.Append($" -qc \"{textBoxQcDataDirectory.Text}\"");

                if (!radioButtonSearchNone.Checked)
                {
                    if (radioButtonSearchXTandem.Checked)
                    {
                        if (textBoxXTandemDir.Text == "" | textBoxXTandemDir.Text == String.Empty)
                        {
                            MessageBox.Show("Please select the X! Tandem directory.", "Error");
                            return;
                        }
                        arguments.Append($" -X \"{textBoxXTandemDir.Text}\"");
                    }

                    if (textBoxFastaFile.Text == "" | textBoxFastaFile.Text == String.Empty)
                    {
                        MessageBox.Show("Please select a FASTA file for the database search.", "Error");
                        return;
                    }
                    arguments.Append($" -db \"{textBoxFastaFile.Text}\"");
                    
                    if ((from x in peptideModifications.Mods where x.Fixed select 1).Sum() > 0)
                    {
                        arguments.Append($" -fmods {peptideModifications.FModsString}");
                    }

                    if ((from x in peptideModifications.Mods where !x.Fixed select 1).Sum() > 0)
                    {
                        arguments.Append($" -vmods {peptideModifications.VModsString}");
                    }

                    arguments.Append($" -N {textBoxNumSpectra.Text}");
                }
            }

            if (checkBoxRefinePrecursor.Checked)
            {
                arguments.Append(" -R");

                if (comboBoxMinCharge.Text != "" & comboBoxMinCharge.Text != string.Empty)
                {
                    arguments.Append($" -mincharge {comboBoxMinCharge.Text}");
                }

                if (comboBoxMaxCharge.Text != "" & comboBoxMaxCharge.Text != string.Empty)
                {
                    arguments.Append($" -maxcharge {comboBoxMaxCharge.Text}");
                }
            }

            utils.VoidBash(command, arguments.ToString());
        }

        private void comboBoxLabelingReagents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLabelingReagents.SelectedIndex == 8)
            {
                MessageBox.Show("Sorry, custom reagents aren't implemented yet.", "Info");
                comboBoxLabelingReagents.SelectedIndex = 0;
            }
        }

        private void comboBoxLabelingReagents_Enter(object sender, EventArgs e)
        {
        }

        private void ckbxOutputParse_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbxOutputParse.Checked)
            {
                groupBoxQuantOpt.Enabled = true;
            }
            else
            {
                groupBoxQuantOpt.Enabled = false;
                ckbxOutputMGF.Checked = false;
                checkBoxFaims.Checked = false;
            }
        }

        private void ckbxOutputQuant_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbxOutputQuant.Checked)
            {
                comboBoxLabelingReagents.Enabled = true;
                checkBoxReporterIonFiltering.Enabled = true;

                labelReporterIonIntensityFilter.Enabled = checkBoxReporterIonFiltering.Checked;
                textBoxReporterIntensityFilter.Enabled = checkBoxReporterIonFiltering.Checked;
                labelReporterIonMissingFilter.Enabled = checkBoxReporterIonFiltering.Checked;
                textBoxReporterNumberMissingFilter.Enabled = checkBoxReporterIonFiltering.Checked;
                checkBoxReporterFilterMatrix.Enabled = checkBoxReporterIonFiltering.Checked;
                checkBoxReporterFilterMGF.Enabled = checkBoxReporterIonFiltering.Checked & ckbxOutputMGF.Checked;
                labelReporterIonFilteringApplyTo.Enabled = checkBoxReporterIonFiltering.Checked;
            }
            else
            {
                foreach (var control in ckbxOutputQuant.Parent.Controls.OfType<Control>())
                {
                    control.Enabled = false;
                }
                ckbxOutputQuant.Enabled = true;
            }
        }

        private void textBoxMgfLowMass_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxMgfLowMass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMgfLowMass.Checked)
            {
                textBoxMgfLowMass.Enabled = true;
            }
            else
            {
                textBoxMgfLowMass.Enabled = false;
            }
        }

        private void checkBoxReporterIonFiltering_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxReporterIonFiltering.Checked)
            {
                labelReporterIonIntensityFilter.Enabled = true;
                labelReporterIonMissingFilter.Enabled = true;
                textBoxReporterIntensityFilter.Enabled = true;
                textBoxReporterNumberMissingFilter.Enabled = true;
                labelReporterIonFilteringApplyTo.Enabled = true;
                checkBoxReporterFilterMatrix.Enabled = true;
                checkBoxReporterFilterMGF.Enabled = ckbxOutputMGF.Checked;
            }
            else
            {
                labelReporterIonIntensityFilter.Enabled = false;
                labelReporterIonMissingFilter.Enabled = false;
                textBoxReporterIntensityFilter.Enabled = false;
                textBoxReporterNumberMissingFilter.Enabled = false;
                labelReporterIonFilteringApplyTo.Enabled = false;
                checkBoxReporterFilterMatrix.Enabled = false;
                checkBoxReporterFilterMGF.Enabled = false;
            }
        }

        private void buttonPeptideMods_Click(object sender, EventArgs e)
        {
            //PeptideModifications backupMods = peptideModifications.Copy();

            PeptideModificationForm form = new PeptideModificationForm();
            form.PopulateTable(peptideModifications);

            if (form.ShowDialog() == DialogResult.OK)
            {
                peptideModifications.UpdateModifications(form.dataGridViewModifications);
            }
        }

        private void toolStripMenuItemNewParameters_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void reset()
        {
            foreach (Control x in groupBoxChromatograms.Controls) if (x is CheckBox) ((CheckBox)x).Checked = false;

            foreach (Control x in groupBoxRawFiles.Controls) if (x is TextBox) ((TextBox)x).Clear();

            radioButtonSelectDirectory.Checked = true;

            textBoxDataOutputDir.Clear();
            
            checkBoxMgfLowMass.Checked = false;

            foreach (Control x in groupBoxMode.Controls) if (x is CheckBox) ((CheckBox)x).Checked = false;

            checkBoxRefinePrecursor.Checked = true;

            comboBoxMinCharge.Text = "0";
            comboBoxMaxCharge.Text = "4";
            comboBoxMinCharge.Text = "2";

            foreach (Control x in groupBoxDataOutput.Controls) if (x is CheckBox) ((CheckBox)x).Checked = false;

            radioButtonSearchNone.Checked = true;

            foreach (Control x in groupBoxQcOptions.Controls) if (x is TextBox) ((TextBox)x).Clear();

            textBoxNumSpectra.Text = "10000";

            peptideModifications = new PeptideModifications();

            foreach (Control x in groupBoxQuantOpt.Controls) if (x is CheckBox) ((CheckBox)x).Checked = false;

            foreach (Control x in groupBoxQuantOpt.Controls) if (x is TextBox) ((TextBox)x).Text = "0";

            comboBoxLabelingReagents.Text = "- select -";

            foreach (Control x in groupBoxMgfOpts.Controls) if (x is CheckBox) ((CheckBox)x).Checked = false;

            foreach (Control x in groupBoxMgfOpts.Controls) if (x is TextBox) ((TextBox)x).Text = "0";
        }

        private void LoadParameters(ParametersContainer Pars)
        {
            checkBoxChroBP.Checked = Pars.ChromatogramBP;
            checkBoxChroMs1.Checked = Pars.ChromatogramMs1;
            checkBoxChroMs2.Checked = Pars.ChromatogramMs2;
            checkBoxChroTIC.Checked = Pars.ChromatogramTIC;

            textBoxRawFileDirectory.Text = Pars.RawFileDirectory;
            textBoxRawFiles.Text = Pars.RawFileList;

            radioButtonSelectDirectory.Checked = Pars.UseRawFileDirectory;

            textBoxDataOutputDir.Text = Pars.DataOutputDirectoryString;
            
            checkBoxMgfLowMass.Checked = Pars.MGFLowMassCutoff;

            checkBoxModeParse.Checked = Pars.ParseMode;

            checkBoxModeQC.Checked = Pars.QcMode;

            checkBoxRefinePrecursor.Checked = Pars.RefinePrecursorMassCharge;

            comboBoxMinCharge.Text = "0";
            comboBoxMaxCharge.Text = "100";
            comboBoxMinCharge.Text = Pars.MinCharge;
            comboBoxMaxCharge.Text = Pars.MaxCharge;

            ckbxOutputChromatograms.Checked = Pars.DataOutputChromatograms;
            ckbxOutputMetrics.Checked = Pars.DataOutputMetrics;
            ckbxOutputMGF.Checked = Pars.DataOutputMGF;
            checkBoxFaims.Checked = Pars.FaimsOutputMgf;
            ckbxOutputParse.Checked = Pars.DataOutputParseMatrix;
            checkBoxDataOutputDirectory.Checked = Pars.DataOutputDirectory;
            
            radioButtonSearchNone.Checked = Pars.SearchEngineNone;
            radioButtonSearchXTandem.Checked = Pars.SearchEngineXTandem;

            textBoxQcDataDirectory.Text = Pars.QcDataDirectory;
            textBoxXTandemDir.Text = Pars.XTandemDirectory;
            textBoxFastaFile.Text = Pars.FastaFile;

            textBoxNumSpectra.Text = Pars.NumberSpectraToSearch;

            peptideModifications = Pars.PeptideModifications;

            ckbxOutputQuant.Checked = Pars.QuantifyReporterIons;
            comboBoxLabelingReagents.Text = Pars.Reagents;

            checkBoxMgfLowMass.Checked = Pars.MGFLowMassCutoff;

            textBoxMgfLowMass.Text = Pars.MGFLowMassCutoffValue;
        }

        private ParametersContainer GetOutParameters()
        {
            ParametersContainer Pars = new ParametersContainer();
            
            Pars.ChromatogramBP = checkBoxChroBP.Checked;
            Pars.ChromatogramMs1 = checkBoxChroMs1.Checked;
            Pars.ChromatogramMs2 = checkBoxChroMs2.Checked;
            Pars.ChromatogramTIC = checkBoxChroTIC.Checked;

            Pars.RawFileDirectory = textBoxRawFileDirectory.Text;
            Pars.RawFileList = textBoxRawFiles.Text;

            Pars.UseRawFileDirectory = radioButtonSelectDirectory.Checked;

            Pars.DataOutputDirectoryString = textBoxDataOutputDir.Text;

            Pars.MGFLowMassCutoff = checkBoxMgfLowMass.Checked;

            Pars.ParseMode = checkBoxModeParse.Checked;

            Pars.QcMode = checkBoxModeQC.Checked;

            Pars.RefinePrecursorMassCharge = checkBoxRefinePrecursor.Checked;
            
            Pars.MinCharge = comboBoxMinCharge.Text;
            Pars.MaxCharge = comboBoxMaxCharge.Text;

            Pars.DataOutputChromatograms = ckbxOutputChromatograms.Checked;
            Pars.DataOutputMetrics = ckbxOutputMetrics.Checked;
            Pars.DataOutputMGF = ckbxOutputMGF.Checked;
            Pars.FaimsOutputMgf = checkBoxFaims.Checked;
            Pars.DataOutputParseMatrix = ckbxOutputParse.Checked;
            Pars.DataOutputDirectory = checkBoxDataOutputDirectory.Checked;

            Pars.SearchEngineNone = radioButtonSearchNone.Checked;
            Pars.SearchEngineXTandem = radioButtonSearchXTandem.Checked;

            Pars.QcDataDirectory = textBoxQcDataDirectory.Text;
            Pars.XTandemDirectory = textBoxXTandemDir.Text;
            Pars.FastaFile = textBoxFastaFile.Text;

            Pars.NumberSpectraToSearch = textBoxNumSpectra.Text;

            Pars.PeptideModifications = peptideModifications;

            Pars.QuantifyReporterIons = ckbxOutputQuant.Checked;
            Pars.Reagents = comboBoxLabelingReagents.Text;

            Pars.MGFLowMassCutoff = checkBoxMgfLowMass.Checked;

            Pars.MGFLowMassCutoffValue = textBoxMgfLowMass.Text;

            return Pars;
        }

        private void toolStripMenuItemOpenParameters_Click(object sender, EventArgs e)
        {
            OpenFileDialog openParameters = new OpenFileDialog();
            openParameters.Filter = "RawTools Parameters|*.rtxml";
            openParameters.Title = "Load RawTools Parameters";
            openParameters.ShowDialog();

            if (openParameters.FileName != "")
            {
                var serializer = new XmlSerializer(typeof(ParametersContainer));
                using (StreamReader reader = new StreamReader(openParameters.FileName))
                {
                    ParametersContainer Pars = (ParametersContainer)serializer.Deserialize(reader);
                    LoadParameters(Pars);
                }
            }
        }

        private void toolStripMenuItemSaveParameters_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveParameters = new SaveFileDialog();
            ParametersContainer Pars = GetOutParameters();
            saveParameters.Filter = "RawTools Parameters|*.rtxml";
            saveParameters.Title = "Save RawTools Parameters";
            saveParameters.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (saveParameters.FileName != "")
            {
                var serializer = new XmlSerializer(typeof(ParametersContainer));
                using (StreamWriter writer = new StreamWriter(saveParameters.FileName))
                {
                    serializer.Serialize(writer, Pars);
                }
            }
        }

        private void exitToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            if (ConfirmExit())
            {
                Environment.Exit(0);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (ConfirmExit() == false)
            {
                e.Cancel = true;
            };
        }

        private bool ConfirmExit()
        {
            const string message = "Are you sure you wish to exit?";
            const string caption = "Exit";
            var result = MessageBox.Show(message, caption,
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                return true;
            else
                return false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutBox = new AboutBox1();
            aboutBox.ShowDialog();
        }
    }

    static class utils
    {
        public static void VoidBash(string cmd, string args)
        {

            // run a string as a process, return void
            // thanks to https://loune.net/2017/06/running-shell-bash-commands-in-net-core/ for this code.

            var escapedArgs = args.Replace("\"", "\\\"");
            Process process;
            process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = args,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };
            
            process.Start();
            return;
        }
    }
}
