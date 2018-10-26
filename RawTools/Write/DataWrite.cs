﻿// Copyright 2018 Kevin Kovalchik & Christopher Hughes
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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RawTools.Data.Containers;
using RawTools.Data.Collections;
using RawTools.Data.Extraction;
using RawTools.Utilities;
using RawTools.QC;
using ThermoFisher;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;
using RawTools.WorkFlows;
using Serilog;

namespace RawTools.Data.IO
{
    class ParseWriter
    {
        //private List<string> data;
        static private string fileName;
        static private ScanIndex index;
        static private CentroidStreamCollection centroidStreams;
        static private SegmentScanCollection segmentScans;
        static private ScanMetaDataCollectionDDA metaDataDDA;
        static private ScanMetaDataCollectionDIA metaDataDIA;
        static private QuantDataCollection quantData;
        static private RetentionTimeCollection retentionTimes;
        static private PrecursorMassCollection precursorMasses;
        static private PrecursorScanCollection precursorScans;
        static private PrecursorPeakCollection precursorPeaks;
        static private TrailerExtraCollection trailerExtras;
        static private WorkflowParameters parameters;
        static private MethodDataContainer methodData;

        private delegate void WriteToFile(int scan, StreamWriter f);
        static private Dictionary<string, WriteToFile> Delegates = new Dictionary<string, WriteToFile>()
        {
            {"MS3ScanNumber", WriterDelegates.MS3ScanNumber },
            {"MS2ScanNumber", WriterDelegates.MS2ScanNumber },
            {"MS1ScanNumber", WriterDelegates.MS1ScanNumber },
            {"QuantScanRetTime", WriterDelegates.QuantScanRetTime },
            {"ParentScanRetTime", WriterDelegates.ParentScanRetTime },
            {"DutyCycle", WriterDelegates.DutyCycle },
            {"MS2ScansPerCycle", WriterDelegates.MS2ScansPerCycle },
            {"ParentIonMass", WriterDelegates.ParentIonMass },
            {"MonoisotopicMass", WriterDelegates.MonoisotopicMass },
            {"PrecursorCharge", WriterDelegates.PrecursorCharge },
            {"MS1IsolationInterference", WriterDelegates.MS1IsolationInterference },
            {"ParentPeakFound", WriterDelegates.ParentPeakFound },
            {"ParentPeakArea", WriterDelegates.ParentPeakArea },
            {"PeakFirstScan", WriterDelegates.PeakFirstScan },
            {"PeakMaxScan", WriterDelegates.PeakMaxScan },
            {"PeakLastScan", WriterDelegates.PeakLastScan },
            {"BaseLinePeakWidth(s)", WriterDelegates.BaseLinePeakWidth },
            {"PeakParentScanIntensity", WriterDelegates.PeakParentScanIntensity },
            {"PeakMaxIntensity", WriterDelegates.PeakMaxIntensity },
            {"MS1IonInjectionTime", WriterDelegates.MS1IonInjectionTime },
            {"MS2IonInjectionTime", WriterDelegates.MS2IonInjectionTime },
            {"MS3IonInjectionTime", WriterDelegates.MS3IonInjectionTime },
            {"HCDEnergy", WriterDelegates.HCDEnergy },
            {"MS1MedianIntensity", WriterDelegates.MS1MedianIntensity },
            {"MS2MedianIntensity", WriterDelegates.MS2MedianIntensity }
        };


        /// <summary>
        /// Writer for DDA data
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="centroids"></param>
        /// <param name="segments"></param>
        /// <param name="metaData"></param>
        /// <param name="retentionTimes"></param>
        /// <param name="precursorMasses"></param>
        /// <param name="precursorScans"></param>
        /// <param name="precursorPeaks"></param>
        /// <param name="trailerExtras"></param>
        /// <param name="quantData"></param>
        public ParseWriter(string fileName, CentroidStreamCollection centroids, SegmentScanCollection segments,
            ScanMetaDataCollectionDDA metaData, RetentionTimeCollection retentionTimes, PrecursorMassCollection precursorMasses,
            PrecursorScanCollection precursorScans, PrecursorPeakCollection precursorPeaks, TrailerExtraCollection trailerExtras,
            ScanIndex Index, QuantDataCollection quantData = null)
        {
            ParseWriter.fileName = fileName;
            index = Index;

            centroidStreams = centroids;
            segmentScans = segments;
            metaDataDDA = metaData;
            ParseWriter.quantData = quantData;
            ParseWriter.retentionTimes = retentionTimes;
            ParseWriter.precursorMasses = precursorMasses;
            ParseWriter.precursorScans = precursorScans;
            ParseWriter.trailerExtras = trailerExtras;
            ParseWriter.precursorPeaks = precursorPeaks;
        }

