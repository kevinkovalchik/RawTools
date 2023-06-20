// Copyright 2018 Kevin Kovalchik & Christopher Hughes
// 
// Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// Kevin Kovalchik and Christopher Hughes do not claim copyright of
// any third-party libraries ditributed with RawTools. All third party
// licenses are provided in accompanying files as outline in the NOTICE.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using CommandLine;
using CommandLine.Text;
using ThermoFisher.CommonCore.Data;

namespace RawTools.ArgumentParser
{
    public class Options
    {
        [Option('f', "files", Required = false, HelpText =
            "Indicates input file(s) to be processed, separated by a space if there are multiple files. " +
            "Must be Thermo .raw files. You must use either -f or -d to indicate the file(s) to process.")]
        public IEnumerable<string> RawFiles { get; set; }

        [Option('d', "directory", Required = false, HelpText =
            "Indicates directory to be processed. Files other than .raw files will be ignored. " +
            "You must use either -d or -f to indicate the file(s) to process.")]
        public string RawFileDirectory { get; set; }

        [Option('s', "searchsubdirectories", Required = false,
                HelpText = "Indicate that, in addition to the directory specified by -d, search all subdirectories for .raw files.")]
        public bool SearchSubdirectories { get; set; }

        [Option("qcdirectory", Required = false,
                HelpText = "Indicates you want to perform QC. Must be followed by the tath to the directory containing " +
                "(or to contain) the QC data file (called QC.xml).")]
        public string QcDirectory { get; set; }

        [Option('p', "parse", Required = false,
                HelpText = "Parses raw file meta and scan data and writes the output to a tab-delimited text file. " +
                "Typically either this output or the quant output (-q) is used unless your aim is to simply create an MGF " +
                "or to observe broad metrics using -x.")]
        public bool Parse { get; set; }

        [Option('q', "quant", Required = false,
                HelpText = "Similar to parse (-p), but also quantifies reporter ions and write results to output matrix. " +
                "Use of this flag requires you also specify the reagents used for isobaric labeling with the -r argument (e.g. -r TMT10)")]
        public bool Quant { get; set; }

        [Option('r', "labellingreagent", Required = false,
                HelpText = "Required for reporter ion quantification. Reagents used to label peptides, required if using quant option. " +
                "Available options are: {TMT0, TMT2, TMT6, TMT10, TMT11, TMT16, TMT18, iTRAQ4, iTRAQ8}.")]
        public string LabelingReagent { get; set; }

        [Option('u', "unlabeledquant", Required = false, HelpText =
            "Calculate areas of precursor peaks and writes them to the parse or quant file " +
            "(ParentPeakArea column). This option is to be used in combination with -p or -q.")]
        public bool UnlabeledQuant { get; set; }

        [Option('m', "mgf", Required = false,
            HelpText = "Writes a standard MGF file. To specify a mass cutoff use the -c argument.")]
        public bool WriteMGF { get; set; }

        [Option("mgfLevels", Required = false,
            HelpText = "Writes a standard MGF file for different scan levels. Should be in the format \"-mgfLevels [levels]\", " +
            "where levels is the MS level (or a combination of levels). For example, to output MS2 and MS3 scans, you would " +
            "invoke the command \"-mgfLevels 23\". Currently, only MS levels 1, 2, and 3 are supported.")]
        public string WriteMgfLevels { get; set; }

        [Option("faimsMgf", Required = false,
            HelpText = "Writes an MGF for each FAIMS CV detected in the file.")]
        public bool FaimsMgf { get; set; }

        [Option('c', "masscutoff", Required = false,
            HelpText = "Specify a mass cutoff to be applied when generating MGF files. " +
                "May be of use if removal of reporter ions is desired prior to searching of MS2 spectra. Default is 0.")]
        public float MgfMassCutOff { get; set; }

        [Option('o', "out", Required = false,
            HelpText = "The directory in which to write output. Can be a relative or absolute path to the directory. If it is a relative path it will be placed inside " +
            "the directory containing the respective raw file. Note that relative paths should not start with a slash. If this is left blank, the directory where the raw file is stored will " +
            "be used by default.")]
        public string OutputDirectory { get; set; }

        [Option('x', "metrics", Required = false,
            HelpText = "Write a text file containing general metrics about the MS run.")]
        public bool Metrics { get; set; }

        [Option("allScanData", Required = false,
            HelpText = "Write a text file containing all scan data acquired in the MS run. Currently, this only outputs" +
            "MS1 data to the text file. Depending on the size of your raw file, this will generate a large output and can be slow.")]
        public bool AllScanData { get; set; }

