using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

namespace RawToolsGUI
{
    public class PeptideModifications
    {
        public (string Mass, string AA, bool Use) KMod;
        public (string Mass, string AA, bool Use) NMod;
        public (string Mass, string AA, bool Use) XMod;
        public List<(string Mass, string AA, bool Use)> FMods;

        public PeptideModifications()
        {
            KMod.AA = "K";
            NMod.AA = "[";
            FMods = new List<(string Mass, string AA, bool Use)>();
        }

        public string KModString { get { return GetModString(KMod); } }

        public string NModString { get { return GetModString(NMod); } }

        public string XModString { get { return GetModString(NMod); } }

        public string FModsString
        {
            get
            {
                List<string> mods = new List<string>();

                foreach (var mod in FMods)
                {
                    mods.Add(GetModString(mod));
                }

                return String.Join(",", mods);
            }
        }

        public PeptideModifications Copy()
        {
            PeptideModifications copied = new PeptideModifications();
            copied.KMod = CopyPeptideMod(KMod);
            copied.NMod = CopyPeptideMod(NMod);
            copied.XMod = CopyPeptideMod(XMod);

            foreach (var mod in FMods) copied.FMods.Add(CopyPeptideMod(mod));

            return copied;
        }

        public void UpdateModifications(DataGridView data)
        {
            KMod = GetModFromGridRow(data.Rows[0]);
            NMod = GetModFromGridRow(data.Rows[1]);
            XMod = GetModFromGridRow(data.Rows[2]);

            FMods = new List<(string Mass, string AA, bool Use)>();

            for (int i = 3; i < data.Rows.Count; i++)
            {
                FMods.Add(GetModFromGridRow(data.Rows[i]));
            }
        }

        private (string Mass, string AA, bool Use) GetModFromGridRow(DataGridViewRow row)
        {
            (string Mass, string AA, bool Use) mod;

            mod.AA = (string)row.Cells["ModAA"].Value;
            mod.Mass = (string)row.Cells["ModMass"].Value;
            mod.Use = (bool)row.Cells["UseMod"].Value;

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
