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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermoFisher;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data;
using RawTools.Data.IO;
using RawTools.Data.Collections;
using RawTools.Data.Extraction;
using RawTools.Data.Containers;
using RawTools.Utilities;
using RawTools.Algorithms;
using RawTools.QC;
using RawTools.WorkFlows;
using RawTools.Algorithms.Analyze;
using RawTools.Algorithms.ExtractData;
using RawTools.Utilities.MathStats;
using System.Xml.Linq;
using Serilog;
//using Serilog.Sinks.File;
using CommandLine;
//using CommandLine.Text;

namespace RawTools
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix | Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                Console.Out.NewLine = "\n\n";
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("rawtools_log.txt", rollOnFileSizeLimit: true, retainedFileCountLimit: 3, fileSizeLimitBytes: 2097152)
                .CreateLogger();

            Log.Information("Program started with arguments: {0}", String.Join(" ", args));


            Parser.Default.ParseArguments<ArgumentParser.ParseOptions, ArgumentParser.QcOptions, ArgumentParser.TestOptions, ArgumentParser.ExampleMods, ArgumentParser.LogDumpOptions>(args)
                .WithParsed<ArgumentParser.ParseOptions>(opts => DoStuff(opts))
                .WithParsed<ArgumentParser.QcOptions>(opts => DoStuff(opts))
                .WithParsed<ArgumentParser.TestOptions>(opts => DoStuff(opts))
                .WithParsed<ArgumentParser.ExampleMods>(opts => DoStuff(opts))
                .WithParsed<ArgumentParser.LogDumpOptions>(opts => DoStuff(opts))
                .WithNotParsed((errs) => HandleParseError(args));

            Log.CloseAndFlush();
        }

        static int DoStuff(ArgumentParser.LogDumpOptions opts)
        {
            List<string> files = new List<string>();

            if (opts.InputFiles.Count() > 0) // did the user give us a list of files?
            {
                List<string> problems = new List<string>();
                files = opts.InputFiles.ToList();

                // check if the list provided contains only .raw files
                foreach (string file in files)
                {

                    if (!file.EndsWith(".raw", StringComparison.OrdinalIgnoreCase))
                    {
                        problems.Add(file);
                    }
                }

                if (problems.Count() == 1)
                {
                    Console.WriteLine("\nERROR: {0} does not appear to be a .raw file. Invoke '>RawTools --help' if you need help.", problems.ElementAt(0));
                    Log.Error("Invalid file provided: {0}", problems.ElementAt(0));

                    return 1;
                }

                if (problems.Count() > 1)
                {
                    Console.WriteLine("\nERROR: The following {0} files do not appear to be .raw files. Invoke '>RawTools --help' if you need help." +
                        "\n\n{1}", problems.Count(), String.Join("\n", problems));
                    Log.Error("Invalid files provided: {0}", String.Join(" ", problems));
                    return 1;
                }

                Log.Information("Files to be processed, provided as list: {0}", String.Join(" ", files));
            }

            else // did the user give us a directory?
            {
                if (Directory.Exists(opts.InputDirectory))
                {
                    files = Directory.GetFiles(opts.InputDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".raw", StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else
                {
                    Console.WriteLine("ERROR: The provided directory does not appear to be valid.");
                    Log.Error("Invalid directory provided: {0}", opts.InputDirectory);
                    return 1;
                }

                Log.Information("Files to be processed, provided as directory: {0}", String.Join(" ", files));

            }

            Console.WriteLine();

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
            return 0;
        }

        static int DoStuff(ArgumentParser.ExampleMods opts)
        {
            SearchQC.ExampleMods();
            return 0;
        }

        static int DoStuff(ArgumentParser.TestOptions opts)
        {
            var rawFile = RawFileReaderFactory.ReadFile(opts.InputFiles.First());
            rawFile.SelectInstrument(Device.MS, 1);

            var created = rawFile.CreationDate;
            var modified = rawFile.FileHeader.ModifiedDate;
            var diff = modified - created;
            var estTime = rawFile.RunHeader.ExpectedRuntime;

            var timesModified = rawFile.FileHeader.NumberOfTimesModified;

            Console.WriteLine("=============================================");
            Console.WriteLine($"Creation date/time: {created}");
            Console.WriteLine($"Last modified date/time: {modified}");
            Console.WriteLine($"Number of times modified: {timesModified}");
            Console.WriteLine($"Total time: {diff}");
            Console.WriteLine($"Expected run time: {estTime}");
            Console.WriteLine();

            Console.WriteLine($"Estimated dead time: {diff.TotalMinutes - estTime}");
            Console.WriteLine("=============================================");
            /*
            var numberOfLogs = rawFile.GetStatusLogEntriesCount();
            var logInfo = rawFile.GetStatusLogHeaderInformation();

            string logName = opts.InputFiles.First() + ".INST_LOG.txt";

            Dictionary<int, ISingleValueStatusLog> log = new Dictionary<int, ISingleValueStatusLog>();

            for (int i = 0; i < logInfo.Count(); i++)
            {
                log.Add(i, rawFile.GetStatusLogAtPosition(i));
            }

            using (StreamWriter f = new StreamWriter(logName))
            {
                ProgressIndicator P = new ProgressIndicator(numberOfLogs, "Writing instrument log");
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
                            f.Write("{0}\t",log[j].Values[i]);
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

            Console.WriteLine(rawFile.GetInstrumentMethod(0));
            Console.WriteLine(rawFile.CreationDate);
            Console.WriteLine(rawFile.FileHeader.ModifiedDate);
            Console.WriteLine(rawFile.RunHeader.StartTime);
            Console.WriteLine(rawFile.RunHeader.EndTime);
            */
            return 0;
        }

        static int DoStuff(ArgumentParser.QcOptions opts)
        {
            Log.Information("Starting QC. Identipy: {Identipy}", opts.Identipy);
            //Console.WriteLine("\n");
            
            /*
            if (new List<string>() { "DDA", "DIA", "PRM" }.Contains(opts.ExperimentType))
            { }
            else
            {
                Log.Error("Experiment type of {ExpType} was passed", opts.ExperimentType);
                Console.WriteLine("Experiment type must be one of ['DDA', 'DIA', 'PRM'], not {0}", opts.ExperimentType);
                Environment.Exit(1);
            }*/

            if (opts.SearchAlgorithm != null & !(new List<string>() { "identipy", "xtandem", "None" }.Contains(opts.SearchAlgorithm)))
            {
                // the search algorithm is not null but it also it not identipy or xtandem
                Log.Error("Invalid search algorithm argument: {Argument}", opts.SearchAlgorithm);
                Console.WriteLine("ERROR: Search algorithm must be one of {identipy, xtandem}");
                return 1;
            }

            if (opts.Identipy)
            {
                opts.SearchAlgorithm = "identipy";
            }

            WorkflowParameters parameters = new WorkflowParameters(opts);


            if (opts.SearchAlgorithm != "None")
            {
                if (opts.FastaDatabase == null)
                {
                    Log.Error("No fasta database provided for Identipy search");
                    Console.WriteLine("ERROR: A fasta protein database is required for a database search. Please use the --db parameter to " +
                        "provide the path to a database.");
                    Environment.Exit(1);
                }
                
                if (opts.SearchAlgorithm == "identipy")
                {
                    if ((opts.IdentipyScript == null & opts.PythonExecutable != null) | (opts.IdentipyScript != null & opts.PythonExecutable == null))
                    {
                        Log.Error("If providing location of python or identipy, must specify both of them.");
                        Console.WriteLine("ERROR: When invoking the -p or -I options, you must supply both of them.");
                        Environment.Exit(1);
                    }

                    Identipy.CheckIdentipyDependencies(parameters);
                    
                }

                if (opts.SearchAlgorithm == "xtandem")
                {
                    if (opts.XTandemDirectory == null)
                    {
                        Log.Error("Path to XTandem directory was not provided");
                        Console.WriteLine("ERROR: You must specify the X! Tandem directory using the -X argument to perform a search using X! Tandem.");
                        return 1;
                    }
                }
            }

            QC.QcWorkflow.DoQc(parameters);
            
            return 0;
        }

        static int HandleParseError(string[] args)
        {
            Log.Error("Error occured during command line parsing. Arguments given: {0}", String.Join(" ", args));
            return 1;
        }

        static int DoStuff(ArgumentParser.ParseOptions opts)
        {
            List<string> files = new List<string>();

            if (opts.InputFiles.Count() > 0) // did the user give us a list of files?
            {
                List<string> problems = new List<string>();
                files = opts.InputFiles.ToList();

                // check if the list provided contains only .raw files
                foreach (string file in files)
                {
                    
                    if (!file.EndsWith(".raw", StringComparison.OrdinalIgnoreCase))
                    {
                        problems.Add(file);
                    }
                }

                if (problems.Count() == 1)
                {
                    Console.WriteLine("\nERROR: {0} does not appear to be a .raw file. Invoke '>RawTools --help' if you need help.", problems.ElementAt(0));
                    Log.Error("Invalid file provided: {0}", problems.ElementAt(0));

                    return 1;
                }

                if (problems.Count() > 1)
                {
                    Console.WriteLine("\nERROR: The following {0} files do not appear to be .raw files. Invoke '>RawTools --help' if you need help." +
                        "\n\n{1}", problems.Count(), String.Join("\n", problems));
                    Log.Error("Invalid files provided: {0}", String.Join(" ", problems));
                    return 1;
                }

                Log.Information("Files to be processed, provided as list: {0}", String.Join(" ", files));
            }

            else // did the user give us a directory?
            {
                if (Directory.Exists(opts.InputDirectory))
                {
                    files = Directory.GetFiles(opts.InputDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".raw", StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else
                {
                    Console.WriteLine("ERROR: The provided directory does not appear to be valid.");
                    Log.Error("Invalid directory provided: {0}", opts.InputDirectory);
                    return 1;
                }

                Log.Information("Files to be processed, provided as directory: {0}", String.Join(" ", files));

            }

            if (opts.Quant)
            {
                List<string> possible = new List<string>() { "TMT0", "TMT2", "TMT6", "TMT10", "TMT11", "iTRAQ4", "iTRAQ8" };
                if (!possible.Contains(opts.LabelingReagents))
                {
                    Console.WriteLine("ERROR: For quantification, the labeling reagent must be one of {TMT0, TMT2, TMT6, TMT10, TMT11, iTRAQ4, iTRAQ8}");
                    Log.Error("Invalid labeling reagent provided: {0}", opts.LabelingReagents);
                    return 1;
                }
            }

            if (opts.Chromatogram != null)
            {
                List<string> possible = new List<string>() { "1T", "2T", "3T", "1B", "2B", "3B", "1TB", "2TB", "3TB", "1TB", "2TB", "3TB"};
                if (!possible.Contains(opts.Chromatogram))
                {
                    Console.WriteLine("ERROR: Incorrect format for --chro. See help.");
                    Log.Error("Invalid chromatogram argument provided: {Chro}", opts.Chromatogram);
                    return 1;
                }
            }
            /*
            // is the experiment type valid?
            if (! new List<string>() { "DDA", "DIA", "PRM" }.Contains(opts.ExperimentType))
            {
                Log.Error("Experiment type of {ExpType} was passed", opts.ExperimentType);
                Console.WriteLine("Experiment type must be one of ['DDA', 'DIA', 'PRM'], not {0}", opts.ExperimentType);
                Environment.Exit(1);
            }*/


            System.Diagnostics.Stopwatch singleFileTime = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch totalTime = new System.Diagnostics.Stopwatch();
            totalTime.Start();

            WorkflowParameters parameters = new WorkflowParameters(opts);

            foreach (string file in files)
            {
                singleFileTime.Start();

                Console.WriteLine("\nProcessing: {0}\n", file);

                //using (IRawDataPlus rawFile = RawFileReaderFactory.ReadFile(fileName:file))
                using (IRawFileThreadManager rawFile = RawFileReaderFactory.CreateThreadManager(file))
                {
                    if (parameters.ParseParams.OutputDirectory == null)
                    {
                        parameters.ParseParams.OutputDirectory = Path.GetDirectoryName(file);
                    }

                    if (parameters.ExpType == ExperimentType.DDA)
                    {
                        WorkFlowsDDA.ParseDDA(rawFile, parameters);
                    }
                    else if (parameters.ExpType == ExperimentType.DIA)
                    {
                        WorkFlowsDIA.ParseDIA(rawFile, parameters);
                    }
                }

                singleFileTime.Stop();
                Console.WriteLine("\nElapsed time: {0} s", Math.Round(Convert.ToDouble(singleFileTime.ElapsedMilliseconds)/1000.0,2));
                singleFileTime.Reset();
            }
            totalTime.Stop();
            Console.WriteLine("\nTime to process all {0} files: {1}", files.Count(), totalTime.Elapsed);

            return 0;
        }
    }
}