        [Option("chromatograms", Required = false,
            HelpText = "Write a chromatogram to disk. Should be in the format \"-chro [order][type]\", where order " +
            "is the MS order (or a combination of orders) and type is T, B, or TB (TIC, base peak and both, respectively). " +
            "For example, to generate MS1 and MS2 TIC and base peak chromatograms, invoke \"-chromatograms 12TB\". Or, to generate a MS2 TIC, " +
            "invoke \"-chromatograms 2T\".")]
        public string Chromatogram { get; set; }

        [Option("xic", Required = false,
            HelpText = "Provide a mass value to write an extraction ion chromatrogram from MS1 data. The values should be provided " +
            "in a comma delimited format (e.g. 500,10) for mass,tolerance. The tolerance value is treated as a window around the " +
            "given mass value.")]
        public string Xic { get; set; }

        [Option('R', "refinemasscharge", Required = false,
           HelpText = "Refine precursor charge and monoisotopic mass assignments. Highly recommended if " +
            "monoisotopic precursor selection was turned off in the instrument method (or peptide match on a QE instrument).")]
        public bool RefineMassCharge { get; set; }

        [Option("mincharge", Required = false,
            HelpText = "The minimum charge to consider when refining precursor mass and charge.")]
        public int MinCharge { get; set; } = 2;

        [Option("maxcharge", Required = false,
            HelpText = "The maximum charge to consider when refining precursor mass and charge.")]
        public int MaxCharge { get; set; } = 4;

        [Option("fastadb", Required = false,
            HelpText = "Required for an X! Tandem search during QC. Path to a fasta protein database.")]
        public string FastaDB { get; set; }

        [Option("fixedmods", Required = false,
            HelpText = "Fixed modifications to pass to the search, if desired. Use mass@aminoacid1,mass@aminoacid2 format. " +
            "It is important that the values are separated with a comma and not spaces. Invoke \">RawTools -modifications\" " +
            "to see examples of some common modifications")]
        public string FixedModifications { get; set; }

        [Option("variablemods", Required = false,
            HelpText = "Variable modifications to pass to the search, if desired. Use mass@aminoacid1,mass@aminoacid2 format. " +
            "It is important that the values are separated with a comma and not spaces. Invoke \">RawTools -modifications\" " +
            "to see examples of some common modifications")]
        public string VariableModifications { get; set; }

        [Option('X', "xtandem", Required = false,
            HelpText = "Specify the path to the X! Tandem directory (the directory containing \"tandem.exe\") if you want " +
            "to run a database search as part of QC.")]
        public string XTandemDirectory { get; set; }

        [Option('N', "numberspectra", Required = false,
            HelpText = "The number of MS2 spectra to be passes to the search engine as an MGF file. Defaults to 10,000. " +
            "If N is greater than the number of MS2 scans in a raw file, all MS2 scans will be used.")]
        public int NumberSpectra { get; set; } = 3000;

        [Option('l', "logdump", Required = false,
            HelpText = "Write the instrument logs from all provided raw files to disk.")]
        public bool LogDump { get; set; }

        [Option("examplecommands", Required = false,
            HelpText = "Displays command line examples.")]
        public bool ExampleCommands { get; set; }

        [Option("examplemods", Required = false,
            HelpText = "Displays example peptide modifications.")]
        public bool ExampleModifications { get; set; }

        [Option("version", Required = false,
            HelpText = "Displays version details.")]
        public bool VersionInfo { get; set; } = false;

        [Option('k',
            "maxprocesses",
            Required = false,
            HelpText = "Maximum number of concurrent processes.")]
        public int MaxProcesses { get; set; } = 4;

        [Option('i', "impurities",
            Required = false,
            HelpText = "Path to a TMT impurity table. To save an example table you can modify, use the command \"RawTools -printtable LABEL\" where LABEL is one of the available TMT labels.")]
        public string TmtImpurityTable { get; set; }

// parser.AddMutuallyExclusiveGroup(new List<string> { "RawFiles", "RawFileDirectory" });
// parser.AddMutuallyExclusiveGroup(new List<string> { "RawFiles", "QcDirectory" });

// parser.AddMutuallyDependenteGroup(new List<string> { "Quant", "LabelingReagent" });
// parser.AddMutuallyDependenteGroup(new List<string> { "XTandemDirectory", "FastaDB" });

//parser.AddRequiredGroup(new List<string> { "RawFiles", "RawFileDirectory" });

// parser.AddDependencyGroup("WriteMGF", new List<string> { "MgfMassCutoff" });
            //parser.AddDependencyGroup("XTandemDirectory", new List<string> { "FixedModifications",
            //    "VariableModifications", "XModifications", "NumberSpectra" });

    }

    static class ParserForRawTools
    {
        public static Options Parse(string[] args)
        {
            return CommandLine.Parser.Default.ParseArguments<Options>(args).Value;
        }
    }
}
