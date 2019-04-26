using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using System.IO;
using RawTools.Utilities;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;

namespace RawTools.Write
{
    static class LogDump
    {
        public static void WriteToDisk(WorkFlows.WorkflowParameters opts)
        {
            List<string> files = new List<string>();
            
            Console.WriteLine();

            Console.WriteLine("Writing log files");

            foreach (var file in files)
            {
                Console.WriteLine(Path.GetFileName(file));
                Console.WriteLine("----------------------------------------");
                using (IRawDataPlus rawFile = RawFileReaderFactory.ReadFile(file))
                {
                    rawFile.SelectMsData();

                    var numberOfLogs = rawFile.GetStatusLogEntriesCount();
                    var logInfo = rawFile.GetStatusLogHeaderInformation();

                    string logName = file + ".INST_LOG.txt";

                    Dictionary<int, ISingleValueStatusLog> log = new Dictionary<int, ISingleValueStatusLog>();

                    ProgressIndicator P = new ProgressIndicator(logInfo.Count(), "Preparing log data");
                    P.Start();
                    for (int i = 0; i < logInfo.Count(); i++)
                    {
                        log.Add(i, rawFile.GetStatusLogAtPosition(i));
                        P.Update();
                    }
                    P.Done();

                    using (StreamWriter f = new StreamWriter(logName))
                    {
                        P = new ProgressIndicator(numberOfLogs, $"Writing log");
                        P.Start();
                        f.Write("Time\t");
                        foreach (var x in logInfo)
                        {
                            f.Write(x.Label + "\t");
                        }
                        f.Write("\n");

                        for (int i = 0; i < numberOfLogs; i++)
                        {
                            f.Write($"{log[0].Times[i]}\t");

                            for (int j = 0; j < logInfo.Length; j++)
                            {
                                try
                                {
                                    f.Write("{0}\t", log[j].Values[i]);
                                }
                                catch (Exception)
                                {
                                    f.Write("\t");
                                }
                            }
                            f.Write("\n");
                            P.Update();
                        }
                        P.Done();
                    }
                }
                Console.WriteLine("\n");
            }
        }
    }
}
