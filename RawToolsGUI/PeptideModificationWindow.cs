using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawToolsGUI
{
    public partial class PeptideModificationForm : Form
    {
        public PeptideModificationForm()
        {
            InitializeComponent();
        }

        public void PopulateTable(PeptideModifications Mods)
        {
            foreach (var mod in Mods.Mods)
            {
                dataGridViewModifications.Rows.Add(mod.Mass, mod.AA, mod.Fixed);
            }
        }

        /*
        public void EditTable(PeptideModifications Mods)
        {
            var backup = Mods.Copy();

            if (this.ShowDialog() == DialogResult.OK)
            {
                return;
            }
            else
            {
                //this.dataGridViewModifications.DataSource = dt;
                return;
            }
        }
        */

        private void buttonAddFixedMod_Click(object sender, EventArgs e)
        {
            dataGridViewModifications.Rows.Add("", "", false);
        }

        private void buttonRemoveFixedMod_Click(object sender, EventArgs e)
        {
            dataGridViewModifications.Rows.Remove(dataGridViewModifications.CurrentRow);
        }

        private void buttonEditModsOK_Click(object sender, EventArgs e)
        {

        }

        private void buttonEditModsCancel_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewModifications_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 0 & dataGridViewModifications.EditingControl != null) // 1 should be your column index
            {
                double i;

                if (!double.TryParse(Convert.ToString(e.FormattedValue), out i))
                {
                    //dataGridViewModifications.EditingControl.Text = "";
                    dataGridViewModifications.CurrentCell.Style.BackColor = Color.Red;
                    
                    e.Cancel = false;
                }
                else
                {
                    dataGridViewModifications.CurrentCell.Style.BackColor = Color.White;
                    e.Cancel = false;
                }
            }

            if (e.ColumnIndex == 1
                & dataGridViewModifications.EditingControl != null) // 1 should be your column index
            {
                string value = Convert.ToString(e.FormattedValue);

                if (value.Length > 0 & value != null)
                {
                    value = value.First().ToString();

                    if (Char.IsLetter(value.First()))
                    {
                        dataGridViewModifications.EditingControl.Text = value.ToUpper();
                        dataGridViewModifications.CurrentCell.Style.BackColor = Color.White;

                        e.Cancel = false;
                    }
                    else
                    {
                        //dataGridViewModifications.EditingControl.Text = "";
                        dataGridViewModifications.CurrentCell.Style.BackColor = Color.Red;

                        e.Cancel = false;
                    }
                }
                else
                {
                    //dataGridViewModifications.EditingControl.Text = "";
                    dataGridViewModifications.CurrentCell.Style.BackColor = Color.Red;

                    e.Cancel = false;
                }
            }
        }
    }
}
