using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

namespace RawToolsGUI
{
    [Serializable]
    public class PeptideModifications
    {
        public List<(string Mass, string AA, bool Fixed)> Mods;

        public PeptideModifications()
        {
            Mods = new List<(string Mass, string AA, bool Use)>();
            Mods.Add(("57.02146", "C", true));
            Mods.Add(("15.99491", "M", false));
            Mods.Add(("229.16293", "K", false));
            Mods.Add(("229.16293", "[", false));
        }
        
        public string FModsString
        {
            get
            {
                List<string> mods = new List<string>();

                foreach (var mod in Mods)
                {
                    if (mod.Fixed) mods.Add(GetModString(mod));
                }

                return String.Join(",", mods);
            }
        }

        public string VModsString
        {
            get
            {
                List<string> mods = new List<string>();

                foreach (var mod in Mods)
                {
                    if (!mod.Fixed) mods.Add(GetModString(mod));
                }

                return String.Join(",", mods);
            }
        }

        public PeptideModifications Copy()
        {
            PeptideModifications copied = new PeptideModifications();

            copied.Mods = new List<(string Mass, string AA, bool Fixed)>();

            foreach (var mod in this.Mods) copied.Mods.Add(CopyPeptideMod(mod));

            return copied;
        }

        public void UpdateModifications(DataGridView data)
        {
            Mods = new List<(string Mass, string AA, bool Use)>();

            for (int i = 0; i < data.Rows.Count; i++)
            {
                Mods.Add(GetModFromGridRow(data.Rows[i]));
            }
        }

        private (string Mass, string AA, bool Use) GetModFromGridRow(DataGridViewRow row)
        {
            (string Mass, string AA, bool Fixed) mod;

            mod.AA = (string)row.Cells["ModAA"].Value;
            mod.Mass = (string)row.Cells["ModMass"].Value;
            mod.Fixed = (bool)row.Cells["FixedMod"].Value;

            return mod;
        }

        private string GetModString((string Mass, string AA, bool Use) Mod)
        {
            return String.Concat(Mod.Mass, "@", Mod.AA);
        }

        private (string Mass, string AA, bool Use) CopyPeptideMod((string Mass, string AA, bool Use) Mod)
        {
            (string Mass, string AA, bool Use) output;

            output.Mass = Mod.Mass;
            output.AA = String.Copy(Mod.AA);
            output.Use = Mod.Use == true;

            return output;
        }
    }
}
