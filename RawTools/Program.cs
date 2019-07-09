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
using CLParser;
//using Serilog.Sinks.File;

namespace RawTools
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            if (Environment.OSVersion.Platform == PlatformID.Unix | Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                Console.Out.NewLine = "\n\n";
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("rawtools_log.txt", rollOnFileSizeLimit: true, retainedFileCountLimit: 3, fileSizeLimitBytes: 2097152)
                .CreateLogger();

            Log.Information("Program started with arguments: {0}", String.Join(" ", args));

            ClParser parser = ArgumentParser.ParserForRawTools.Create();

            var arguments = parser.Parse(args);

            Run(arguments);

            Log.CloseAndFlush();
        }

        static int Run(Dictionary<string, object> opts)
        {
            if ((bool)opts["ExampleCommands"] == true)
            {
                Examples.CommandLineUsage();
                Environment.Exit(0);
            }

            if ((bool)opts["ExampleModifications"] == true)
            {
                Examples.ExampleMods();
                Environment.Exit(0);
            }

            List<string> files = new List<string>();
            QcDataCollection qcDataCollection = new QcDataCollection();

            WorkflowParameters parameters = new WorkflowParameters(opts);

            if (parameters.InputFiles != null) // did the user give us a list of files?
            {
                List<string> problems = new List<string>();
                files = parameters.InputFiles.ToList();

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
                    //Console.Write("Press any key to exit...");
                    //Console.ReadKey();
                    return 1;
                }

                if (problems.Count() > 1)
                {
                    Console.WriteLine("\nERROR: The following {0} files do not appear to be .raw files. Invoke '>RawTools --help' if you need help." +
                        "\n\n{1}", problems.Count(), String.Join("\n", problems));
                    Log.Error("Invalid files provided: {0}", String.Join(" ", problems));
                    //Console.Write("Press any key to exit...");
                    //Console.ReadKey();
                    return 1;
                }

                files = RawFileInfo.RemoveInAcquistionFiles(files);

                // if the file location(s) are relative, we need to get the absolute path to them
                files.EnsureAbsolutePaths();

                Log.Information("Files to be processed, provided as list: {0}", String.Join(" ", files));
            }

            else if (!String.IsNullOrEmpty(parameters.RawFileDirectory)) // did the user give us a directory?
            {
                // if QC is being done, use the QC method snf get the qc data collection at the same time
                if (parameters.QcParams.QcDirectory != null)
                {
                    (files, qcDataCollection) = QcWorkflow.GetFileListAndQcFile(parameters, parameters.IncludeSubdirectories);
                }

                // if not, the parse method
                else if (Directory.Exists(parameters.RawFileDirectory))
                {
                    files = Directory.GetFiles(parameters.RawFileDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.EndsWith(".raw", StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else
                {
                    Console.WriteLine("ERROR: The provided directory does not appear to be valid.");
                    Log.Error("Invalid directory provided: {0}", parameters.RawFileDirectory);
                    //Console.Write("Press any key to exit...");
                    //Console.ReadKey();
                    return 1;
                }

                files = RawFileInfo.RemoveInAcquistionFiles(files);

                // if the file location(s) are relative, we need to get the absolute path to them
                files.EnsureAbsolutePaths();

                Log.Information("Files to be processed, provided as directory: {0}", String.Join(" ", files));
            }
            else
            {
                Console.WriteLine("ERROR: At least one of the following arguments is required: -f, -d");
                Log.Error("No raw files or directory specified.");
                return 1;
            }

            if (parameters.ParseParams.Quant)
            {
                List<string> possible = new List<string>() { "TMT0", "TMT2", "TMT6", "TMT10", "TMT11", "iTRAQ4", "iTRAQ8" };
                if (!possible.Contains(parameters.ParseParams.LabelingReagents))
                {
                    Console.WriteLine("ERROR: For quantification, the labeling reagent must be one of {TMT0, TMT2, TMT6, TMT10, TMT11, iTRAQ4, iTRAQ8}");
                    Log.Error("Invalid labeling reagent provided: {0}", parameters.ParseParams.LabelingReagents);
                    //Console.Write("Press any key to exit...");
                    //Console.ReadKey();
                    return 1;
                }
            }

            if (parameters.ParseParams.Chromatogram != null)
            {
                List<string> possible = new List<string>() { "1", "2", "3", "T", "B" };
                foreach (var x in parameters.ParseParams.Chromatogram)
                {
                    if (!possible.Contains(x.ToString()))
                    {
                        Console.WriteLine("ERROR: Incorrect format for -chro. See help.");
                        Log.Error("Invalid chromatogram argument provided: {Chro}", parameters.ParseParams.Chromatogram);
                        //Console.Write("Press any key to exit...");
                        //Console.ReadKey();
                        return 1;
                    }
                }
            }

            System.Diagnostics.Stopwatch singleFileTime = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch totalTime = new System.Diagnostics.Stopwatch();
            totalTime.Start();
            
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

                    WorkFlowsDDA.UniversalDDA(rawFile, parameters, qcDataCollection);
                }

                singleFileTime.Stop();
                Console.WriteLine("\nElapsed time: {0} s", Math.Round(Convert.ToDouble(singleFileTime.ElapsedMilliseconds) / 1000.0, 2));
                singleFileTime.Reset();
            }

            if (parameters.LogDump)
            {
                Write.LogDump.WriteToDisk(parameters);
            }

            totalTime.Stop();
            Console.WriteLine("\nTime to process all {0} files: {1}", files.Count(), totalTime.Elapsed);

            //Console.Write("Press any key to exit...");
            //Console.ReadKey();

            return 0;
        }
    }
}
