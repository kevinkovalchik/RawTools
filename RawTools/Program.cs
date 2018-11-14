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
using RawTools.Algorithms.MatchBewteen;
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
            if(Environment.OSVersion.Platform == PlatformID.Unix | Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                Console.Out.NewLine = "\n\n";
            }
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("rawtools_log.txt", rollOnFileSizeLimit: true, retainedFileCountLimit: 3, fileSizeLimitBytes: 2097152)
                .CreateLogger();

            Log.Information("Program started with arguments: {0}", String.Join(" ", args));
            

            Parser.Default.ParseArguments<ArgumentParser.ParseOptions, ArgumentParser.QcOptions, ArgumentParser.TestOptions, ArgumentParser.ExampleMods>(args)
                .WithParsed<ArgumentParser.ParseOptions>(opts => DoStuff(opts))
                .WithParsed<ArgumentParser.QcOptions>(opts => DoStuff(opts))
                .WithParsed<ArgumentParser.TestOptions>(opts => DoStuff(opts))
                .WithParsed<ArgumentParser.ExampleMods>(opts => DoStuff(opts))
                .WithNotParsed((errs) => HandleParseError(args));

            Log.CloseAndFlush();
        }

        static int DoStuff(ArgumentParser.ExampleMods opts)
        {
            SearchQC.ExampleMods();
            return 0;
        }

        static int DoStuff(ArgumentParser.TestOptions opts)
        {
            List<string> files = Directory.GetFiles(opts.Directory, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".raw", StringComparison.OrdinalIgnoreCase)).ToList();

            WorkflowParameters parameters = new WorkflowParameters();
            parameters.QcParams.NumberSpectra = 1000000;
            parameters.QcParams.FixedScans = true;
            parameters.QcParams.QcDirectory = opts.Directory + "/SearchData";
            parameters.QcParams.SearchAlgorithm = SearchAlgorithm.XTandem;
            parameters.QcParams.XMod = "15.99491@M";
            parameters.QcParams.FixedMods = opts.FixedMods;
            parameters.QcParams.XTandemDirectory = opts.XTandemDirectory;
            parameters.QcParams.FastaDatabase = opts.FastaDatabase;

            if (!RawFileInfo.CheckIfValid(files[0]) | !RawFileInfo.CheckIfValid(files[1]))
            {
                if (!RawFileInfo.CheckIfValid(files[0]))
                {
                    Console.WriteLine("{0} is not a valid raw file.", files[0]);
                }

                if (!RawFileInfo.CheckIfValid(files[1]))
                {
                    Console.WriteLine("{0} is not a valid raw file.", files[1]);
                }
                Environment.Exit(1);
            }

            IRawFileThreadManager rawFile1 = RawFileReaderFactory.CreateThreadManager(fileName: files[0]);
            IRawFileThreadManager rawFile2 = RawFileReaderFactory.CreateThreadManager(fileName: files[1]);

            var staticRawFile1 = rawFile1.CreateThreadAccessor();
            var staticRawFile2 = rawFile2.CreateThreadAccessor();
            
            staticRawFile1.SelectInstrument(Device.MS, 1);
            staticRawFile2.SelectInstrument(Device.MS, 1);

            staticRawFile1.CheckIfBoxcar();
            staticRawFile2.CheckIfBoxcar();

            ScanIndex Index1 = Extract.ScanIndices(rawFile1.CreateThreadAccessor());

            TrailerExtraCollection trailerExtras1 = Extract.TrailerExtras(rawFile1.CreateThreadAccessor(), Index1);

            MethodDataContainer methodData1 = Extract.MethodData(rawFile1.CreateThreadAccessor(), Index1);

            (CentroidStreamCollection centroidStreams1, SegmentScanCollection segmentScans1) =
                Extract.MsData(rawFile: rawFile1.CreateThreadAccessor(), index: Index1);

            (PrecursorScanCollection precursorScans1, ScanDependentsCollections scanDependents1) = Extract.DependentsAndPrecursorScansByScanDependents(rawFile1.CreateThreadAccessor(), Index1);

            PrecursorMassCollection precursorMasses1 = Extract.PrecursorMasses(rawFile1.CreateThreadAccessor(), precursorScans1, trailerExtras1, Index1);

            RetentionTimeCollection retentionTimes1 = Extract.RetentionTimes(rawFile1.CreateThreadAccessor(), Index1);

            if (opts.RefineMassCharge)
            {
                ProgressIndicator P = new ProgressIndicator(Index1.ScanEnumerators[MSOrderType.Ms2].Length, "Refining precursor charge state and monoisotopic mass");
                P.Start();
                int refinedCharge;
                double refinedMass;


                foreach (int scan in Index1.ScanEnumerators[MSOrderType.Ms2])
                {
                    // refine precursor mass and charge
                    (refinedCharge, refinedMass) =
                        MonoIsoPredictor.GetMonoIsotopicMassCharge(centroidStreams1[precursorScans1[scan].MasterScan],
                        precursorMasses1[scan].ParentMZ, trailerExtras1[scan].ChargeState);
                    trailerExtras1[scan].ChargeState = refinedCharge;
                    precursorMasses1[scan].MonoisotopicMZ = refinedMass;
                    P.Update();
                }
                P.Done();
            }

            PrecursorPeakCollection peakData1 = AnalyzePeaks.AnalyzeAllPeaks(centroidStreams1, retentionTimes1, precursorMasses1, precursorScans1, Index1);
            
            if (!opts.StaticSearch)
            {
                Search.WriteSearchMGF(parameters, centroidStreams1, segmentScans1, retentionTimes1, precursorMasses1, precursorScans1, trailerExtras1, methodData1,
                        Index1, staticRawFile1.FileName, parameters.QcParams.FixedScans);

                Search.RunSearch(parameters, methodData1, staticRawFile1.FileName);
            }

            ScanIndex Index2 = Extract.ScanIndices(rawFile2.CreateThreadAccessor());

            TrailerExtraCollection trailerExtras2 = Extract.TrailerExtras(rawFile2.CreateThreadAccessor(), Index2);

            MethodDataContainer methodData2 = Extract.MethodData(rawFile2.CreateThreadAccessor(), Index2);

            (CentroidStreamCollection centroidStreams2, SegmentScanCollection segmentScans2) =
                Extract.MsData(rawFile: rawFile2.CreateThreadAccessor(), index: Index2);

            (PrecursorScanCollection precursorScans2, ScanDependentsCollections scanDependents2) = Extract.DependentsAndPrecursorScansByScanDependents(rawFile2.CreateThreadAccessor(), Index2);

            PrecursorMassCollection precursorMasses2 = Extract.PrecursorMasses(rawFile2.CreateThreadAccessor(), precursorScans2, trailerExtras2, Index2);

            RetentionTimeCollection retentionTimes2 = Extract.RetentionTimes(rawFile2.CreateThreadAccessor(), Index2);

            if (opts.RefineMassCharge)
            {
                ProgressIndicator P = new ProgressIndicator(Index2.ScanEnumerators[MSOrderType.Ms2].Length, "Refining precursor charge state and monoisotopic mass");
                P.Start();
                int refinedCharge;
                double refinedMass;


                foreach (int scan in Index2.ScanEnumerators[MSOrderType.Ms2])
                {
                    // refine precursor mass and charge
                    (refinedCharge, refinedMass) =
                        MonoIsoPredictor.GetMonoIsotopicMassCharge(centroidStreams2[precursorScans2[scan].MasterScan],
                        precursorMasses2[scan].ParentMZ, trailerExtras2[scan].ChargeState);
                    trailerExtras2[scan].ChargeState = refinedCharge;
                    precursorMasses2[scan].MonoisotopicMZ = refinedMass;
                    P.Update();
                }
                P.Done();
            }

            PrecursorPeakCollection peakData2 = AnalyzePeaks.AnalyzeAllPeaks(centroidStreams2, retentionTimes2, precursorMasses2, precursorScans2, Index2);


            if (!opts.StaticSearch)
            {
                Search.WriteSearchMGF(parameters, centroidStreams2, segmentScans2, retentionTimes2, precursorMasses2, precursorScans2, trailerExtras2, methodData2,
                    Index2, staticRawFile2.FileName, parameters.QcParams.FixedScans);

                Search.RunSearch(parameters, methodData2, staticRawFile2.FileName);
            }
            

            PsmDataCollection psms1 = MatchBetween.LoadPsmData(retentionTimes1, precursorScans1, parameters, staticRawFile1.FileName);
            PsmDataCollection psms2 = MatchBetween.LoadPsmData(retentionTimes2, precursorScans2, parameters, staticRawFile2.FileName);

            Ms1FeatureCollection features1 = MatchBetween.AggregateMs1Features(peakData1, psms1, precursorMasses1);
            Ms1FeatureCollection features2 = MatchBetween.AggregateMs1Features(peakData2, psms2, precursorMasses2);

            //using (var f = new StreamWriter(Path.Combine(staticRawFile1.FileName + ".massdrift.txt")))
            //{
            //    foreach (var x in features1) if (x.Value.PSM != null) f.WriteLine("{0}\t{1}", retentionTimes1[x.Value.Peak.Ms2Scan], x.Value.PSM.MassDrift);
            //}

            //MultiRunFeatureCollection features = AlignTimeAndMass.MatchFeatures(psms1, psms2, peakData1, peakData2, precursorMasses1,
            //    precursorMasses2, opts.TimePercentTol, opts.MassPPM);

            //MultiRunFeatureCollection features = AlignTimeAndMass.CorrelateFeatures(psms1, psms2, peakData1, peakData2, precursorMasses1, precursorMasses2, opts.TimePercentTol, opts.MassPPM);

            if (opts.Align)
            {
                AlignRetentionTimes.AlignRT(features1, features2, segmentScans1, segmentScans2, opts.ExpectationValue);
            }

            MultiRunFeatureCollection features = MatchBetween.CorrelateFeatures2(features1, features2, segmentScans1, segmentScans2, opts.TimePercentTol, opts.MassPPM);

            XCorr.ScoreMultiRunSpectra(features, segmentScans1, segmentScans2);
            StreamWriter score = new StreamWriter(Path.Combine(opts.Directory, "AllScores.csv"));
            List<double> scores = new List<double>();
            foreach (var feature in features) scores = scores.Concat(feature.Value.AllScores.Values).ToList();
            foreach (var s in scores) score.WriteLine(s);
            score.Close();
            score = new StreamWriter(Path.Combine(opts.Directory, "Matched.csv"));
            foreach (var feature in features) if (feature.Value.ConfirmSeqMatch) score.WriteLine(feature.Value.XCorr);
            score.Close();

            int NoId = 0;
            int OnlyIn1 = 0;
            int IdIn1FeatureIn2 = 0;
            int IdInBoth = 0;
            int FeatureIn1IdIn2 = 0;
            int OnlyIn2 = 0;
            int SeqMatch = 0;

            StreamWriter NeitherID = new StreamWriter(Path.Combine(opts.Directory, "IdInNeither.csv"));
            StreamWriter BothID = new StreamWriter(Path.Combine(opts.Directory, "IdInBoth.csv"));
            StreamWriter IdInFile1 = new StreamWriter(Path.Combine(opts.Directory, "IdInFile1.csv"));
            StreamWriter IdInFile2 = new StreamWriter(Path.Combine(opts.Directory, "IdInFile2.csv"));
            NeitherID.WriteLine("RT1,RT2,Mass1,Mass2");
            BothID.WriteLine("RT1,RT2,Mass1,Mass2");
            IdInFile1.WriteLine("RT1,RT2,Mass1,Mass2");
            IdInFile2.WriteLine("RT1,RT2,Mass1,Mass2");

            List<(int scan1, int scan2)> IdInNeither = new List<(int scan1, int scan2)>();
            List<(int scan1, int scan2)> BothIdScans = new List<(int scan1, int scan2)>();
            List<(int scan1, int scan2)> IdInFile1Scans = new List<(int scan1, int scan2)>();
            List<(int scan1, int scan2)> IdInFile2Scans = new List<(int scan1, int scan2)>();

            foreach (var feature in features.Values)
            {
                if (feature.IdIn1 & feature.FoundIn1 & feature.IdIn2 & feature.FoundIn2)
                {
                    IdInBoth += 1;
                    BothID.WriteLine("{0},{1},{2},{3}", feature.RT1, feature.RT2, feature.Mass1, feature.Mass2);
                    BothIdScans.Add((feature.Ms2Scan1, feature.Ms2Scan2));

                    if (feature.ConfirmSeqMatch)
                    {
                        SeqMatch += 1;
                    }
                }
                else if (feature.IdIn1 & feature.FoundIn1 & feature.FoundIn2 & !feature.IdIn2)
                {
                    IdIn1FeatureIn2 += 1;
                    IdInFile1.WriteLine("{0},{1},{2},{3}", feature.RT1, feature.RT2, feature.Mass1, feature.Mass2);
                    IdInFile1Scans.Add((feature.Ms2Scan1, feature.Ms2Scan2));

                }
                else if (feature.IdIn1 & feature.FoundIn1 & !feature.FoundIn2 & !feature.FoundIn2)
                {
                    OnlyIn1 += 1;
                }
                else if (feature.IdIn2 & feature.FoundIn2 & feature.FoundIn1 & !feature.IdIn1)
                {
                    FeatureIn1IdIn2 += 1;
                    IdInFile2.WriteLine("{0},{1},{2},{3}", feature.RT1, feature.RT2, feature.Mass1, feature.Mass2);
                    IdInFile2Scans.Add((feature.Ms2Scan1, feature.Ms2Scan2));
                }
                else if (feature.IdIn2 & feature.FoundIn2 & !feature.FoundIn1 & !feature.IdIn1)
                {
                    OnlyIn2 += 1;
                }
                else if (feature.FoundIn1 & feature.FoundIn2 & !feature.IdIn1 & !feature.IdIn2)
                {
                    NoId += 1;
                    NeitherID.WriteLine("{0},{1},{2},{3}", feature.RT1, feature.RT2, feature.Mass1, feature.Mass2);
                    IdInNeither.Add((feature.Ms2Scan1, feature.Ms2Scan2));
                }
            }

            BothID.Close();
            IdInFile1.Close();
            IdInFile2.Close();

            Console.WriteLine("\n");
            Console.WriteLine("Features ID'd in both: {0}", IdInBoth);
            Console.WriteLine("Features ID'd in neither: {0}", NoId);
            Console.WriteLine("Features found only in file 1: {0}", OnlyIn1);
            Console.WriteLine("Features found only in file 2: {0}", OnlyIn2);
            Console.WriteLine("Features found in both, but only ID'd in file 1: {0}", IdIn1FeatureIn2);
            Console.WriteLine("Features found in both, but only ID'd in file 2: {0}", FeatureIn1IdIn2);
            Console.WriteLine("Confirmed sequence matches: {0}%", Convert.ToDouble(SeqMatch) / IdInBoth * 100);

            using (StreamWriter f = new StreamWriter(Path.Combine(opts.Directory, "results.txt")))
            {
                f.WriteLine("File 1: {0}", staticRawFile1.FileName);
                f.WriteLine("File 2: {0}", staticRawFile2.FileName);
                f.WriteLine();

                f.WriteLine("Features ID'd in both: {0}", IdInBoth);
                f.WriteLine("Features found only in file 1: {0}", OnlyIn1);
                f.WriteLine("Features found only in file 2: {0}", OnlyIn2);
                f.WriteLine("Features found in both, but only ID'd in file 1: {0}", IdIn1FeatureIn2);
                f.WriteLine("Features found in both, but only ID'd in file 2: {0}", FeatureIn1IdIn2);
                f.WriteLine("Confirmed sequence matches: {0}%", Convert.ToDouble(SeqMatch) / IdInBoth * 100);
            }

            var centroids = new List<CentroidStreamCollection>() { centroidStreams1, centroidStreams2 };
            var segments = new List<SegmentScanCollection>() { segmentScans1, segmentScans2 };
            var retentionTimes = new List<RetentionTimeCollection>() { retentionTimes1, retentionTimes2 };
            var precursorMasses = new List<PrecursorMassCollection>() { precursorMasses1, precursorMasses2 };
            var trailerExtras = new List<TrailerExtraCollection>() { trailerExtras1, trailerExtras2 };
            var rawFileNames = new List<string>() { staticRawFile1.FileName, staticRawFile2.FileName };
            var featuresFromBoth = new List<Ms1FeatureCollection>() { features1, features2 };

            MatchedMgfWriter.WriteMGF(parameters, BothIdScans, Path.Combine(opts.Directory, "IdInBoth.mgf"), featuresFromBoth, centroids, segments,
                retentionTimes, precursorMasses, trailerExtras, methodData1, rawFileNames);

            MatchedMgfWriter.WriteMGF(parameters, IdInFile1Scans, Path.Combine(opts.Directory, "IdInFile1.mgf"), featuresFromBoth, centroids, segments,
                retentionTimes, precursorMasses, trailerExtras, methodData1, rawFileNames);

            MatchedMgfWriter.WriteMGF(parameters, IdInFile2Scans, Path.Combine(opts.Directory, "IdInFile2.mgf"), featuresFromBoth, centroids, segments,
                retentionTimes, precursorMasses, trailerExtras, methodData1, rawFileNames);

            return 0;
        }

        static int DoStuff(ArgumentParser.QcOptions opts)
        {
            Log.Information("Starting QC. Identipy: {Identipy}", opts.Identipy);
            //Console.WriteLine("\n");
            
            if (new List<string>() { "DDA", "DIA", "PRM" }.Contains(opts.ExperimentType))
            { }
            else
            {
                Log.Error("Experiment type of {ExpType} was passed", opts.ExperimentType);
                Console.WriteLine("Experiment type must be one of ['DDA', 'DIA', 'PRM'], not {0}", opts.ExperimentType);
                Environment.Exit(1);
            }

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

            // is the experiment type valid?
            if (! new List<string>() { "DDA", "DIA", "PRM" }.Contains(opts.ExperimentType))
            {
                Log.Error("Experiment type of {ExpType} was passed", opts.ExperimentType);
                Console.WriteLine("Experiment type must be one of ['DDA', 'DIA', 'PRM'], not {0}", opts.ExperimentType);
                Environment.Exit(1);
            }


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
