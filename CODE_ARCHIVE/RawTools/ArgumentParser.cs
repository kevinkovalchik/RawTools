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
using CLParser;

namespace RawTools.ArgumentParser
{
    static class ParserForRawTools
    {
        public static ClParser Create()
        {
            ClParser parser = new ClParser("Welcome to the main page for RawTools version 2.0.4! RawTools is an " +
                "open-source and freely available package designed to perform scan data parsing and quantification, " +
                "and quality control analysis of Thermo Orbitrap raw mass spectrometer files. RawTools uses the " +
                "Thermo RawFileReader library (Copyright © 2016 by Thermo Fisher Scientific, Inc. All rights reserved). " +
                "RawTools is fully compatible with Windows, Linux, and MacOS operating systems.");

            parser.Add(new Argument(name: "RawFiles", shortArgument: "-f", longArgument: "-files", required: false,
                typeOf: typeof(List<string>), allowList: true,
                helpText: "Indicates input file(s) to be processed, separated by a space if there are multiple files. " +
            "Must be Thermo .raw files. You must use either -f or -d to indicate the file(s) to process."));

            parser.Add(new Argument(name: "RawFileDirectory", shortArgument: "-d", longArgument: "-directory", required: false,
                typeOf: typeof(string),
                helpText: "Indicates directory to be processed. Files other than .raw files will be ignored. " +
                "You must use either -d or -f to indicate the file(s) to process."));

            parser.Add(new Argument(name: "SearchSubdirectories", shortArgument: "-s", longArgument: "-searchsubdirectories", required: false,
                typeOf: typeof(bool),
                helpText: "Indicate that, in addition to the directory specified by -d, search all subdirectories for .raw files."));

            parser.Add(new Argument(name: "QcDirectory", shortArgument: "-qc", longArgument: "-qcdirectory", required: false,
                typeOf: typeof(string),
                helpText: "Indicates you want to perform QC. Must be followed by the tath to the directory containing " +
                "(or to contain) the QC data file (called QC.xml)."));

            parser.Add(new Argument(name: "Parse", shortArgument: "-p", longArgument: "-parse", required: false,
                typeOf: typeof(bool),
                helpText: "Parses raw file meta and scan data and writes the output to a tab-delimited text file. " +
                "Typically either this output or the quant output (-q) is used unless your aim is to simply create an MGF " +
                "or to observe broad metrics using -x."));

            parser.Add(new Argument(name: "Quant", shortArgument: "-q", longArgument: "-quant", required: false,
                typeOf: typeof(bool),
                helpText: "Similar to parse (-p), but also quantifies reporter ions and write results to output matrix. " +
                "Use of this flag requires you also specify the reagents used for isobaric labeling with the -r argument (e.g. -r TMT10)"));

            parser.Add(new Argument(name: "LabelingReagent", shortArgument: "-r", longArgument: "-labellingreagent", required: false,
                typeOf: typeof(string),
                helpText: "Required for reporter ion quantification. Reagents used to label peptides, required if using quant option. " +
                "Available options are: {TMT0, TMT2, TMT6, TMT10, TMT11, TMT16, TMT18, iTRAQ4, iTRAQ8}."));

            parser.Add(new Argument(name: "UnlabeledQuant", shortArgument: "-u", longArgument: "-unlabeledquant", required: false,
                typeOf: typeof(bool),
                helpText: "Calculate areas of precursor peaks and writes them to the parse or quant file " +
                "(ParentPeakArea column). This option is to be used in combination with -p or -q."));

            parser.Add(new Argument(name: "WriteMGF", shortArgument: "-m", longArgument: "-mgf", required: false,
                typeOf: typeof(bool),
                helpText: "Writes a standard MGF file. To specify a mass cutoff use the -c argument."));

            parser.Add(new Argument(name: "WriteMgfLevels", shortArgument: "-ml", longArgument: "-mgfLevels", required: false,
                typeOf: typeof(string),
                helpText: "Writes a standard MGF file for different scan levels. Should be in the format \"-mgfLevels [levels]\", " +
                "where levels is the MS level (or a combination of levels). For example, to output MS2 and MS3 scans, you would " +
                "invoke the command \"-mgfLevels 23\". Currently, only MS levels 1, 2, and 3 are supported."));

            parser.Add(new Argument(name: "FaimsMgf", shortArgument: "-faimsMgf", longArgument: "-faimsMgf", required: false,
                typeOf: typeof(bool),
                helpText: "Writes an MGF for each FAIMS CV detected in the file."));

            parser.Add(new Argument(name: "MgfMassCutoff", shortArgument: "-c", longArgument: "-masscutoff", required: false,
                typeOf: typeof(float),
                helpText: "Specify a mass cutoff to be applied when generating MGF files. " +
                   "May be of use if removal of reporter ions is desired prior to searching of MS2 spectra. Default is 0."));

            parser.Add(new Argument(name: "OutputDirectory", shortArgument: "-o", longArgument: "-out", required: false,
                typeOf: typeof(string),
                helpText: "The directory in which to write output. Can be a relative or absolute path to the directory. If it is a relative path it will be placed inside " +
                "the directory containing the respective raw file. Note that relative paths should not start with a slash. If this is left blank, the directory where the raw file is stored will " +
                "be used by default."));

            parser.Add(new Argument(name: "Metrics", shortArgument: "-x", longArgument: "-metrics", required: false,
                typeOf: typeof(bool),
                helpText: "Write a text file containing general metrics about the MS run."));

            parser.Add(new Argument(name: "AllScanData", shortArgument: "-asd", longArgument: "-allScanData", required: false,
                typeOf: typeof(bool),
                helpText: "Write a text file containing all scan data acquired in the MS run. Currently, this only outputs" +
                "MS1 data to the text file. Depending on the size of your raw file, this will generate a large output and can be slow."));

            parser.Add(new Argument(name: "Chromatogram", shortArgument: "-chro", longArgument: "-chromatograms", required: false,
                typeOf: typeof(string),
                helpText: "Write a chromatogram to disk. Should be in the format \"-chro [order][type]\", where order " +
                "is the MS order (or a combination of orders) and type is T, B, or TB (TIC, base peak and both, respectively). " +
                "For example, to generate MS1 and MS2 TIC and base peak chromatograms, invoke \"-chro 12TB\". Or, to generate a MS2 TIC, " +
                "invoke \"-chro 2T\"."));

            parser.Add(new Argument(name: "Xic", shortArgument: "-xic", longArgument: "-xic", required: false,
                typeOf: typeof(string),
                helpText: "Provide a mass value to write an extraction ion chromatrogram from MS1 data. The values should be provided " +
                "in a comma delimited format (e.g. 500,10) for mass,tolerance. The tolerance value is treated as a window around the " +
                "given mass value."));

            parser.Add(new Argument(name: "RefineMassCharge", shortArgument: "-R", longArgument: "-refinemasscharge", required: false,
                typeOf: typeof(bool),
                helpText: "Refine precursor charge and monoisotopic mass assignments. Highly recommended if " +
                "monoisotopic precursor selection was turned off in the instrument method (or peptide match on a QE instrument)."));

            parser.Add(new Argument(name: "MinCharge", shortArgument: "-min", longArgument: "-mincharge", required: false,
                typeOf: typeof(int),
                defaultValue: 2,
                helpText: "The minimum charge to consider when refining precursor mass and charge."));

            parser.Add(new Argument(name: "MaxCharge", shortArgument: "-max", longArgument: "-maxcharge", required: false,
                typeOf: typeof(int),
                defaultValue: 4,
                helpText: "The maximum charge to consider when refining precursor mass and charge."));

            parser.Add(new Argument(name: "FastaDB", shortArgument: "-db", longArgument: "-fastadb", required: false,
                typeOf: typeof(string),
                helpText: "Required for an X! Tandem search during QC. Path to a fasta protein database."));

            parser.Add(new Argument(name: "FixedModifications", shortArgument: "-fmods", longArgument: "-fixedmods", required: false,
                typeOf: typeof(string),
                helpText: "Fixed modifications to pass to the search, if desired. Use mass@aminoacid1,mass@aminoacid2 format. " +
                "It is important that the values are separated with a comma and not spaces. Invoke \">RawTools -modifications\" " +
                "to see examples of some common modifications"));

            parser.Add(new Argument(name: "VariableModifications", shortArgument: "-vmods", longArgument: "-variablemods", required: false,
                typeOf: typeof(string),
                helpText: "Variable modifications to pass to the search, if desired. Use mass@aminoacid1,mass@aminoacid2 format. " +
                "It is important that the values are separated with a comma and not spaces. Invoke \">RawTools -modifications\" " +
                "to see examples of some common modifications"));

            parser.Add(new Argument(name: "XTandemDirectory", shortArgument: "-X", longArgument: "-xtandem", required: false,
                typeOf: typeof(string),
                helpText: "Specify the path to the X! Tandem directory (the directory containing \"tandem.exe\") if you want " +
                "to run a database search as part of QC."));

            parser.Add(new Argument(name: "NumberSpectra", shortArgument: "-N", longArgument: "-numberspectra", required: false,
                typeOf: typeof(int),
                defaultValue: 3000,
                helpText: "The number of MS2 spectra to be passes to the search engine as an MGF file. Defaults to 10,000. " +
                "If N is greater than the number of MS2 scans in a raw file, all MS2 scans will be used."));

            parser.Add(new Argument(name: "LogDump", shortArgument: "-l", longArgument: "-logdump", required: false,
                typeOf: typeof(bool),
                helpText: "Write the instrument logs from all provided raw files to disk."));

            parser.Add(new Argument(name: "ExampleCommands", shortArgument: "-commands", longArgument: "-examplecommands", required: false, typeOf: typeof(bool),
                helpText: "Displays command line examples."));

            parser.Add(new Argument(name: "ExampleModifications", shortArgument: "-modifications", longArgument: "-examplemods", required: false, typeOf: typeof(bool),
                helpText: "Displays example peptide modifications."));

            parser.Add(new Argument(name: "VersionInfo", shortArgument: "-version", longArgument: "-version", required: false, typeOf: typeof(bool),
                helpText: "Displays version details."));

            parser.Add(new Argument(
                name: "MaxProcesses",
                shortArgument: "-k",
                longArgument: "-maxprocesses",
                required: false,
                typeOf: typeof(int),
                defaultValue: 4,
                helpText: "Maximum number of concurrent processes."));

            parser.Add(new Argument(
                name: "TmtImpurityTable",
                shortArgument: "-i",
                longArgument: "-impurities",
                required: false,
                typeOf: typeof(string),
                helpText: "Path to a TMT impurity table. To save an example table you can modify, use the command \"RawTools -printtable LABEL\" where LABEL is one of the available TMT labels."));

            parser.AddMutuallyExclusiveGroup(new List<string> { "RawFiles", "RawFileDirectory" });
            parser.AddMutuallyExclusiveGroup(new List<string> { "RawFiles", "QcDirectory" });

            parser.AddMutuallyDependenteGroup(new List<string> { "Quant", "LabelingReagent" });
            parser.AddMutuallyDependenteGroup(new List<string> { "XTandemDirectory", "FastaDB" });

            //parser.AddRequiredGroup(new List<string> { "RawFiles", "RawFileDirectory" });
            
            parser.AddDependencyGroup("WriteMGF", new List<string> { "MgfMassCutoff" });
            //parser.AddDependencyGroup("XTandemDirectory", new List<string> { "FixedModifications",
            //    "VariableModifications", "XModifications", "NumberSpectra" });

            return parser;
        }
    }
}
