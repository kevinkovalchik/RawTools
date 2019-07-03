using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawToolsViz.HelpWindows
{
    public partial class RawfileVizHelp : Form
    {
        public RawfileVizHelp()
        {
            InitializeComponent();

            var defaultFont = helpText.SelectionFont;

            helpText.SelectionFont = new Font(defaultFont.FontFamily, 14, FontStyle.Bold);

            helpText.AppendText("RawFileViz Help\n\n");

            helpText.SelectionFont = new Font(defaultFont.FontFamily, 11, FontStyle.Regular);

            helpText.AppendText(
                "RawFileViz is a lightweight application for viewing Orbitrap RAW data files.\n\n" +
                "" +
                "The main screen of RawFileViz is divided into three sections: chromatogram, spectrum, and meta data. " +
                "The chromatogram displays a chromatogram trace of the data in the raw file, the spectrum displays a single mass " +
                "spectrum, and the meta data section can display either the trailer for a given scan, or the instrument status " +
                "log associated with the currently selected retention time.\n\n"
                );

            helpText.SelectionFont = new Font(defaultFont.FontFamily, 12, FontStyle.Bold);

            helpText.AppendText("Chromatogram\n\n");

            helpText.SelectionFont = new Font(defaultFont.FontFamily, 11, FontStyle.Regular);

            helpText.AppendText("This section is incomplete and will be finished in the future.");

        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
