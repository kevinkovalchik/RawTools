using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data.Business;

namespace RawToolsViz
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();
        }

        private bool ConfirmExit()
        {
            const string message = "Are you sure you wish to exit the launcher?";
            const string caption = "Exit";
            var result = MessageBox.Show(message, caption,
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                return true;
            else
                return false;
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (ConfirmExit() == false)
            {
                e.Cancel = true;
            };
        }

        private void loadQcButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openQcData = new OpenFileDialog();
            openQcData.Filter = "RawTools QC data|*.csv";
            openQcData.Title = "Select QC data";
            openQcData.ShowDialog();

            if (String.IsNullOrEmpty(openQcData.FileName))
            {
                return;
            }

            var t = new Thread(() => new QcDataViz(openQcData.FileName).ShowDialog());
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void loadParseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openParseData = new OpenFileDialog();
            openParseData.Filter = "RawTools parse data|*.txt";
            openParseData.Title = "Select parse data";
            openParseData.ShowDialog();

            if (String.IsNullOrEmpty(openParseData.FileName))
            {
                return;
            }

            var t = new Thread(() => new ParseDataViz(openParseData.FileName).ShowDialog());
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void loadChroButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openChroData = new OpenFileDialog();
            openChroData.Filter = "RawTools chromatogram data|*.txt";
            openChroData.Title = "Select chromatogram data";
            openChroData.ShowDialog();

            if (String.IsNullOrEmpty(openChroData.FileName))
            {
                return;
            }

            var t = new Thread(() => new ChromatogramViz(openChroData.FileName).ShowDialog());
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void loadRawButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openChroData = new OpenFileDialog();
            openChroData.Filter = "Thermo RAW files|*.raw";
            openChroData.Title = "Select RAW file";
            openChroData.ShowDialog();

            if (String.IsNullOrEmpty(openChroData.FileName))
            {
                return;
            }

            var t = new Thread(() => new RawFileViz(openChroData.FileName).ShowDialog());
            //t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
    }
}
