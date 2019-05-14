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
            if (ConfirmExit())
            {
                this.Close();
                //Environment.Exit(0);
            }
        }

        private void loadQcButton_Click(object sender, EventArgs e)
        {
            new Thread(() => new QcDataViz(@"C:\Users\Kevin\Downloads\QcDataTable.csv").ShowDialog()).Start();
        }
    }
}
