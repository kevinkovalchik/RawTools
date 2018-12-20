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
using System.Diagnostics;
using Ookii.Dialogs.WinForms;

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
            VistaOpenFileDialog dlg = new VistaOpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "*.raw|*.raw";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxRawFiles.Text = String.Join(" ", dlg.FileNames);
            }
        }

        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxRawFileDirectory.Text = dlg.SelectedPath;
            }
        }

        private void buttonQcDataDirectory_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxQcDataDirectory.Text = dlg.SelectedPath;
            }
        }

        private void buttonXTandemDir_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxXTandemDir.Text = dlg.SelectedPath;
            }
        }

        private void buttonPythonExe_Click(object sender, EventArgs e)
        {
            VistaOpenFileDialog dlg = new VistaOpenFileDialog();
            dlg.Multiselect = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxPythonExe.Text = dlg.FileName;
            }
        }

        private void buttonIdentipyScript_Click(object sender, EventArgs e)
        {
            VistaOpenFileDialog dlg = new VistaOpenFileDialog();
            dlg.Multiselect = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxIdentipyScript.Text = dlg.FileName;
            }
        }

        private void buttonFastaFile_Click(object sender, EventArgs e)
        {
            VistaOpenFileDialog dlg = new VistaOpenFileDialog();
            dlg.Multiselect = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxFastaFile.Text = dlg.FileName;
            }
        }

        private void buttonDataOutputDir_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxDataOutputDir.Text = dlg.SelectedPath;
            }
        }

        private void radioButtonSearchNone_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSearchNone.Checked)
            {
                buttonFastaFile.Enabled = false;
                buttonXTandemDir.Enabled = false;
                buttonPythonExe.Enabled = false;
                buttonIdentipyScript.Enabled = false;

                textBoxFastaFile.Enabled = false;
                textBoxXTandemDir.Enabled = false;
                textBoxPythonExe.Enabled = false;
                textBoxIdentipyScript.Enabled = false;

                labelNumSpectra.Enabled = false;
                textBoxNumSpectra.Enabled = false;

                checkBoxAutoSearchIdentipy.Enabled = false;
            }
        }

        private void radioButtonSearchXTandem_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSearchXTandem.Checked)
            {
                buttonFastaFile.Enabled = true;
                buttonXTandemDir.Enabled = true;
                buttonPythonExe.Enabled = false;
                buttonIdentipyScript.Enabled = false;

                textBoxFastaFile.Enabled = true;
                textBoxXTandemDir.Enabled = true;
                textBoxPythonExe.Enabled = false;
                textBoxIdentipyScript.Enabled = false;

                labelNumSpectra.Enabled = true;
                textBoxNumSpectra.Enabled = true;

                checkBoxAutoSearchIdentipy.Enabled = false;
            }
        }

        private void radioButtonSearchIdentipy_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSearchIdentipy.Checked)
            {
                buttonFastaFile.Enabled = true;
                buttonXTandemDir.Enabled = false;

                textBoxFastaFile.Enabled = true;
                textBoxXTandemDir.Enabled = false;

                labelNumSpectra.Enabled = true;
                textBoxNumSpectra.Enabled = true;

                checkBoxAutoSearchIdentipy.Enabled = true;

                if (!checkBoxAutoSearchIdentipy.Checked)
                {
                    buttonPythonExe.Enabled = true;
                    buttonIdentipyScript.Enabled = true;

                    textBoxPythonExe.Enabled = true;
                    textBoxIdentipyScript.Enabled = true;
                }
            }
        }

        private void checkBoxAutoSearchIdentipy_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxAutoSearchIdentipy.Checked)
            {
                textBoxPythonExe.Enabled = true;
                textBoxIdentipyScript.Enabled = true;
                buttonPythonExe.Enabled = true;
                buttonIdentipyScript.Enabled = true;
            }
            else
            {
                textBoxPythonExe.Enabled = false;
                textBoxIdentipyScript.Enabled = false;
                buttonPythonExe.Enabled = false;
                buttonIdentipyScript.Enabled = false;
            }
        }

        private void checkBoxModeParse_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBoxModeQC_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxModeQC.Checked)
            {
                radioButtonSelectDirectory.Checked = true;
                radioButtonSelectDirectory.Enabled = true;
                buttonSelectDirectory.Enabled = true;
                textBoxRawFileDirectory.Enabled = true;

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

            if (checkBoxModeParse.Checked)
            {
                arguments.Append(" parse");
            }
            else if (checkBoxModeQC.Checked)
            {
                arguments.Append(" qc");
                //MessageBox.Show("Sorry, I haven't finished the part of this that makes QC go yet.");
                //return;
            }
            else
            {
                MessageBox.Show("You need to select a Mode for RawTools. Please pick QC or Parse.", "Info");
                return;
            }

            if (checkBoxModeParse.Checked & checkBoxModeQC.Checked)
            {
                MessageBox.Show("Sorry, I know this is lame... but you can't do QC and Parse at the same time yet... " +
                    "Just pick one or the other for now.", "Info");
                return;
            }

            if (checkBoxModeParse.Checked)
            {
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

                if (!ckbxOutputMGF.Checked & !ckbxOutputMetrics.Checked & !ckbxOutputChromatograms.Checked &
                    !ckbxOutputParse.Checked & !ckbxOutputQuant.Checked & !checkBoxModeQC.Checked)
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
                if (checkBoxRefinePrecursor.Checked)
                {
                    arguments.Append("R");
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
                    arguments.Append($" -c {textBoxMgfLowMass}");
                }

                if (checkBoxMgfIntensityFiltering.Checked & radioButtonMgfFilterRelativeIntensity.Checked)
                {
                    arguments.Append($" -y {textBoxMgfFilterRelativeIntensity}");
                }

                if (ckbxOutputChromatograms.Checked)
                {
                    if (checkBoxChroMs1.Checked | checkBoxChroMs2.Checked | checkBoxChroTIC.Checked | checkBoxChroBP.Checked)
                    {
                        arguments.Append(" --chro ");
                    }

                    if (checkBoxChroMs1.Checked) arguments.Append("1");
                    if (checkBoxChroMs2.Checked) arguments.Append("2");
                    if (checkBoxChroTIC.Checked) arguments.Append("T");
                    if (checkBoxChroBP.Checked) arguments.Append("B");
                }
            }

            if (checkBoxModeQC.Checked)
            {
                arguments.Append($" -d \"{textBoxRawFileDirectory.Text}\"");
                arguments.Append($" -q \"{textBoxQcDataDirectory.Text}\"");

                if (!radioButtonSearchNone.Checked)
                {
                    if (radioButtonSearchXTandem.Checked)
                    {
                        if (textBoxXTandemDir.Text == "" | textBoxXTandemDir.Text == String.Empty)
                        {
                            MessageBox.Show("Please select the X! Tandem directory.", "Error");
                            return;
                        }
                        arguments.Append($" -s xtandem -X \"{textBoxXTandemDir.Text}\"");
                    }
                    else
                    {
                        arguments.Append($" -s identipy");

                        if (!checkBoxAutoSearchIdentipy.Checked)
                        {
                            if (textBoxPythonExe.Text == "" | textBoxPythonExe.Text == String.Empty
                                | textBoxIdentipyScript.Text == "" | textBoxIdentipyScript.Text == String.Empty)
                            {
                                MessageBox.Show("Please select both the Python executable file and the Identipy script file.", "Error");
                                return;
                            }
                            arguments.Append($" -P \"{textBoxPythonExe.Text}\" -I \"{textBoxIdentipyScript.Text}\"");
                        }
                    }

                    if (textBoxFastaFile.Text == "" | textBoxFastaFile.Text == String.Empty)
                    {
                        MessageBox.Show("Please select a FASTA file for the database search.", "Error");
                        return;
                    }
                    arguments.Append($" --db \"{textBoxFastaFile.Text}\"");

                    if (peptideModifications.KMod.Use)
                    {
                        arguments.Append($" --kmod {peptideModifications.KModString}");
                    }

                    if (peptideModifications.NMod.Use)
                    {
                        arguments.Append($" --nmod {peptideModifications.NModString}");
                    }

                    if (peptideModifications.XMod.Use)
                    {
                        arguments.Append($" --xmod {peptideModifications.XModString}");
                    }

                    if ((from x in peptideModifications.FMods where x.Use select 1).Sum() > 0)
                    {
                        arguments.Append($" --fmods {peptideModifications.FModsString}");
                    }

                    arguments.Append($" -N {textBoxNumSpectra.Text}");
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

        private void checkBoxMgfIntensityFiltering_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMgfIntensityFiltering.Checked)
            {
                radioButtonMgfFilterRelativeIntensity.Enabled = true;
                radioButtonMgfIntensityFilterNoiseModel.Enabled = true;
                textBoxMgfFilterRelativeIntensity.Enabled = true;
            }
            else
            {
                radioButtonMgfFilterRelativeIntensity.Enabled = false;
                radioButtonMgfIntensityFilterNoiseModel.Enabled = false;
                textBoxMgfFilterRelativeIntensity.Enabled = false;
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

            //string result = string.Empty;
            /*
            process.Start();
            using (StreamReader reader = process.StandardOutput)
            {
                process.WaitForExit();
                string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                return result;
            }
            */
            process.Start();
            //string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return;
        }

        // The following is a placeholder for a function to open up the custom reporter ion form
        /*
        public void ShowMyDialogBox()
        {
            Form2 testDialog = new Form2();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (testDialog.ShowDialog(this) == DialogResult.OK)
            {
                // Read the contents of testDialog's TextBox.
                this.txtResult.Text = testDialog.TextBox1.Text;
            }
            else
            {
                this.txtResult.Text = "Cancelled";
            }
            testDialog.Dispose();
        }
        */
    }
}
