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

            

            comboBoxMinCharge.SelectedIndexChanged -= new System.EventHandler(comboBoxMinCharge_SelectedIndexChanged);
            comboBoxMaxCharge.SelectedIndexChanged -= new System.EventHandler(comboBoxMaxCharge_SelectedIndexChanged);

            comboBoxMinCharge.SelectedIndex = 1;
            previousMinChargeIndex = comboBoxMinCharge.SelectedIndex;
            comboBoxMaxCharge.SelectedIndex = 3;
            previousMaxChargeIndex = comboBoxMaxCharge.SelectedIndex;

            comboBoxMinCharge.SelectedIndexChanged += new System.EventHandler(comboBoxMinCharge_SelectedIndexChanged);
            comboBoxMaxCharge.SelectedIndexChanged += new System.EventHandler(comboBoxMaxCharge_SelectedIndexChanged);
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
    }
}