        /// <summary>
        /// Writer for DIA data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <param name="centroids"></param>
        /// <param name="segments"></param>
        /// <param name="metaData"></param>
        /// <param name="retentionTimes"></param>
        /// <param name="precursorMasses"></param>
        /// <param name="precursorScans"></param>
        /// <param name="precursorPeaks"></param>
        /// <param name="trailerExtras"></param>
        /// <param name="quantData"></param>
        public ParseWriter(string fileName, CentroidStreamCollection centroids, SegmentScanCollection segments,
            ScanMetaDataCollectionDIA metaData, RetentionTimeCollection retentionTimes, TrailerExtraCollection trailerExtras,
            ScanIndex Index)
        {
            ParseWriter.fileName = fileName;
            ParseWriter.index = Index;
            centroidStreams = centroids;
            segmentScans = segments;
            metaDataDIA = metaData;
            ParseWriter.retentionTimes = retentionTimes;
            ParseWriter.trailerExtras = trailerExtras;
        }

        public ParseWriter(CentroidStreamCollection centroids, SegmentScanCollection segments, WorkflowParameters parameters, RetentionTimeCollection retentionTimes,
            PrecursorMassCollection precursorMasses, PrecursorScanCollection precursorScans, TrailerExtraCollection trailerExtras, MethodDataContainer methodData,
            ScanIndex Index)
        {
            centroidStreams = centroids;
            segmentScans = segments;
            ParseWriter.parameters = parameters;
            ParseWriter.retentionTimes = retentionTimes;
            ParseWriter.precursorMasses = precursorMasses;
            ParseWriter.precursorScans = precursorScans;
            ParseWriter.trailerExtras = trailerExtras;
            ParseWriter.methodData = methodData;
            ParseWriter.index = Index;
        }

        static class WriterDelegates
        {

            public static void MS3ScanNumber(int scan, StreamWriter f)
            {
                f.Write(precursorScans[scan].MS3Scan);
            }

            public static void MS2ScanNumber(int scan, StreamWriter f)
            {
                f.Write(precursorScans[scan].MS2Scan);
            }

            public static void MS1ScanNumber(int scan, StreamWriter f)
            {
                f.Write(precursorScans[scan].MasterScan);
            }

            public static void QuantScanRetTime(int scan, StreamWriter f)
            {
                f.Write(retentionTimes[scan]);
            }

            public static void ParentScanRetTime(int scan, StreamWriter f)
            {
                f.Write(retentionTimes[precursorScans[scan].MasterScan]);
            }

            public static void MS2ScansPerCycle(int scan, StreamWriter f)
            {
                f.Write(metaDataDDA.MS2ScansPerCycle[precursorScans[scan].MasterScan]);
            }

            public static void DutyCycle(int scan, StreamWriter f)
            {
                f.Write(metaDataDDA.DutyCycle[precursorScans[scan].MasterScan]);
            }

            public static void ParentIonMass(int scan, StreamWriter f)
            {
                f.Write(precursorMasses[precursorScans[scan].MS2Scan].ParentMZ);
            }

            public static void MonoisotopicMass(int scan, StreamWriter f)
            {
                f.Write(precursorMasses[precursorScans[scan].MS2Scan].MonoisotopicMZ);
            }

            public static void PrecursorCharge(int scan, StreamWriter f)
            {
                f.Write(trailerExtras[precursorScans[scan].MS2Scan].ChargeState);
            }

            public static void MS1IsolationInterference(int scan, StreamWriter f)
            {
                f.Write(metaDataDDA.Ms1IsolationInterference[scan]);
            }

            public static void ParentPeakFound(int scan, StreamWriter f)
            {
                var test = precursorPeaks;
                f.Write(precursorPeaks[precursorScans[scan].MS2Scan].PeakFound);
            }

            public static void ParentPeakArea(int scan, StreamWriter f)
            {
                f.Write(precursorPeaks[precursorScans[scan].MS2Scan].Area);
            }

            public static void PeakFirstScan(int scan, StreamWriter f)
            {
                f.Write(precursorPeaks[precursorScans[scan].MS2Scan].FirstScan);
            }

            public static void PeakMaxScan(int scan, StreamWriter f)
            {
                f.Write(precursorPeaks[precursorScans[scan].MS2Scan].MaxScan);
            }

            public static void PeakLastScan(int scan, StreamWriter f)
            {
                f.Write(precursorPeaks[precursorScans[scan].MS2Scan].LastScan);
            }

            public static void BaseLinePeakWidth(int scan, StreamWriter f)
            {
                f.Write(precursorPeaks[precursorScans[scan].MS2Scan].BaselineWidth);
            }

            public static void PeakParentScanIntensity(int scan, StreamWriter f)
            {
                f.Write(precursorPeaks[precursorScans[scan].MS2Scan].ParentIntensity);
            }

            public static void PeakMaxIntensity(int scan, StreamWriter f)
            {
                f.Write(precursorPeaks[precursorScans[scan].MS2Scan].MaximumIntensity);
            }

