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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace RawTools.ArgumentParser
{
    [Verb("testing", HelpText = "A place to contain testing routines during development")]
    class TestOptions
    {
        [Option('f', HelpText = "file to make the test go")]
        public string File { get; set; }

        [Option('d', HelpText = "directory to make the test go")]
        public string Directory { get; set; }

        [Option("db", Default = "C:\\Users\\Kevin\\Documents\\GSC\\Projects\\RawToolsHelaFiles\\uniprot-human-ref-20180807.fasta", HelpText = "Required for X! Tandem or IdentiPy search. Path to a fasta protein database.")]
        public string FastaDatabase { get; set; }

        [Option("fmods", Default = "57.02146@C,229.16293@K,229.16293@[", HelpText = "Optional. Fixed modifications to pass to the search, if desired. Use mass@aminoacid1,mass@aminoacid2 format. " +
            "It is important that the values are separated with a comma and not spaces. IMPORTANT: Do not include isobaric quantification tags here  (e.g. TMT, iTRAQ). Instead, these must " +
            "be specified using the --qmod argument. Invoke \">RawTools qc -e\" to see examples of some common modifications")]
        public string FixedMods { get; set; }

        [Option('X', Default = "C:\\Users\\Kevin\\Documents\\GSC\\Projects\\tandem-win-17-02-01-4\\tandem-win-17-02-01-4\\bin", HelpText = "Specify the path to the X! Tandem directory (the directory containing \"tandem.exe\").")]
        public string XTandemDirectory { get; set; }

        [Option("static", HelpText = "Will use mgf and search results aleady present. Only used for development purposes.")]
        public bool StaticSearch { get; set; }
    }

    [Verb("examples", HelpText = "Display some common peptide modification in mass@aa format and examples of usage.")]
    class ExampleMods
    { }

    [Verb("parse", HelpText = "Perform meta and quant data parsing. Also performs data processing, MGF file creation, and saves meta/quant data and run metrics to disk.")]
    class ParseOptions
    {
        [Option('f', "files", SetName = "files", HelpText = "Indicates input file(s) to be processed, separated by a space if there are multiple files. " +
            "Must be Thermo .raw files. You must use either -f or -d to indicate the file(s) to process.")]
        public IEnumerable<string> InputFiles { get; set; }

        [Option('d', "directory", SetName = "directory", HelpText = "Indicates directory to be processed. Files other than .raw files will be ignored. " +
            "You must use either -d or -f to indicate the file(s) to process.")]
        public string InputDirectory { get; set; }

        [Option('p', "parse", HelpText = "Optional. Parses raw file meta and scan data and writes the output to a tab-delimited text file. " +
            "Typically either this output or the quant output (-q) is used unless your aim is to simply create an MGF or to observe broad metrics using -x.")]
        public bool ParseData { get; set; }

        [Option('q', "quant", HelpText = "Optional. Similar to parse (-p), but also quantifies reporter ions and write results to output matrix. " +
            "Use of this flag requires you also specify the reagents used for isobaric labeling with the -r argument (e.g. -r TMT10)")]
        public bool Quant { get; set; }

        [Option('r', "labelingreagent", HelpText = "Required for reporter ion quantification. Reagents used to label peptides, required if using quant option. " +
            "Available options are: {TMT0, TMT2, TMT6, TMT10, TMT11, iTRAQ4, iTRAQ8}.")]
        public string LabelingReagents { get; set; }

        [Option('u', "unlabeledquant", HelpText = "Optional. Calculate areas of precursor peaks and writes them to the parse or quant file (ParentPeakArea column). This option is to be used in combination with -p or -q.")]
        public bool UnlabeledQuant { get; set; }

        [Option('m', "writemgf", HelpText = "Optional. Writes a standard MGF file. To specify mass and intensity cutoffs use the -c and -y arguments.")]
        public bool WriteMGF { get; set; }

        [Option('c', "masscutoff", Default = 0, HelpText = "Optional. Specify a mass cutoff to be applied when generating MGF files. " +
            "May be of use if removal of reporter ions is desired prior to searching of MS2 spectra. Default is 0.")]
        public double MassCutOff { get; set; }

        [Option('y', "intensitycutoff", Default = 0, HelpText = "Optional. Specify a relative intensity cutoff for ions included in a MGF file. " +
            "The value is relative to the highest intensity ion found in a given scan. For example, if you use \"-y 0.01\", then for all scans peaks of less than 1% " +
            "the intensity of the highest intensity ion in that scan will be excluded.")]
        public double IntensityCutoff { get; set; }

        [Option('o', "out", HelpText = "Optional. The directory in which to write output. Can be a relative or absolute path to the directory. If it is a relative path it will be placed inside " +
            "the directory containing the respective raw file. Note that relative paths should not start with a slash. If this is left blank, the directory where the raw file is stored will " +
            "be used by default.")]
        public string OutputDirectory { get; set; }

        [Option('x', "metrics", HelpText = "Optional. Writes a txt file containing general metrics about the MS run.")]
        public bool Metrics { get; set; }

        [Option("chro", HelpText = "Optional. Write a chromatogram to disk. Should be in the format \"--chro [order][type]\", where order " +
            "is the MS order and type is T, B, or TB (TIC, base peak and both, respectively). For example, to generate a MS1 TIC and base peak chromatogram, invoke " +
            "\"--chro 1TB\". Or, to generate a MS2 TIC, invoke \"--chro 2T\".")]
        public string Chromatogram { get; set; }

        [Option('e', "experimenttype", Default = "DDA", HelpText = "Specify the type of MS experiment. Options are [DDA, DIA, PRM].")]
        public string ExperimentType { get; set; }

        [Option('R', "refinemasscharge", HelpText = "Optional. Refine precursor charge and monoisotopic mass assignments. Highly recommended if " +
            "monoisotopic precursor selection was turned off in the instrument method.")]
        public bool RefineMassCharge { get; set; }
    }

    [Verb("qc", HelpText = "Perform QC operations. Two arguments are required: -d, the directory containing one or more raw files to QC; -q, a " +
        "directory in which to hold the QC data. -q and -d can be identical if desired. After running for the first time on a given directory, a file " +
        "called QC.xml will appear in the QC directory. It is an xml file containing the QC data. If new files are copied or acquired into " +
        "the -d directory, QC may be run again and the new files will be added to the QC data.")]
    class QcOptions
    {
        [Option('d', "directory", Required = true, HelpText = "Path to the directory containing the raw files for QC")]
        public string DirectoryToQc { get; set; }

        [Option('q', "qcdirectory", Required = true, HelpText = "Path to the directory containing (or to contain) the QC data file (called QC.xml)")]
        public string QcDirectory { get; set; }

        [Option('s', "search", Required = false, Default = "None", HelpText = "Specify the search engine to facilitate the calculation of identification related metrics. " +
            "Must be one of the following: {identipy, xtandem}")]
        public string SearchAlgorithm { get; set; }

        [Option('i', "identipy", HelpText = "[DEPRECATED. This argument will be replaced with the more general -s (--search) in a future version.]" +
            "Optional. Perform an IdentiPy search on a subset of " +
            "the ms2 scans in the raw file to get identification-related metrics as part of the QC.")]
        public bool Identipy { get; set; }
        
        [Option("db", HelpText = "Required for X! Tandem or IdentiPy search. Path to a fasta protein database.")]
        public string FastaDatabase { get; set; }

        [Option("fmods", HelpText = "Optional. Fixed modifications to pass to the search, if desired. Use mass@aminoacid1,mass@aminoacid2 format. " +
            "It is important that the values are separated with a comma and not spaces. IMPORTANT: Do not include isobaric quantification tags here  (e.g. TMT, iTRAQ). Instead, these must " +
            "be specified using the --qmod argument. Invoke \">RawTools qc -e\" to see examples of some common modifications")]
        public string FixedMods { get; set; }

        [Option("nmod", HelpText = "Optional. A single variable N-term modification to pass to the search, if desired. Use mass@aminoacid1 format. " +
            "Invoke \">RawTools qc -e\" to see examples of some common modifications")]
        public string VariableNMod { get; set; }

        [Option("kmod", HelpText = "Optional. A single variable lysine modification to pass to the search, if desired. Use mass@aminoacid1 format. " +
            "Invoke \">RawTools qc -e\" to see examples of some common modifications")]
        public string VariableKMod { get; set; }

        [Option("xmod", HelpText = "Optional. An additional single modification to pass to the search, if desired. The modification cannot " +
            "be on the N-terminus or lysine. Use mass@aminoacid1 format. Invoke \">RawTools qc -e\" to see examples of some common modifications.")]
        public string VariableXMod { get; set; }

        [Option('p', "pythonexecutable", HelpText = "Path to the Python executable. This is usually unnecessary, but if you encounter problems " +
            "with Python being detected you might need to invoke it. -p and -I must be invoked together.")]
        public string PythonExecutable { get; set; }

        [Option('I', "identipyscript", HelpText = "Path to the script used to start Identipy. On windows this should be in the Scripts folder found " +
            "in the Python installation directory and is called identipy-script.py. On Linux it is usually in /bin or /usr/bin and is simply called identipy. " +
            "However, if you are using a virtual python environment this location will vary from the system-wide installation. " +
            "Include the extension if there is one.  -I and -p must be invoked together.")]
        public string IdentipyScript { get; set; }

        [Option('X', "xtandemdirectory", HelpText = "Specify the path to the X! Tandem directory (the directory containing \"tandem.exe\").")]
        public string XTandemDirectory { get; set; }

        [Option('N', "numberspectra", Default = 10000, HelpText = "Optional. The number of MS2 spectra to be passes to the search engine as an MGF file. Defaults to 10,000. " +
            "If N is greater than the number of MS2 scans in a raw file, all MS2 scans will be used.")]
        public int NumberSpectra { get; set; }

        [Option('c', "masscutoff", Default = 0, HelpText = "Optional. Specify a mass cutoff to be applied when generating MGF files for use in the IdentiPy search. " +
            "May be of use if removal of reporter ions is desired prior to searching of MS2 spectra. Default is 0.")]
        public double MassCutOff { get; set; }

        [Option('y', "intensitycutoff", Default = 0, HelpText = "Optional. Specify a relative intensity cutoff for ions included in the MGF file passed to IdentiPy. " +
            "The value is relative to the highest intensity ion found in a given scan. For example, if you use \"-y 0.01\", then for all scans peaks of less than 1% " +
            "the intensity of the highest intensity ion in that scan will be excluded.")]
        public double IntensityCutoff { get; set; }

        [Option("fixedscans", HelpText = "Causes the scans in the mgf file used for a database search to be static (i.e. not random, the same " +
            "scans are used everytime). This is intended for testing purposes, not for general use.")]
        public bool FixedScans { get; set; }

        [Option('e', "experimenttype", Default = "DDA", HelpText = "Specify the type of MS experiment. Options are [DDA, DIA, PRM].")]
        public string ExperimentType { get; set; }

        [Option('R', "refinemasscharge", HelpText = "Optional. Refine precursor charge and monoisotopic mass assignments. Highly recommended if " +
            "monoisotopic precursor selection was turned off in the instrument method.")]
        public bool RefineMassCharge { get; set; }
    }
}
