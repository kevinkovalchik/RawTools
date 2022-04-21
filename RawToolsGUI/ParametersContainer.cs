using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawToolsGUI
{
    [Serializable]
    public class ParametersContainer
    {
        public bool ParseMode, QcMode, Ms1Mode;

        public string RawFileDirectory, RawFileList;

        public bool DataOutputMGF, DataOutputParseMatrix, DataOutputMetrics, DataOutputChromatograms, DataOutputDirectory;

        public string DataOutputDirectoryString;

        public bool RefinePrecursorMassCharge;

        public string MinCharge, MaxCharge;

        public string QcDataDirectory;

        public bool SearchEngineNone, SearchEngineXTandem;

        public string XTandemDirectory, FastaFile;

        public string NumberSpectraToSearch;

        public PeptideModifications PeptideModifications;

        public bool QuantifyReporterIons;

        public string Reagents;

        public bool MGFLowMassCutoff;

        public string MGFLowMassCutoffValue;

        public bool ChromatogramMs1, ChromatogramMs2, ChromatogramTIC, ChromatogramBP;

        public bool UseRawFileDirectory, UseRawFileList;
    }
}
