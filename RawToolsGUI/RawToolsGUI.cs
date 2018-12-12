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
        public RawToolsGUI()
        {
            InitializeComponent();
        }

        private void buttonSelectFiles_Click(object sender, EventArgs e)
        {
            VistaOpenFileDialog dlg = new VistaOpenFileDialog();
            dlg.Multiselect = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtbxRawFiles.Text = String.Join(" ", dlg.FileNames);
            }
        }

        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
            dlg.ShowNewFolderButton = false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtbxRawFileDirectory.Text = dlg.SelectedPath;
            }


        }
    }
}
