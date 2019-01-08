using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawToolsGUI
{
    [Serializable]
    class ParametersContainer
    {
        bool ParseMode, QcMode;

        string RawFileDirectory;

        List<string> RawFileList;

        bool DataOutputMGF, DataOutputParseMatrix, DataOutputMetrics, DataOutputChromatograms, DataOutputDirectory;

        string DataOutputDirectoryString;

        bool RefinePrecursorMassCharge;

        string MinCharge, MaxCharge;

        string QcDataDirectory;

        bool SearchEngineNone, SearchEngineXTandem, SearchEngineIdentipy, SearchEngineIdentipyAutoFind;

        string XTandemDirectory, PythonExecutable, IdentipyScript, FastaFile;

        string NumberSpectraToSearch;

        PeptideModifications PeptideModifications;

        bool QuantifyReporterIons;

        string Reagents;

        bool MGFLowMassCutoff;

        string MGFLowMassCutoffValue;

        bool ChromatogramMs1, ChromatogramMs2, ChromatogramTIC, ChromatogramBP;
    }
}
