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
using RawTools.Data.Processing;
using RawTools.Data.Collections;
using RawTools.Data.Extraction;
using RawTools.Data.Containers;
using RawTools.Utilities;
using RawTools.QC;
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
            if(Environment.OSVersion.Platform == PlatformID.Unix | Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                Console.Out.NewLine = "\n\n";
            }
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("rawtools_log.txt", rollOnFileSizeLimit: true, retainedFileCountLimit: 3, fileSizeLimitBytes: 2097152)
                .CreateLogger();

            Log.Information("Program started with arguments: {0}", String.Join(" ", args));

            /*
            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                Console.WriteLine("MacOSX system detected. Sorry, RawTools is not compatible with MacOSX at this time");
                Log.Error("System is MacOSX");
                Environment.Exit(1);
            }
            */

            Parser.Default.ParseArguments<ArgumentParser.ParseOptions, ArgumentParser.QcOptions, ArgumentParser.TestOptions, ArgumentParser.ExampleMods>(args)
                .WithParsed<ArgumentParser.ParseOptions>(opts => DoStuff(opts))
                .WithParsed<ArgumentParser.QcOptions>(opts => DoStuff(opts))
                .WithParsed<ArgumentParser.TestOptions>(opts => DoStuff(opts))
                .WithParsed<ArgumentParser.ExampleMods>(opts => DoStuff(opts))
                .WithNotParsed((errs) => HandleParseError(args));
               /*
            Parser.Default.ParseArguments<ArgumentParser.ParseOptions, ArgumentParser.QcOptions>(args)
              .MapResult(
                (ArgumentParser.ParseOptions opts) => DoStuff(opts),
                (ArgumentParser.QcOptions opts) => DoStuff(opts),
                errs => HandleParseError(args));
                */

            Log.CloseAndFlush();
        }

        static int DoStuff(ArgumentParser.ExampleMods opts)
        {
            Identipy.ExampleMods();
            return 0;
        }

        static int DoStuff(ArgumentParser.TestOptions opts)
        {
            string[] tempMods = new string[] { "1", "2", "3" };
            string vmods = tempMods.Aggregate((i, j) => i + "," + j);
            Console.WriteLine(vmods);

            return 0;
        }

        static int DoStuff(ArgumentParser.QcOptions opts)
        {
            Log.Information("Starting QC. Identipy: {Identipy}", opts.Identipy);
            //Console.WriteLine("\n");
            SearchParameters idpyPars;

            if (opts.Identipy)
            {
                if (opts.FastaDatabase == null)
                {
                    Log.Error("No fasta database provided for Identipy search");
                    Console.WriteLine("ERROR: A fasta protein database is required for an Identipy search. Please use the --db parameter to " +
                        "provide the path to a database.");
                    Environment.Exit(1);
                }
                if ((opts.IdentipyScript == null & opts.PythonExecutable != null) | (opts.IdentipyScript != null & opts.PythonExecutable == null))
                {
                    Log.Error("If providing location of python or identipy, must specify both of them.");
                    Console.WriteLine("ERROR: When invoking the -p or -I options, you must supply both of them.");
                    Environment.Exit(1);
                }

                idpyPars = new SearchParameters();
                idpyPars.PythonExecutable = opts.PythonExecutable;
                idpyPars.IdentipyScript = opts.IdentipyScript;
                idpyPars.FastaDatabase = opts.FastaDatabase;
                idpyPars.FixedMods = opts.FixedMods;
                idpyPars.NMod = opts.VariableNMod;
                idpyPars.KMod = opts.VariableKMod;
                idpyPars.XMod = opts.VariableXMod;
                idpyPars.NumSpectra = opts.NumberSpectra;
                idpyPars.MgfIntensityCutoff = opts.IntensityCutoff;
                idpyPars.MgfMassCutoff = opts.MassCutOff;

                Identipy.CheckIdentipyDependencies(idpyPars);

            }
            else
            {
                idpyPars = null;
            }

            
            QC.QC.DoQc(dataDirectory: opts.DirectoryToQc, qcDirectory: opts.QcDirectory, idpyPars: idpyPars);
            
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

            System.Diagnostics.Stopwatch singleFileTime = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch totalTime = new System.Diagnostics.Stopwatch();
            totalTime.Start();

            foreach (string file in files)
            {
                singleFileTime.Start();

                Console.WriteLine("\nProcessing: {0}\n", file);

                using (IRawDataPlus rawFile = RawFileReaderFactory.ReadFile(fileName:file))
                {
                    rawFile.SelectInstrument(Device.MS, 1);

                    Log.Information("Now processing: {File} --- Instrument: {Instrument}", Path.GetFileName(file), rawFile.GetInstrumentData().Name);

                    RawDataCollection rawData = new RawDataCollection(rawFile:rawFile);
                    QuantDataCollection quantData = new QuantDataCollection();

                    bool isBoxCar = rawData.isBoxCar;

                    if (rawData.isBoxCar)
                    {
                        Console.WriteLine("\nRaw file appears to be a boxcar-type experiment. Precursor peak analysis won't be performed!\n");
                    }

                    if (opts.ParseData | opts.Metrics | opts.Quant)
                    {
                        rawData.ExtractAll(rawFile);

                        if (!isBoxCar)
                        {
                            rawData.CalcPeakRetTimesAndInts(rawFile: rawFile);
                        }
                    }

                    if (opts.Quant)
                    {
                        rawData.quantData.Quantify(rawData:rawData, rawFile:rawFile, labelingReagent:opts.LabelingReagents);
                    }

                    if (opts.UnlabeledQuant & !isBoxCar)
                    {
                        rawData.QuantifyPrecursorPeaks(rawFile);
                    }                    

                    if (opts.Metrics)
                    {
                        rawData.metaData.AggregateMetaData(rawData, rawFile);
                    }

                    if (opts.ParseData | opts.Quant)
                    {
                        if (opts.Quant)
                        {
                            Parse.WriteMatrix(rawData: rawData, rawFile: rawFile, metaData: rawData.metaData, quantData: rawData.quantData, outputDirectory: opts.OutputDirectory);
                        }
                        else
                        {
                            Parse.WriteMatrix(rawData: rawData, rawFile: rawFile, metaData: rawData.metaData, outputDirectory: opts.OutputDirectory);
                        }
                    }                    

                    if (opts.WriteMGF)
                    {
                        MGF.WriteMGF(rawData: rawData, rawFile: rawFile, outputDirectory: opts.OutputDirectory, cutoff: opts.MassCutOff,
                            intensityCutoff: opts.IntensityCutoff);
                    }

                    if (opts.Metrics)
                    {
                        MetricsData metricsData = new MetricsData();

                        if (opts.Quant)
                        {
                            metricsData.GetMetricsData(metaData: rawData.metaData, rawData: rawData, rawFile: rawFile, quantData: rawData.quantData);
                        }
                        else
                        {
                            metricsData.GetMetricsData(metaData: rawData.metaData, rawData: rawData, rawFile: rawFile);
                        }

                        metricsData.GetMetricsData(metaData: rawData.metaData, rawData: rawData, rawFile: rawFile);
                        Metrics.WriteMatrix(rawData, metricsData, opts.OutputDirectory);
                    }

                    if (opts.Chromatogram != null)
                    {
                        int order = Convert.ToInt32((opts.Chromatogram.ElementAt(0).ToString()));

                        if (order > (int)rawData.methodData.AnalysisOrder)
                        {
                            Log.Error("Specified MS order ({Order}) for chromatogram is higher than experiment order ({ExpOrder})",
                                (MSOrderType)order, rawData.methodData.AnalysisOrder);
                            Console.WriteLine("Specified MS order ({0}) for chromatogram is higher than experiment order ({1}). Chromatogram(s) won't be written.",
                                (MSOrderType)order, rawData.methodData.AnalysisOrder);
                        }
                        else
                        {
                        rawData.WriteChromatogram(rawFile, (MSOrderType)order, opts.Chromatogram.Contains("T"), opts.Chromatogram.Contains("B"), opts.OutputDirectory);
                        }
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