            public static void MS1IonInjectionTime(int scan, StreamWriter f)
            {
                f.Write(metaDataDDA.FillTime[precursorScans[scan].MasterScan]);
            }

            public static void MS2IonInjectionTime(int scan, StreamWriter f)
            {
                f.Write(metaDataDDA.FillTime[precursorScans[scan].MS2Scan]);
            }

            public static void MS3IonInjectionTime(int scan, StreamWriter f)
            {
                f.Write(metaDataDDA.FillTime[precursorScans[scan].MS3Scan]);
            }

            public static void HCDEnergy(int scan, StreamWriter f)
            {
                f.Write(trailerExtras[scan].HCDEnergy);
            }

            public static void MS1MedianIntensity(int scan, StreamWriter f)
            {
                f.Write(metaDataDDA.IntensityDistribution[precursorScans[scan].MasterScan].P50);
            }

            public static void MS2MedianIntensity(int scan, StreamWriter f)
            {
                f.Write(metaDataDDA.IntensityDistribution[precursorScans[scan].MS2Scan].P50);
            }
        }
        
        private static void WriteMatrix(List<string> Data)
        {
            using (StreamWriter f = new StreamWriter(fileName)) //Open a new file
            {
                var scans = index.ScanEnumerators[index.AnalysisOrder];

                for (int i = 0; i < Data.Count(); i++)
                {
                    f.Write(Data[i] + "\t");
                }
                if (quantData != null)
                {
                    string reagents = quantData.LabelingReagents;
                    foreach (string label in new LabelingReagents().Reagents[reagents].Labels)
                    {
                        f.Write(label + "Intensity\t");
                    }
                    foreach (string label in new LabelingReagents().Reagents[reagents].Labels)
                    {
                        f.Write(label + "Mass\t");
                    }
                    foreach (string label in new LabelingReagents().Reagents[reagents].Labels)
                    {
                        f.Write(label + "Noise\t");
                    }
                    foreach (string label in new LabelingReagents().Reagents[reagents].Labels)
                    {
                        f.Write(label + "Resolution\t");
                    }
                    foreach (string label in new LabelingReagents().Reagents[reagents].Labels)
                    {
                        f.Write(label + "Baseline\t");
                    }
                }
                f.Write("\n");

                ProgressIndicator P = new ProgressIndicator(scans.Count(), "Writing output table");
                P.Start();

                foreach (int scan in scans)
                {

                    for (int i = 0; i < Data.Count(); i++)
                    {
                        Delegates[Data[i]](scan, f);
                        f.Write("\t");
                    }

                    if (quantData != null)
                    {
                        foreach (string label in quantData[scan].Keys)
                        {
                            f.Write(quantData[scan][label].Intensity + "\t");
                        }
                        foreach (string label in quantData[scan].Keys)
                        {
                            f.Write(quantData[scan][label].Mass + "\t");
                        }
                        foreach (string label in quantData[scan].Keys)
                        {
                            f.Write(quantData[scan][label].Noise + "\t");
                        }
                        foreach (string label in quantData[scan].Keys)
                        {
                            f.Write(quantData[scan][label].Resolution + "\t");
                        }
                        foreach (string label in quantData[scan].Keys)
                        {
                            f.Write(quantData[scan][label].Baseline + "\t");
                        }
                    }
                    f.Write("\n");
                    P.Update();
                }
                P.Done();
            }
        }

        public void WriteMatrixDDA(MSOrderType order)
        {
            List<string> data = new List<string>();

            if (order == MSOrderType.Ms3)
            {
                data = new List<string>
                {
                "MS3ScanNumber", "MS2ScanNumber", "MS1ScanNumber", "QuantScanRetTime", "ParentScanRetTime", "DutyCycle",
                "MS2ScansPerCycle", "ParentIonMass", "MonoisotopicMass", "PrecursorCharge", "MS1IsolationInterference",
                "ParentPeakFound", "ParentPeakArea", "PeakFirstScan", "PeakMaxScan", "PeakLastScan", "BaseLinePeakWidth(s)",
                "PeakParentScanIntensity", "PeakMaxIntensity", "MS1IonInjectionTime", "MS2IonInjectionTime",
                "MS3IonInjectionTime", "HCDEnergy", "MS1MedianIntensity", "MS2MedianIntensity"
                };
            }
            else if (order == MSOrderType.Ms2)
            {
                data = new List<string>
                {
                "MS2ScanNumber", "MS1ScanNumber", "QuantScanRetTime", "ParentScanRetTime", "DutyCycle",
                "MS2ScansPerCycle", "ParentIonMass", "MonoisotopicMass", "PrecursorCharge", "MS1IsolationInterference",
                "ParentPeakFound", "ParentPeakArea", "PeakFirstScan", "PeakMaxScan", "PeakLastScan", "BaseLinePeakWidth(s)",
                "PeakParentScanIntensity", "PeakMaxIntensity", "MS1IonInjectionTime", "MS2IonInjectionTime",
                "HCDEnergy", "MS1MedianIntensity", "MS2MedianIntensity"
                };
            }
            else
            {
                Log.Error("Incompatible ms order: {MSOrder}", order);
            }

            WriteMatrix(data);
        }

