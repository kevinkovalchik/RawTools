using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawTools
{
    class Examples
    {
        public static void ExampleMods()
        {
            StringBuilder examples = new StringBuilder();
            examples.AppendLine("\n");
            examples.AppendLine("-----------------------------");
            examples.AppendLine("Peptide modification examples");
            examples.AppendLine("-----------------------------");

            examples.AppendLine("\nThere are four arguments which may be used on the command line to specify peptide modifications: " +
                "fmods, nmod, kmod, xmod. RawTools passes these modifications to the selected search tool if a search is performed. " +
                "--fmods can be any number of fixed modifications, separated by a comma (and no spaces). The other three are always " +
                "variable, and modification frequency (e.g. labeling efficiency) is calculated for each. --nmod specifies a modification to the peptide N-terminus, --kmod " +
                "specifies a modification to lysine, and --xmod can specify a modification to any other residue. The peptide modifications " +
                "must be in mass@aa format. Examples of common modifications are given below.\n");

            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Oxidation of methionine:", "15.99491@M");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Carboxyamidomethylation of cysteine:", "57.02146@C");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Acetylation of (peptide) N-terminus:", "42.01056@[");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Phosphorylation at X:", "79.96633@X");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "TMT0 label at K or N-terminus:", "224.15247@K,224.15247@[");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "TMT2 label at K or N-terminus:", "225.15583@K,225.15583@[");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "TMT6+ label at K or N-terminus:", "229.16293@K,229.16293@[");

            examples.AppendLine("\nFor example, to specify fixed CamC, and variable oxidation of M and TMT10 quant labels, you would invoke the following arguments:");
            examples.AppendLine("\t--fmods 57.02146@C --nmod 229.16293@[ --kmod 229.16293@K --xmod 15.99491@M");

            examples.AppendLine("\nOr to include the TMT10 labels as fixed modifications, you could invoke the following:");
            examples.AppendLine("\t--fmods 57.02146@C,229.16293@K,229.16293@[ --xmod 15.99491@M");

            Console.Write(examples);
        }

        public static void CommandLineUsage()
        {
            StringBuilder c = new StringBuilder();
            c.AppendLine("\n");
            c.AppendLine("----------------------------");
            c.AppendLine("Command line interface usage");
            c.AppendLine("----------------------------");
            c.AppendLine("\n");
            c.AppendLine("=== parse ===");
            c.AppendLine("");
            c.AppendLine("The parse functionality of RawTools has the following general syntax:");
            c.AppendLine("");
            c.AppendLine(">RawTools parse -f [path(s) to raw file(s)] [further arguments]");
            c.AppendLine("");
            c.AppendLine("\tor");
            c.AppendLine("");
            c.AppendLine(">RawTools parse -d [path to a directory containing one or more raw files] [further arguments]");
            c.AppendLine("");
            c.AppendLine("Following -f or -d, you must include one or more of -p, -m, -x, -q, or --chro which represent the parse, MGF, metrics, quantification, and chromatogram " +
                "functionalities, respectively. If you do not include at least one of these nothing will be written to disk. Note that if " +
                "you use -q you must also include -r to indicate the labeling reagents used. For more detailed information on these " +
                "arguments invoke \">RawTools parse --help\".");
            c.AppendLine("");
            c.AppendLine("There are several completely optional arguments, including -o, -R, -c, -y, and -u. " +
                "For more detailed information on these arguments invoke \">RawTools parse --help\".");
            c.AppendLine("\n");
            c.AppendLine("Examples of parse:");
            c.AppendLine("\n");
            c.AppendLine(">RawTools parse -d C:\\MyRawFiles -p -m");
            c.AppendLine("");
            c.AppendLine("The above command processes all the raw files in C:\\MyRawFiles and writes a parse table and MGF file " +
                "to the same directory.");
            c.AppendLine("\n");
            c.AppendLine(">RawTools parse -d C:\\MyRawFiles -pm -o C:\\MyRawFiles\\results");
            c.AppendLine("");
            c.AppendLine("The above command similarly processes all the raw files in C:\\MyRawFiles, but this time it will write " +
                "the parse table and MGF file to C:\\MyRawFiles\\results. Note that -p and -m have been \"stacked\" into a single argument -pm. " +
                "This can be done with any arguments which do not require text after them.");
            c.AppendLine("\n");
            c.AppendLine(">RawTools parse -f C:\\MyRawFiles\\file1.raw -m");
            c.AppendLine("");
            c.AppendLine("The above command reads the single raw file C:\\MyRawFiles\\file1.raw and writes and MGF into the containing directory.");
            c.AppendLine("\n");
            c.AppendLine(">RawTools parse -d C:\\MyRawFiles -qmx -r TMT10 -o C:\\MyRawFiles\\results");
            c.AppendLine("");
            c.AppendLine("The above command processes all the files in C:\\MyRawFiles and writes a parse table including reporter ion " +
                "quant values and MGF and metrics files to C:\\MyRawFiles\\results. Because we have indicated with -q that we want " +
                "to quantify reporter ions, we need to also include -r to indicate what labeling reagents were used. In this case, TMT10.");
            c.AppendLine("For more detailed information on specific arguments, invoke \">RawTools parse --help\".");
            c.AppendLine("\n");
            c.AppendLine("=== qc ===");
            c.AppendLine("\n");
            c.AppendLine("The QC functionality of RawTools has the following general syntax:");
            c.AppendLine("");
            c.AppendLine(">RawTools qc -d [raw file directory] -q [qc data directory] [further options]");
            c.AppendLine("");
            c.AppendLine("-d is the directory containing raw files and -q is the directory where you want the QC data to be stored. " +
                "They can be the same directory if you desire. Invoking -d and -q with no further arguments will processes the raw " +
                "files using RawTools only, and the general metrics of each file will be included in the QC results. If you wish to " +
                "include identification-related metrics, read on.");
            c.AppendLine("");
            c.AppendLine("To get identification-related metrics, you need to indicate a search engine, which at this time can be either " +
                "IdentiPy or X! Tandem. These are indicated on the command line as \"-s identipy\" or \"-s xtandem\". You must also " +
                "indicate the location of a Fasta database file using --db. If you use IdentiPy, " +
                "you can choose to have RawTools try to find it on your system by itself. To do so use something like the following command:");
            c.AppendLine("");
            c.AppendLine(">RawTools qc -d C:\\MyRawFiles -q C:\\MyRawFiles\\QC -s identipy --db C:\\FastaFiles\\current_human.fasta");
            c.AppendLine("");
            c.AppendLine("RawTools will search for Python 2.7 and IdentiPy on your system. If it fails it will let you know, and then you can tell it where to look " +
                "using -P and -I. See \"RawTools qc --help\" for more details on these arguments.");
            c.AppendLine("");
            c.AppendLine("If you use X! Tandem, you need to tell RawTools where to find it using the -X argument to indicate the directory " +
                "containing the X! Tandem executable. For example:");
            c.AppendLine("");
            c.AppendLine(">RawTools qc -d C:\\MyRawFiles -q C:\\MyRawFiles\\QC --db C:\\FastaFiles\\current_human.fasta -s xtandem " +
                "-X C:\\tandem\\bin");
            c.AppendLine("");
            c.AppendLine("If you invoke a search tool, you can also indicate peptide modifications using the --fmods, --nmod, --xmod and --kmod " +
                "arguments. For examples of these, please see \"RawTools examples --modifications\"");
            c.AppendLine("");
            c.AppendLine("For more detailed information on specific arguments, invoke \">RawTools qc --help\".");

            Console.Write(c);
        }
    }
}
