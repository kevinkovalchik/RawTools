using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using RawTools.Utilities;

namespace RawTools.QC
{
    public static class FastaManipulation
    {
        /// <summary>
        /// Takes an input fasta file and writes a target-decoy database
        /// </summary>
        /// <param name="fastaDB">The input database</param>
        /// <param name="nameOut">If not null, indicates the ouput filename</param>
        public static void ReverseDecoy(string fastaDB, string outputFile = null)
        {
            Console.WriteLine("Reading FASTA file");
            List<string> fasta = File.ReadAllLines(fastaDB).ToList();
            List<char> sequence = new List<char>();
            string decoy = "DECOY_";
            string decoy_header;
            int width = 60;
            string nameOut;
            ProgressIndicator P = new ProgressIndicator(fasta.Count() * 2, "Writing target-decoy database");

            if (outputFile == null)
            {
                nameOut = fastaDB + ".TARGET_DECOY.fasta";
            }
            else
            {
                nameOut = outputFile;
            }
            P.Start();

            using (StreamWriter f = new StreamWriter(nameOut))
            {
                // first write the forward DB
                foreach (string line in fasta)
                {
                    f.WriteLine(line);
                    P.Update();
                }

                // now for the decoy

                // Write the first line
                f.Write(fasta[0]);
                // remove it
                fasta.RemoveAt(0);

                foreach (string line in fasta)
                {
                    if (line[0] == '>')
                    {
                        // the line is the next header, before writing it we need to reverse and write the sequence of the previous protein
                        sequence.Reverse();
                        for (int i = 0; i < sequence.Count(); i++)
                        {
                            if (i % width == 0)
                            {
                                f.Write('\n');
                            }
                            f.Write(sequence[i]);                            
                        }
                        f.Write('\n');

                        // add DECOY_ to the header
                        decoy_header = line.Insert(1, decoy);

                        // write the header
                        f.Write(decoy_header);

                        // erase the current sequence
                        sequence = new List<char>();
                    }
                    else
                    {
                        // the line is not a header, so add it to the sequence list
                        foreach (var aa in line)
                        {
                            sequence.Add(aa);
                        }
                    }
                    P.Update();
                }
                P.Done();
            }
        }

    }
}
