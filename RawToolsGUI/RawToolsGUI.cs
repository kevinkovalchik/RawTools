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
            if (checkBox1.Checked)
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
                checkBoxReporterFilterMGF.Enabled = true;
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

        private void button1_Click(object sender, EventArgs e)
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
                MessageBox.Show("Sorry, I haven't finished the part of this that makes QC go yet.");
                return;
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

            if (radioButtonSelectDirectory.Checked)
            {
                if (textBoxRawFileDirectory.Text == "" | textBoxRawFileDirectory.Text == String.Empty)
                {
                    MessageBox.Show("Please select one or more raw files or a raw file directory.", "Error");
                    return;
                }
                arguments.Append($" -d {textBoxRawFileDirectory.Text}");
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
                foreach (var control in ckbxOutputQuant.Parent.Controls.OfType<Control>())
                {
                    control.Enabled = true;
                }
                checkBoxReporterFilterMGF.Enabled = ckbxOutputMGF.Checked;
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
