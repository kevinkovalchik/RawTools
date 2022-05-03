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

namespace RawTools
{
    class Examples
    {
        public static void VersionInfo()
        {
            StringBuilder examples = new StringBuilder();
            examples.AppendLine("2.0.6a");
            Console.Write(examples);
        }

        public static void ExampleMods()
        {
            StringBuilder examples = new StringBuilder();
            examples.AppendLine("\n");
            examples.AppendLine("-----------------------------");
            examples.AppendLine("Peptide modification examples");
            examples.AppendLine("-----------------------------");

            examples.AppendLine("\nThere are two arguments which may be used on the command line to specify peptide modifications: " +
                "fmods and vmods. RawTools passes these modifications to the selected search tool if a search is performed. " +
                "-fmods can be any number of fixed modifications, separated by a comma (and no spaces). -vmods can be any " +
                "number of variable modifications, separated by a comma (and no spaces). The peptide modifications " +
                "must be in mass@aa format. Examples of common modifications are given below.\n");

            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Oxidation of methionine:", "15.99491@M");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Carboxyamidomethylation of cysteine:", "57.02146@C");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Acetylation of (peptide) N-terminus:", "42.01056@[");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "Phosphorylation at X:", "79.96633@X");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "TMT0 label at K or N-terminus:", "224.15247@K,224.15247@[");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "TMT2 label at K or N-terminus:", "225.15583@K,225.15583@[");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "TMT6+ label at K or N-terminus:", "229.16293@K,229.16293@[");
            examples.AppendFormat("\t{0,-40}{1,-10}\n", "TMT16+ label at K or N-terminus:", "304.2071@K,304.2071@[");

            examples.AppendLine("\nFor example, to specify fixed CamC, and variable oxidation of M and TMT10 quant labels, you would invoke the following arguments:");
            examples.AppendLine("\t-fmods 57.02146@C -vmods 229.16293@[,229.16293@K,15.99491@M");

            examples.AppendLine("\nOr to include the TMT10 labels as fixed modifications, you could invoke the following:");
            examples.AppendLine("\t-fmods 57.02146@C,229.16293@K,229.16293@[ -vmods 15.99491@M");

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
            c.AppendLine(">RawTools -f [path(s) to raw file(s)] [further arguments]");
            c.AppendLine("");
            c.AppendLine("\tor");
            c.AppendLine("");
            c.AppendLine(">RawTools -d [path to a directory containing one or more raw files] [further arguments]");
            c.AppendLine("");
            c.AppendLine("Following -f or -d, you must include one or more of -p, -m, -x, -q, -l, or -chro which represent the parse, MGF, metrics, quantification, status log, and chromatogram " +
                "functionalities, respectively. If you do not include at least one of these nothing will be written to disk. Note that if " +
                "you use -q you must also include -r to indicate the labeling reagents used. For more detailed information on these " +
                "arguments invoke \">RawTools -help\".");
            c.AppendLine("");
            c.AppendLine("There are several completely optional arguments, including -o, -R, -c, and -u. " +
                "For more detailed information on these arguments invoke \">RawTools parse --help\".");
            c.AppendLine("\n");
            c.AppendLine("Examples of parse:");
            c.AppendLine("\n");
            c.AppendLine(">RawTools -d C:\\MyRawFiles -p -m");
            c.AppendLine("");
            c.AppendLine("The above command processes all the raw files in C:\\MyRawFiles and writes a parse table and MGF file " +
                "to the same directory.");
            c.AppendLine("\n");
            c.AppendLine(">RawTools -d C:\\MyRawFiles -pm -o C:\\MyRawFiles\\results");
            c.AppendLine("");
            c.AppendLine("The above command similarly processes all the raw files in C:\\MyRawFiles, but this time it will write " +
                "the parse table and MGF file to C:\\MyRawFiles\\results. Note that -p and -m have been \"stacked\" into a single argument -pm. " +
                "This can be done with any arguments which do not require text after them.");
            c.AppendLine("\n");
            c.AppendLine(">RawTools -f C:\\MyRawFiles\\file1.raw -m");
            c.AppendLine("");
            c.AppendLine("The above command reads the single raw file C:\\MyRawFiles\\file1.raw and writes and MGF into the containing directory.");
            c.AppendLine("\n");
            c.AppendLine(">RawTools -d C:\\MyRawFiles -qmx -r TMT10 -o C:\\MyRawFiles\\results");
            c.AppendLine("");
            c.AppendLine("The above command processes all the files in C:\\MyRawFiles and writes a parse table including reporter ion " +
                "quant values and MGF and metrics files to C:\\MyRawFiles\\results. Because we have indicated with -q that we want " +
                "to quantify reporter ions, we need to also include -r to indicate what labeling reagents were used. In this case, TMT10.");
            c.AppendLine("For more detailed information on specific arguments, invoke \">RawTools -help\".");
            c.AppendLine("\n");
            c.AppendLine("=== qc ===");
            c.AppendLine("\n");
            c.AppendLine("The QC functionality of RawTools has the following general syntax:");
            c.AppendLine("");
            c.AppendLine(">RawTools -d [raw file directory] -qc [qc data directory] [further options]");
            c.AppendLine("");
            c.AppendLine("Note that QC and Parse functionalities can be combined. Any of the options from the parse section above " +
                "can be added to the QC syntax as additional options.");
            c.AppendLine("");
            c.AppendLine("-d is the directory containing raw files and -qc is the directory where you want the QC data to be stored. " +
                "They can be the same directory if you desire. Invoking -d and -qc with no further arguments will processes the raw " +
                "files using RawTools only, and the general metrics of each file will be included in the QC results. If you wish to " +
                "include identification-related metrics, read on.");
            c.AppendLine("");
            c.AppendLine("To get identification-related metrics, you need to use the -X argument to indicate the directory " +
                "containing the X! Tandem executable. For example:");
            c.AppendLine("");
            c.AppendLine(">RawTools -d C:\\MyRawFiles -qc C:\\MyRawFiles\\QC -db C:\\FastaFiles\\current_human.fasta -s xtandem " +
                "-X C:\\tandem\\bin");
            c.AppendLine("");
            c.AppendLine("If you invoke the use of X! Tandem, you can also indicate peptide modifications using the -fmods and -vmods " +
                "arguments. For examples of these, please see \"RawTools -modifications\"");
            c.AppendLine("");
            c.AppendLine("For more detailed information on specific arguments, invoke \">RawTools -help\".");

            Console.Write(c);
        }
    }
}
