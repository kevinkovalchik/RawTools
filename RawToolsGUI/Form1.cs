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
            dataGridViewModifications.Rows.Add("Variable at K", Mods.KMod.Mass, "K", Mods.KMod.Use);
            dataGridViewModifications.Rows[0].Cells["ModAA"].ReadOnly = true;
            dataGridViewModifications.Rows[0].Cells["ModAA"].Style.BackColor = Color.LightGray;
            dataGridViewModifications.Rows[0].Cells["ModType"].Style.BackColor = Color.LightGray;

            dataGridViewModifications.Rows.Add("Variable at N-term", Mods.NMod.Mass, "[", Mods.NMod.Use);
            dataGridViewModifications.Rows[1].Cells["ModAA"].ReadOnly = true;
            dataGridViewModifications.Rows[1].Cells["ModAA"].Style.BackColor = Color.LightGray;
            dataGridViewModifications.Rows[1].Cells["ModType"].Style.BackColor = Color.LightGray;

            dataGridViewModifications.Rows.Add("Variable at X", Mods.XMod.Mass, Mods.XMod.AA, Mods.XMod.Use);
            dataGridViewModifications.Rows[2].Cells["ModType"].Style.BackColor = Color.LightGray;

            int r = 3;
            foreach (var mod in Mods.FMods)
            {
                dataGridViewModifications.Rows.Add("Fixed", mod.Mass, mod.AA, mod.Use);
                dataGridViewModifications.Rows[r++].Cells["ModType"].Style.BackColor = Color.LightGray;
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
            dataGridViewModifications.Rows.Add("Fixed", "", "", true);
            dataGridViewModifications.Rows[dataGridViewModifications.Rows.Count - 1].Cells["ModType"].Style.BackColor = Color.LightGray;;
        }

        private void buttonRemoveFixedMod_Click(object sender, EventArgs e)
        {
            if (dataGridViewModifications.CurrentRow.Index <= 2)
            {
                return;
            }
            else
            {
                dataGridViewModifications.Rows.Remove(dataGridViewModifications.CurrentRow);
            }
        }

        private void buttonEditModsOK_Click(object sender, EventArgs e)
        {

        }

        private void buttonEditModsCancel_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewModifications_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 1) // 1 should be your column index
            {
                double i;

                if (!double.TryParse(Convert.ToString(e.FormattedValue), out i))
                {
                    if (dataGridViewModifications.EditingControl != null)
                    {
                        dataGridViewModifications.EditingControl.Text = "";
                    }
                    
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = false;
                }
            }

            if (e.ColumnIndex == 2 & e.RowIndex != 0 & e.RowIndex != 1) // 1 should be your column index
            {
                string value = Convert.ToString(e.FormattedValue);

                if (value.Length > 1)
                {
                    value = value.First().ToString();
                }

                if (Char.IsLetter(value.First()))
                {
                    dataGridViewModifications.EditingControl.Text = value.ToUpper();
                    e.Cancel = false;
                }
                else
                {
                    dataGridViewModifications.EditingControl.Text = "";
                    e.Cancel = false;
                }
            }
        }
    }
}