        public void WriteMatrixDIA()
        {
            List<string> data = new List<string>();

            data = new List<string>
                {
                "MS2ScanNumber", "MS1ScanNumber", "QuantScanRetTime", "ParentScanRetTime", "DutyCycle",
                "MS2ScansPerCycle", "MS1IonInjectionTime", "MS2IonInjectionTime",
                "HCDEnergy", "MS1MedianIntensity", "MS2MedianIntensity"
                };

            WriteMatrix(data);
        }

        public void WriteMGF(string rawFileName, int[] scans = null, string outputFile = null)
        {
            string fileName;

            double intCutoff = 0;
            if (outputFile == null)
            {
                fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, ".mgf");
            }
            else
            {
                fileName = outputFile;
            }

            MassAnalyzerType ms2MassAnalyzer = methodData.MassAnalyzers[MSOrderType.Ms2];
            
            const int BufferSize = 65536;  // 64 Kilobytes

            using (StreamWriter f = new StreamWriter(fileName, false, Encoding.UTF8, BufferSize)) //Open a new file, the MGF file
            {
                // if the scans argument is null, use all scans
                if (scans == null)
                {
                    scans = index.ScanEnumerators[MSOrderType.Ms2];
                }

                ProgressIndicator progress = new ProgressIndicator(scans.Count(), String.Format("Writing MGF file"));

                foreach (int i in scans)
                {
                    f.WriteLine("\nBEGIN IONS");
                    f.WriteLine("RAWFILE={0}", rawFileName);
                    f.WriteLine("TITLE=Spectrum_{0}", i);
                    f.WriteLine("SCAN={0}", i);
                    f.WriteLine("RTINSECONDS={0}", retentionTimes[i]);
                    f.WriteLine("PEPMASS={0}", precursorMasses[i].MonoisotopicMZ);
                    f.WriteLine("CHARGE={0}", trailerExtras[i].ChargeState);

                    if (ms2MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        CentroidStreamData centroid = centroidStreams[i];

                        if (centroid.Intensities.Length > 0)
                        {
                            intCutoff = centroid.Intensities.Max() * parameters.MgfIntensityCutoff;
                        }
                        else
                        {
                            intCutoff = 0;
                        }

                        for (int j = 0; j < centroid.Masses.Length; j++)
                        {
                            //f.WriteLine(Math.Round(centroid.Masses[j], 4).ToString() + " " + Math.Round(centroid.Intensities[j], 4).ToString());
                            if (centroid.Masses[j] > parameters.MgfMassCutoff & centroid.Intensities[j] > intCutoff)
                            {
                                f.WriteLine("{0} {1}", Math.Round(centroid.Masses[j], 5), Math.Round(centroid.Intensities[j], 4));
                            }
                        }
                    }
                    else
                    {
                        SegmentedScanData segments = segmentScans[i];

                        if (segments.Intensities.Length > 0)
                        {
                            intCutoff = segments.Intensities.Max() * parameters.MgfIntensityCutoff;
                        }
                        else
                        {
                            intCutoff = 0;
                        }

                        for (int j = 0; j < segments.Positions.Length; j++)
                        {
                            if (segments.Positions[j] > parameters.MgfMassCutoff & segments.Intensities[j] > intCutoff)
                            {
                                f.WriteLine("{0} {1}", Math.Round(segments.Positions[j], 5), Math.Round(segments.Intensities[j], 4));
                            }
                        }
                    }

                    f.WriteLine("END IONS");

                    progress.Update();
                }
                progress.Done();
            }
        }
    }

    static class MetricsWriter
    {
        public static void WriteMatrix(RawMetricsDataDDA metrics, SearchMetricsContainer searchMetrics, string rawFileName, string outputDirectory = null)
        {
            string fileName = ReadWrite.GetPathToFile(outputDirectory, rawFileName, "_Metrics.txt");

            using (StreamWriter f = new StreamWriter(fileName)) //Open a new file
            {
                f.WriteLine("RawFile:\t" + metrics.RawFileName);
                f.WriteLine("Instrument:\t" + metrics.Instrument);
                f.WriteLine("ExperimentOrder:\t" + metrics.MSOrder);
                f.WriteLine("MS1Analyzer:\t" + metrics.MS1Analyzer);
                f.WriteLine("MS2Analyzer:\t" + metrics.MS2Analyzer);

                if (metrics.MSOrder == MSOrderType.Ms3)
                {
                    f.WriteLine("MS3Analyzer:\t" + metrics.MS3Analyzer);
                }
                else
                {
                    f.WriteLine("MS3Analyzer:\tNone");
                }

                f.WriteLine("TotalAnalysisTime(min):\t" + Math.Round(metrics.TotalAnalysisTime, 4));
                f.WriteLine("TotalScans:\t" + metrics.TotalScans);
                f.WriteLine("MS1Scans:\t" + metrics.MS1Scans);
                f.WriteLine("MS2Scans:\t" + metrics.MS2Scans);
                f.WriteLine("MS3Scans:\t" + metrics.MS3Scans);
                f.WriteLine("MeanTopN:\t" + Math.Round(metrics.MeanTopN, 4));
                f.WriteLine("MS1ScanRate(/sec):\t" + Math.Round(metrics.MS1ScanRate / 60, 4));
                f.WriteLine("MS2ScanRate(/sec):\t" + Math.Round(metrics.MS2ScanRate / 60, 4));
                f.WriteLine("MeanDutyCycle:\t" + Math.Round(metrics.MeanDutyCycle, 4));
                f.WriteLine("MedianMS1FillTime:\t" + Math.Round(metrics.MedianMS1FillTime, 4));
                f.WriteLine("MedianMS2FillTime:\t" + Math.Round(metrics.MedianMS2FillTime, 4));
                f.WriteLine("MedianMS3FillTime:\t" + Math.Round(metrics.MedianMS3FillTime, 4));
                f.WriteLine("MedianMS2Intensity:\t" + Math.Round(metrics.MedianSummedMS2Intensity, 4));
                f.WriteLine("MedianMS1IsolationInterference:\t" + Math.Round(metrics.MedianMs1IsolationInterference, 4));
                f.WriteLine("MedianPeakWidthAt10Percent(s):\t" + Math.Round(metrics.MedianBaselinePeakWidth * 60, 4));
                f.WriteLine("MedianPeakWidthAtHalfMax(s):\t" + Math.Round(metrics.MedianHalfHeightPeakWidth * 60, 4));
                f.WriteLine("MedianAsymmetryFactor:\t" + Math.Round(metrics.MedianAsymmetryFactor, 4));
                f.WriteLine("ColumnCapacity:\t" + Math.Round(metrics.PeakCapacity, 4));
                f.WriteLine("NumberOfEsiStabilityFlags:\t" + metrics.NumberOfEsiFlags);

                if (searchMetrics != null)
                {
                    f.Write(searchMetrics.MedianMassDrift + ",");

                    //f.Write(qcData.QcData[key].QuantMeta.medianReporterIntensity + ",");

                    f.WriteLine("IdentificationRate:\t" + Math.Round(searchMetrics.IdentificationRate, 4));
                    f.WriteLine("DigestionEfficiency\t" + Math.Round(searchMetrics.DigestionEfficiency, 4));
                    f.WriteLine("MissedCleavageRate\t" + Math.Round(searchMetrics.MissedCleavageRate, 4));

                    f.WriteLine("LabelingEfficiencyAtNTerm\t" + Math.Round(searchMetrics.LabelingEfficiencyAtNTerm, 4));
                    f.WriteLine("LabelingEfficiencyAtK\t" + Math.Round(searchMetrics.LabelingEfficiencyAtK, 4));
                    f.WriteLine("LabelingEfficiencyAtX\t" + Math.Round(searchMetrics.LabelingEfficiencyAtX, 4));
                    f.WriteLine("XLabel:\t", searchMetrics.LabelX);

                    f.WriteLine("PsmChargeRatio3to2\t" + Math.Round(searchMetrics.ChargeRatio3to2, 4));
                    f.WriteLine("PsmChargeRatio4to2\t" + Math.Round(searchMetrics.ChargeRatio4to2, 4));
                }

                if (metrics.IncludesQuant == true)
                {
                    foreach (string tag in metrics.QuantMeta.quantTags)
                    {
                        f.WriteLine("{0}MedianReporterIonIntensity:\t{1}", tag, metrics.QuantMeta.medianReporterIntensityByChannel[tag]);
                    }
                    f.WriteLine("OverallMedianReporterIonIntensity:\t{0}", metrics.QuantMeta.medianReporterIntensity);
                }
            }
        }

        public static void WriteMatrix(RawMetricsDataDIA metrics, string rawFileName, string outputDirectory = null)
        {
            string fileName = ReadWrite.GetPathToFile(outputDirectory, rawFileName, "_Metrics.txt");

            using (StreamWriter f = new StreamWriter(fileName)) //Open a new file
            {
                f.WriteLine("RawFile:\t" + metrics.RawFileName);
                f.WriteLine("Instrument:\t" + metrics.Instrument);
                f.WriteLine("ExperimentOrder:\t" + metrics.MSOrder);
                f.WriteLine("MS1Analyzer:\t" + metrics.MS1Analyzer);
                f.WriteLine("MS2Analyzer:\t" + metrics.MS2Analyzer);
                
                f.WriteLine("TotalAnalysisTime(min):\t" + Math.Round(metrics.TotalAnalysisTime, 4));
                f.WriteLine("TotalScans:\t" + metrics.TotalScans);
                f.WriteLine("MS1Scans:\t" + metrics.MS1Scans);
                f.WriteLine("MS2Scans:\t" + metrics.MS2Scans);
                f.WriteLine("MS1ScanRate(/sec):\t" + Math.Round(metrics.MS1ScanRate / 60, 4));
                f.WriteLine("MS2ScanRate(/sec):\t" + Math.Round(metrics.MS2ScanRate / 60, 4));
                f.WriteLine("MeanDutyCycle:\t" + Math.Round(metrics.MeanDutyCycle, 4));
                f.WriteLine("MedianMS1FillTime:\t" + Math.Round(metrics.MedianMS1FillTime, 4));
                f.WriteLine("MedianMS2FillTime:\t" + Math.Round(metrics.MedianMS2FillTime, 4));
                f.WriteLine("MedianMS2Intensity:\t" + Math.Round(metrics.MedianSummedMS2Intensity, 4));
            }
        }
    }

    static class QcWriter
    {
        public static void WriteQcToTable(this QcDataCollectionDDA qcData)
        {
            if (File.Exists(Path.Combine(qcData.QcDirectory, "QcDataTable.csv")))
            {
                while (Utilities.ReadWrite.IsFileLocked(Path.Combine(qcData.QcDirectory, "QcDataTable.csv")))
                {
                    Console.WriteLine("{0} is in use. Please close the file and press any key to continue.", Path.Combine(qcData.QcDirectory, "QcDataTable.csv"));
                    Console.ReadKey();
                }
            }

            using (StreamWriter f = new StreamWriter(Path.Combine(qcData.QcDirectory, "QcDataTable.csv"), append: false))
            {
                f.WriteLine("DateAcquired,FileName,Instrument,ExperimentMsOrder,Ms1Analyzer,Ms2Analyzer,Ms3Analyzer,TotalAnalysisTime,TotalScans,NumMs1Scans,NumMs2Scans," +
                    "NumMs3Scans,Ms1ScanRate(/s),Ms2ScanRate(/s),Ms3ScanRate(/s),MeanDutyCycle(s),MeanMs2TriggerRate(/Ms1Scan),Ms1MedianSummedIntensity,Ms2MedianSummedIntensity," +
                    "MedianPrecursorIntensity,MedianMs1IsolationInterence,MedianMs2PeakFractionConsumingTop80PercentTotalIntensity,EsiInstabilityFlags(count),MedianMassDrift(ppm)," +
                    "IdentificationRate(IDs/Ms2Scan),DigestionEfficiency,MissedCleavageRate(/PSM),ModificationFrequencyAtNTerm,ModificationFrequencyAtK,ModificationFrequencyAtX," +
                    "MedianMsFillTime(ms),MedianMs2FillTime(ms),MedianMs3FillTime(ms),WidthAt10%H(s),WidthAt50%H(s),AsymmetryAt10%H,AsymmetryAt50%H," +
                    "PeakCapacity,TimeBeforeFirstExceedanceOf10%MaxIntensity,TimeAfterLastExceedanceOf10%MaxIntensity,FractionOfRunAbove10%MaxIntensity,IdChargeRatio3to2,IdChargeRatio4to2,IdentipyParameters");
            }

            List<DateTime> keys = qcData.QcData.Keys.ToList();
            keys.Sort();

            using (StreamWriter f = new StreamWriter(Path.Combine(qcData.QcDirectory, "QcDataTable.csv"), append: true))
            {
                foreach (DateTime key in keys)
                {
                    f.Write(qcData.QcData[key].SearchMetrics.DateAcquired + ",");
                    f.Write(qcData.QcData[key].SearchMetrics.RawFile + ",");
                    f.Write(qcData.QcData[key].SearchMetrics.Instrument + ",");

                    f.Write(qcData.QcData[key].RawMetrics.MSOrder + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MS1Analyzer + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MS2Analyzer + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MS3Analyzer + ",");

                    f.Write(qcData.QcData[key].RawMetrics.Gradient + ",");
                    f.Write(qcData.QcData[key].RawMetrics.TotalScans + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MS1Scans + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MS2Scans + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MS3Scans + ",");

                    f.Write(qcData.QcData[key].RawMetrics.MS1ScanRate / 60 + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MS2ScanRate / 60 + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MS3ScanRate / 60 + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MeanDutyCycle + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MeanTopN + ",");

                    f.Write(qcData.QcData[key].RawMetrics.MedianSummedMS1Intensity + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MedianSummedMS2Intensity + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MedianPrecursorIntensity + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MedianMs1IsolationInterference + ",");
                    f.Write(qcData.QcData[key].RawMetrics.MedianMs2FractionConsumingTop80PercentTotalIntensity + ",");

                    f.Write(qcData.QcData[key].RawMetrics.NumberOfEsiFlags + ",");
                    f.Write(qcData.QcData[key].SearchMetrics.MedianMassDrift + ",");

                    //f.Write(qcData.QcData[key].QuantMeta.medianReporterIntensity + ",");

                    f.Write(qcData.QcData[key].SearchMetrics.IdentificationRate + ",");
                    f.Write(qcData.QcData[key].SearchMetrics.DigestionEfficiency + ",");
                    f.Write(qcData.QcData[key].SearchMetrics.MissedCleavageRate + ",");

                    f.Write(qcData.QcData[key].SearchMetrics.LabelingEfficiencyAtNTerm + ",");
                    f.Write(qcData.QcData[key].SearchMetrics.LabelingEfficiencyAtK + ",");
                    f.Write(qcData.QcData[key].SearchMetrics.LabelingEfficiencyAtX + ",");

                    f.Write(qcData.QcData[key].RawMetrics.Ms1FillTimeDistribution.P50 + ",");
                    f.Write(qcData.QcData[key].RawMetrics.Ms2FillTimeDistribution.P50 + ",");
                    f.Write(qcData.QcData[key].RawMetrics?.Ms3FillTimeDistribution?.P50 + ",");

                    f.Write(qcData.QcData[key].RawMetrics.PeakShape.Width.P10 * 60 + ",");
                    f.Write(qcData.QcData[key].RawMetrics.PeakShape.Width.P50 * 60 + ",");
                    f.Write(qcData.QcData[key].RawMetrics.PeakShape.Asymmetry.P10 + ",");
                    f.Write(qcData.QcData[key].RawMetrics.PeakShape.Asymmetry.P50 + ",");
                    f.Write(qcData.QcData[key].RawMetrics.PeakCapacity + ",");
                    f.Write(qcData.QcData[key].RawMetrics.TimeBeforeFirstScanToExceedPoint1MaxIntensity + ",");
                    f.Write(qcData.QcData[key].RawMetrics.TimeAfterLastScanToExceedPoint1MaxIntensity + ",");
                    f.Write(qcData.QcData[key].RawMetrics.FractionOfRunAbovePoint1MaxIntensity + ",");

                    f.Write(qcData.QcData[key].SearchMetrics.ChargeRatio3to2 + ",");
                    f.Write(qcData.QcData[key].SearchMetrics.ChargeRatio4to2 + ",");

                    f.Write(qcData.QcData[key].SearchMetrics.SearchParameters + "\n");
                }
            }
            Console.WriteLine("Finished writing QC data to csv\n");
        }

        public static void WriteQcToTable(this QcDataCollectionDIA qcData)
        {
            if (File.Exists(Path.Combine(qcData.QcDirectory, "QcDataTable.csv")))
            {
                while (Utilities.ReadWrite.IsFileLocked(Path.Combine(qcData.QcDirectory, "QcDataTable.csv")))
                {
                    Console.WriteLine("{0} is in use. Please close the file and press any key to continue.", Path.Combine(qcData.QcDirectory, "QcDataTable.csv"));
                    Console.ReadKey();
                }
            }

            using (StreamWriter f = new StreamWriter(Path.Combine(qcData.QcDirectory, "QcDataTable.csv"), append: false))
            {
                f.WriteLine("DateAcquired,FileName,Instrument,ExperimentMsOrder,Ms1Analyzer,Ms2Analyzer,TotalAnalysisTime,TotalScans,NumMs1Scans,NumMs2Scans," +
                    "Ms1ScanRate(/s),Ms2ScanRate(/s),MeanDutyCycle(s),MeanMs2TriggerRate(/Ms1Scan),Ms1MedianSummedIntensity,Ms2MedianSummedIntensity," +
                    "MedianMs2PeakFractionConsumingTop80PercentTotalIntensity,EsiInstabilityFlags(count)," +
                    "MedianMsFillTime(ms),MedianMs2FillTime(ms),MedianMs3FillTime(ms)," +
                    "TimeBeforeFirstExceedanceOf10%MaxIntensity,TimeAfterLastExceedanceOf10%MaxIntensity,FractionOfRunAbove10%MaxIntensity");
            }

            List<DateTime> keys = qcData.QcData.Keys.ToList();
            keys.Sort();

            using (StreamWriter f = new StreamWriter(Path.Combine(qcData.QcDirectory, "QcDataTable.csv"), append: true))
            {
                foreach (DateTime key in keys)
                {

                    f.Write(qcData.QcData[key].MSOrder + ",");
                    f.Write(qcData.QcData[key].MS1Analyzer + ",");
                    f.Write(qcData.QcData[key].MS2Analyzer + ",");

                    f.Write(qcData.QcData[key].Gradient + ",");
                    f.Write(qcData.QcData[key].TotalScans + ",");
                    f.Write(qcData.QcData[key].MS1Scans + ",");
                    f.Write(qcData.QcData[key].MS2Scans + ",");

                    f.Write(qcData.QcData[key].MS1ScanRate / 60 + ",");
                    f.Write(qcData.QcData[key].MS2ScanRate / 60 + ",");
                    f.Write(qcData.QcData[key].MeanDutyCycle + ",");

                    f.Write(qcData.QcData[key].MedianSummedMS1Intensity + ",");
                    f.Write(qcData.QcData[key].MedianSummedMS2Intensity + ",");
                    f.Write(qcData.QcData[key].MedianMs2FractionConsumingTop80PercentTotalIntensity + ",");

                    f.Write(qcData.QcData[key].NumberOfEsiFlags + ",");
                    
                    f.Write(qcData.QcData[key].Ms1FillTimeDistribution.P50 + ",");
                    f.Write(qcData.QcData[key].Ms2FillTimeDistribution.P50 + ",");
                    
                    f.Write(qcData.QcData[key].TimeBeforeFirstScanToExceedPoint1MaxIntensity + ",");
                    f.Write(qcData.QcData[key].TimeAfterLastScanToExceedPoint1MaxIntensity + ",");
                    f.Write(qcData.QcData[key].FractionOfRunAbovePoint1MaxIntensity + ",");
                }
            }
            Console.WriteLine("Finished writing QC data to csv\n");
        }
    }

    static class ChromatogramWriter
    {
        public static void WriteChromatogram(CentroidStreamCollection centroids, SegmentScanCollection segments, RetentionTimeCollection retentionTimes, MethodDataContainer methodData, ScanIndex index, WorkflowParameters parameters, string rawFileName)
        {
            string chro = parameters.ParseParams.Chromatogram;

            MSOrderType order = (MSOrderType)Convert.ToInt32((chro.ElementAt(0).ToString()));

            MassAnalyzerType analyzer = methodData.MassAnalyzers[order];

            if ((int)order > (int)methodData.AnalysisOrder)
            {
                Log.Error("Specified MS order ({Order}) for chromatogram is higher than experiment order ({ExpOrder})",
                    order, methodData.AnalysisOrder);
                Console.WriteLine("Specified MS order ({0}) for chromatogram is higher than experiment order ({1}). Chromatogram(s) won't be written.",
                    order, methodData.AnalysisOrder);
            }

            bool TIC = chro.Contains("T");
            bool BP = chro.Contains("B");

            int[] scans = index.ScanEnumerators[order];

            if (TIC)
            {
                ProgressIndicator progress = new ProgressIndicator(scans.Length, String.Format("Writing {0} TIC chromatogram", order));
                progress.Start();
                string fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, "_" + order + "_TIC_chromatogram.txt");

                using (StreamWriter f = new StreamWriter(fileName))
                {
                    f.WriteLine("RetentionTime\tIntensity");

                    if (analyzer == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        foreach (int scan in scans)
                        {
                            if (centroids[scan].Intensities.Length > 0)
                            {
                                f.WriteLine("{0}\t{1}", retentionTimes[scan], centroids[scan].Intensities.Sum());
                            }
                            else
                            {
                                f.WriteLine("{0}\t{1}", retentionTimes[scan], 0);
                            }
                            progress.Update();
                        }
                    }
                    else
                    {
                        foreach (int scan in scans)
                        {
                            if (segments[scan].Intensities.Length > 0)
                            {
                                f.WriteLine("{0}\t{1}", retentionTimes[scan], segments[scan].Intensities.Sum());
                            }
                            else
                            {
                                f.WriteLine("{0}\t{1}", retentionTimes[scan], 0);
                            }
                            progress.Update();
                        }
                    }
                }
                progress.Done();
            }
            if (BP)
            {
                ProgressIndicator progress = new ProgressIndicator(scans.Length, String.Format("Writing {0} base peak chromatogram", order));
                progress.Start();

                string fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, "_" + order + "_BP_chromatogram.txt");

                using (StreamWriter f = new StreamWriter(fileName))
                {
                    f.WriteLine("RetentionTime\tIntensity");

                    if (analyzer == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        foreach (int scan in scans)
                        {
                            if (centroids[scan].Intensities.Length > 0)
                            {
                                f.WriteLine("{0}\t{1}", retentionTimes[scan], centroids[scan].Intensities.Max());
                            }
                            else
                            {
                                f.WriteLine("{0}\t{1}", retentionTimes[scan], 0);
                            }
                            progress.Update();
                        }
                    }
                    else
                    {
                        foreach (int scan in scans)
                        {
                            if (segments[scan].Intensities.Length > 0)
                            {
                                f.WriteLine("{0}\t{1}", retentionTimes[scan], segments[scan].Intensities.Max());
                            }
                            else
                            {
                                f.WriteLine("{0}\t{1}", retentionTimes[scan], 0);
                            }
                            progress.Update();
                        }
                    }
                }
                progress.Done();
            }
        }
    }
}
