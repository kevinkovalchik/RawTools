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
using RawTools.Algorithms;
using ThermoFisher;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;
using RawTools.WorkFlows;
using Serilog;

namespace RawTools.Data.IO
{
    static class MatrixWriter
    {
        public static void ParseQuantDDA(string fileName, CentroidStreamCollection centroids, SegmentScanCollection segments,
            ScanMetaDataCollectionDDA metaData, RetentionTimeCollection retentionTimes, PrecursorMassCollection precursorMasses,
            PrecursorScanCollection precursorScans, PrecursorPeakCollection precursorPeaks, TrailerExtraCollection trailerExtras,
            ScanIndex Index, QuantDataCollection quantData = null)
        {
            ReadWrite.CheckFileAccessibility(fileName);

            using (StreamWriter f = new StreamWriter(fileName))
            {
                var scans = Index.ScanEnumerators[Index.AnalysisOrder];
                int masterScan, ms2scan, ms3scan;

                // first we write all the columns headers

                if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write("MS3ScanNumber\t");

                f.Write("MS2ScanNumber\tMS1ScanNumber\t");

                if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write("MS3RetTime(min)\t");

                f.Write("MS2RetTime(min)\tMS1RetTime(min)\tDutyCycle(s)\t" +
                "Ms2TriggerRate(/Ms1Scan)\tParentIonMass\tMonoisotopicMass\tPrecursorCharge\tMS1IsolationInterference\t" +
                "ParentPeakFound\tParentPeakArea\tPeakFirstScan\tPeakMaxScan\tPeakLastScan\tBaseLinePeakWidth(s)\t" +
                "PeakParentScanIntensity\tPeakMaxIntensity\t");

                if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write("Ms3FillTime\t");

                f.Write("Ms2FillTime\tMs1FillTime\tHCDEnergy\tFaimsVoltage\tMs2MedianIntensity\tMs1MedianIntensity\t");

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
                    foreach (string label in new LabelingReagents().Reagents[reagents].Labels)
                    {
                        f.Write(label + "SignalToNoise\t");
                    }
                    foreach (string label in new LabelingReagents().Reagents[reagents].Labels)
                    {
                        f.Write(label + "ppmMassError\t");
                    }
                }
                f.Write("\n");

                // now write all the data

                foreach (int scan in scans)
                {
                    masterScan = precursorScans[scan].MasterScan;
                    ms2scan = precursorScans[scan].MS2Scan;
                    ms3scan = precursorScans[scan].MS3Scan;

                    if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write($"{ms3scan}\t");

                    f.Write($"{ms2scan}\t{masterScan}\t");

                    if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write($"{retentionTimes[ms3scan]}\t");

                    f.Write($"{retentionTimes[ms2scan]}\t{retentionTimes[masterScan]}\t{metaData.DutyCycle[masterScan]}\t" +
                    $"{metaData.MS2ScansPerCycle[masterScan]}\t{precursorMasses[ms2scan].ParentMZ}\t{precursorMasses[ms2scan].MonoisotopicMZ}\t" +
                    $"{trailerExtras[ms2scan].ChargeState}\t{metaData.Ms1IsolationInterference[scan]}\t" +
                    $"{precursorPeaks[ms2scan].PeakFound}\t{precursorPeaks[ms2scan].Area}\t{precursorPeaks[ms2scan].FirstScan}\t" +
                    $"{precursorPeaks[ms2scan].MaxScan}\t{precursorPeaks[ms2scan].LastScan}\t{precursorPeaks[ms2scan].BaselineWidth}\t" +
                    $"{precursorPeaks[ms2scan].ParentIntensity}\t{precursorPeaks[ms2scan].MaximumIntensity}\t");

                    if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write($"{metaData.FillTime[ms3scan]}\t");

                    f.Write($"{metaData.FillTime[ms2scan]}\t{metaData.FillTime[masterScan]}\t");

                    f.Write($"{trailerExtras[scan].HCDEnergy}\t{trailerExtras[scan].FaimsVoltage}\t{metaData.IntensityDistribution[ms2scan].P50}\t{metaData.IntensityDistribution[masterScan].P50}\t");

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
                        foreach (string label in quantData[scan].Keys)
                        {
                            f.Write(quantData[scan][label].SignalToNoise + "\t");
                        }
                        foreach (string label in quantData[scan].Keys)
                        {
                            f.Write(quantData[scan][label].ppmMassError + "\t");
                        }
                    }
                    f.Write("\n");
                }
            }
        }

        public static void ParseQuantDIA(string fileName, CentroidStreamCollection centroids, SegmentScanCollection segments,
            ScanMetaDataCollectionDDA metaData, RetentionTimeCollection retentionTimes, PrecursorMassCollection precursorMasses,
            PrecursorScanCollection precursorScans, PrecursorPeakCollection precursorPeaks, TrailerExtraCollection trailerExtras,
            ScanIndex Index, QuantDataCollection quantData = null)
        {
            ReadWrite.CheckFileAccessibility(fileName);

            // NOTE: This is a placeholder. Once we know what needs to go into the DIA files, it can be modified

            using (StreamWriter f = new StreamWriter(fileName))
            {
                var scans = Index.ScanEnumerators[Index.AnalysisOrder];
                int masterScan, ms2scan, ms3scan;

                // first we write all the columns headers

                if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write("MS3ScanNumber\t");

                f.Write("MS2ScanNumber\tMS1ScanNumber\t");

                if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write("MS3RetTime\t");

                f.Write("MS2RetTime\tMS1RetTime\tDutyCycle\t" +
                "MS2ScansPerCycle\tParentIonMass\tMonoisotopicMass\tPrecursorCharge\tMS1IsolationInterference\t" +
                "ParentPeakFound\tParentPeakArea\tPeakFirstScan\tPeakMaxScan\tPeakLastScan\tBaseLinePeakWidth(s)\t" +
                "PeakParentScanIntensity\tPeakMaxIntensity\t");

                if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write("MS3IonInjectionTime\t");

                f.Write("MS1IonInjectionTime\tMS2IonInjectionTime\tHCDEnergy\tMS2MedianIntensity\tMS1MedianIntensity\t");

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
                    foreach (string label in new LabelingReagents().Reagents[reagents].Labels)
                    {
                        f.Write(label + "SignalToNoise\t");
                    }
                    foreach (string label in new LabelingReagents().Reagents[reagents].Labels)
                    {
                        f.Write(label + "ppmMassError\t");
                    }
                }
                f.Write("\n");

                // now write all the data

                foreach (int scan in scans)
                {
                    masterScan = precursorScans[scan].MasterScan;
                    ms2scan = precursorScans[scan].MS2Scan;
                    ms3scan = precursorScans[scan].MS3Scan;

                    if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write($"{ms3scan}\t");

                    f.Write($"{ms2scan}\t{masterScan}\t");

                    if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write($"{retentionTimes[ms3scan]}\t");

                    f.Write($"{retentionTimes[ms2scan]}\t{retentionTimes[masterScan]}\t{metaData.DutyCycle[masterScan]}\t" +
                    $"{metaData.MS2ScansPerCycle[masterScan]}\t{precursorMasses[ms2scan].ParentMZ}\t{precursorMasses[ms2scan].MonoisotopicMZ}\t" +
                    $"{trailerExtras[ms2scan].ChargeState}\t{metaData.Ms1IsolationInterference[scan]}\t" +
                    $"{precursorPeaks[ms2scan].PeakFound}\t{precursorPeaks[ms2scan].Area}\t{precursorPeaks[ms2scan].FirstScan}\t" +
                    $"{precursorPeaks[ms2scan].MaxScan}\t{precursorPeaks[ms2scan].LastScan}\t{precursorPeaks[ms2scan].BaselineWidth}\t" +
                    $"{precursorPeaks[ms2scan].ParentIntensity}\t{precursorPeaks[ms2scan].MaximumIntensity}\t");

                    if (Index.AnalysisOrder == MSOrderType.Ms3) f.Write($"{metaData.FillTime[ms3scan]}\t");

                    f.Write($"{metaData.FillTime[ms2scan]}\t{metaData.FillTime[masterScan]}\t");

                    f.Write($"{trailerExtras[scan].HCDEnergy}\t{metaData.IntensityDistribution[ms2scan].P50}\t{metaData.IntensityDistribution[masterScan].P50}\t");

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
                        foreach (string label in quantData[scan].Keys)
                        {
                            f.Write(quantData[scan][label].SignalToNoise + "\t");
                        }
                        foreach (string label in quantData[scan].Keys)
                        {
                            f.Write(quantData[scan][label].ppmMassError + "\t");
                        }
                    }
                    f.Write("\n");
                }
            }

            var data = new List<string>
                {
                "MS3ScanNumber\tMS2ScanNumber\tMS1ScanNumber\tQuantScanRetTime\tParentScanRetTime\tDutyCycle\t",
                "MS2ScansPerCycle\tParentIonMass\tMonoisotopicMass\tPrecursorCharge\tMS1IsolationInterference\t",
                "ParentPeakFound\tParentPeakArea\tPeakFirstScan\tPeakMaxScan\tPeakLastScan\tBaseLinePeakWidth(s)\t",
                "PeakParentScanIntensity\tPeakMaxIntensity\tMS1IonInjectionTime\tMS2IonInjectionTime\t",
                "MS3IonInjectionTime\tHCDEnergy\tMS1MedianIntensity\tMS2MedianIntensity\t"
                };
        }
    }

    static class MgfWriter
    {
        public static void WriteMGF(string rawFileName, CentroidStreamCollection centroidStreams, SegmentScanCollection segmentScans, WorkflowParameters parameters, RetentionTimeCollection retentionTimes,
            PrecursorMassCollection precursorMasses, PrecursorScanCollection precursorScans, TrailerExtraCollection trailerExtras, MethodDataContainer methodData,
            ScanIndex Index, int[] scans = null, string outputFile = null)
        {
            string fileName;

            double intCutoff = 0;

            List<double> faimsVoltages = new List<double>();

            if (outputFile == null)
            {
                fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, ".mgf");
            }
            else
            {
                fileName = outputFile;
            }

            ReadWrite.CheckFileAccessibility(fileName);

            MassAnalyzerType ms2MassAnalyzer = methodData.MassAnalyzers[MSOrderType.Ms2];

            const int BufferSize = 65536;  // 64 Kilobytes

            using (StreamWriter f = new StreamWriter(fileName, false, Encoding.UTF8, BufferSize)) //Open a new file, the MGF file
            {
                // if the scans argument is null, use all scans
                if (scans == null)
                {
                    scans = Index.ScanEnumerators[MSOrderType.Ms2];
                }

                ProgressIndicator progress = new ProgressIndicator(scans.Count(), String.Format("Writing MGF file"));

                // we need to add a blank line at the begining of the file so MS-GF+ works, no idea why...
                f.WriteLine();

                foreach (int i in scans)
                {
                    f.WriteLine("BEGIN IONS");
                    f.WriteLine("TITLE=Spectrum_{0}", i);
                    f.WriteLine("PEPMASS={0}", precursorMasses[i].MonoisotopicMZ);
                    f.WriteLine("CHARGE={0}+", trailerExtras[i].ChargeState);
                    f.WriteLine("RTINSECONDS={0}", retentionTimes[i] * 60);
                    f.WriteLine("SCANS={0}", i);
                    f.WriteLine("RAWFILE={0}", rawFileName);

                    if (trailerExtras[i].FaimsVoltage != -1)
                    {
                        faimsVoltages.Add(trailerExtras[i].FaimsVoltage);
                    }

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

                    f.WriteLine("END IONS\n");

                    progress.Update();
                }
                progress.Done();
                
            }

            var DistinctFaimsVoltages = faimsVoltages.Distinct();
            foreach (var item in DistinctFaimsVoltages)
            {
                Console.WriteLine("The faims values are " + item);
                //fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, "_CV" + item + ".mgf");
                //ReadWrite.CheckFileAccessibility(fileName);
                //using (StreamWriter f = new StreamWriter(fileName, false, Encoding.UTF8, BufferSize)) //Open a new file, the MGF file
                //{

                //}
            }
        }
    }

    static class FaimsMgfWriter
    {
        public static void WriteFaimsMGF(string rawFileName, CentroidStreamCollection centroidStreams, SegmentScanCollection segmentScans, WorkflowParameters parameters, RetentionTimeCollection retentionTimes,
            PrecursorMassCollection precursorMasses, PrecursorScanCollection precursorScans, TrailerExtraCollection trailerExtras, MethodDataContainer methodData,
            ScanIndex Index, int[] scans = null, string outputFile = null)
        {
            string fileName;
            List<double> faimsVoltageSet = new List<double>();
            double intCutoff = 0;

            //pull the ms2 scans out of the file
            if (scans == null)
            {
                scans = Index.ScanEnumerators[MSOrderType.Ms2];
            }

            //get the faims voltages across all scans
            foreach (int i in scans)
            {
                if (trailerExtras[i].FaimsVoltage != -1)
                {
                    faimsVoltageSet.Add(trailerExtras[i].FaimsVoltage);
                }
            }

            //get the distinct faims voltage values
            var DistinctFaimsVoltages = faimsVoltageSet.Distinct().ToArray();
            foreach (var item in DistinctFaimsVoltages)
            {
                if (outputFile == null)
                {
                    fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, "_CV" + item.ToString().Substring(1) + ".mgf");
                }
                else
                {
                    fileName = outputFile;
                }

                ReadWrite.CheckFileAccessibility(fileName);

                MassAnalyzerType ms2MassAnalyzer = methodData.MassAnalyzers[MSOrderType.Ms2];

                const int BufferSize = 65536;  // 64 Kilobytes

                using (StreamWriter f = new StreamWriter(fileName, false, Encoding.UTF8, BufferSize)) //Open a new file, the MGF file
                {
                    // if the scans argument is null, use all scans
                    if (scans == null)
                    {
                        scans = Index.ScanEnumerators[MSOrderType.Ms2];
                    }

                    ProgressIndicator progress = new ProgressIndicator(scans.Count(), String.Format("Writing MGF file for CV " + item));

                    // we need to add a blank line at the begining of the file so MS-GF+ works, no idea why...
                    f.WriteLine();

                    foreach (int i in scans)
                    {
                        if (trailerExtras[i].FaimsVoltage == item)
                        {
                            f.WriteLine("BEGIN IONS");
                            f.WriteLine("TITLE=Spectrum_{0}", i);
                            f.WriteLine("PEPMASS={0}", precursorMasses[i].MonoisotopicMZ);
                            f.WriteLine("CHARGE={0}+", trailerExtras[i].ChargeState);
                            f.WriteLine("RTINSECONDS={0}", retentionTimes[i] * 60);
                            f.WriteLine("SCANS={0}", i);
                            f.WriteLine("RAWFILE={0}", rawFileName);

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

                            f.WriteLine("END IONS\n");

                            progress.Update();
                        }
                        
                    }

                    progress.Done();

                }
            }
        }
    }


    static class MgfLevelsWriter
    {
        public static void WriteLevelsMgf(string rawFileName, CentroidStreamCollection centroidStreams, SegmentScanCollection segmentScans, WorkflowParameters parameters, RetentionTimeCollection retentionTimes,
            PrecursorMassCollection precursorMasses, PrecursorScanCollection precursorScans, TrailerExtraCollection trailerExtras, MethodDataContainer methodData,
            ScanIndex Index, int[] scans = null, string outputFile = null)
        {
            string fileName;
            string mgfLevels = parameters.ParseParams.WriteMgfLevels;
            int levelsInt;
            MSOrderType order;

            foreach (var o in mgfLevels)
            {
                if (!Int32.TryParse(o.ToString(), out levelsInt)) continue;
                else order = (MSOrderType)levelsInt;

                if ((int)order > (int)methodData.AnalysisOrder)
                {
                    Log.Error("Specified MS order ({Order}) for mgf is higher than experiment order ({ExpOrder})",
                        order, methodData.AnalysisOrder);
                    Console.WriteLine("Specified MS order ({0}) for mgf is higher than experiment order ({1}). MGF won't be written.",
                        order, methodData.AnalysisOrder);
                }


                double intCutoff = 0;
                if (outputFile == null)
                {
                    fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, "_" + order + ".mgf");
                }
                else
                {
                    fileName = outputFile;
                }

                ReadWrite.CheckFileAccessibility(fileName);

                MassAnalyzerType analyzer = methodData.MassAnalyzers[order];
                Console.WriteLine("Mass analyzer for " + order + " is " + analyzer);
                
                const int BufferSize = 65536;  // 64 Kilobytes

                using (StreamWriter f = new StreamWriter(fileName, false, Encoding.UTF8, BufferSize)) //Open a new file, the MGF file
                {
                    // if the scans argument is null, use all scans

                    if ((levelsInt == 1) && (scans == null))
                    {
                        MassAnalyzerType msMassAnalyzer = methodData.MassAnalyzers[order];
                        scans = Index.ScanEnumerators[MSOrderType.Ms];

                        ProgressIndicator progress = new ProgressIndicator(scans.Count(), String.Format("Writing " + order + " MGF file"));

                        // we need to add a blank line at the begining of the file so MS-GF+ works, no idea why...
                        f.WriteLine();

                        foreach (int i in scans)
                        {
                            f.WriteLine("BEGIN IONS");
                            f.WriteLine("TITLE=Spectrum_{0}", i);
                            f.WriteLine("SCANS={0}", i);
                            f.WriteLine("RTINSECONDS={0}", retentionTimes[i] * 60);
                            f.WriteLine("RAWFILE={0}", rawFileName);

                            if (msMassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
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

                            f.WriteLine("END IONS\n");

                            progress.Update();
                        }
                        progress.Done();
                        scans = null;
                    }

                    if ((levelsInt == 2) && (scans == null))
                    {
                        MassAnalyzerType msMassAnalyzer = methodData.MassAnalyzers[order];
                        scans = Index.ScanEnumerators[MSOrderType.Ms2];

                        ProgressIndicator progress = new ProgressIndicator(scans.Count(), String.Format("Writing " + order + " MGF file"));

                        // we need to add a blank line at the begining of the file so MS-GF+ works, no idea why...
                        f.WriteLine();

                        foreach (int i in scans)
                        {
                            f.WriteLine("BEGIN IONS");
                            f.WriteLine("TITLE=Spectrum_{0}", i);
                            f.WriteLine("PEPMASS={0}", precursorMasses[i].MonoisotopicMZ);
                            f.WriteLine("CHARGE={0}+", trailerExtras[i].ChargeState);
                            f.WriteLine("RTINSECONDS={0}", retentionTimes[i] * 60);
                            f.WriteLine("SCANS={0}", i);
                            f.WriteLine("RAWFILE={0}", rawFileName);

                            if (msMassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
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

                            f.WriteLine("END IONS\n");

                            progress.Update();
                        }
                        progress.Done();
                        scans = null;
                    }

                    if ((levelsInt == 3) && (scans == null))
                    {
                        MassAnalyzerType msMassAnalyzer = methodData.MassAnalyzers[order];
                        scans = Index.ScanEnumerators[MSOrderType.Ms3];

                        ProgressIndicator progress = new ProgressIndicator(scans.Count(), String.Format("Writing " + order + " MGF file"));

                        // we need to add a blank line at the begining of the file so MS-GF+ works, no idea why...
                        f.WriteLine();

                        foreach (int i in scans)
                        {
                            f.WriteLine("BEGIN IONS");
                            f.WriteLine("TITLE=Spectrum_{0}", i);
                            f.WriteLine("PEPMASS={0}", precursorMasses[i].MonoisotopicMZ);
                            f.WriteLine("RTINSECONDS={0}", retentionTimes[i] * 60);
                            f.WriteLine("SCANS={0}", i);
                            f.WriteLine("RAWFILE={0}", rawFileName);

                            if (msMassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
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

                            f.WriteLine("END IONS\n");

                            progress.Update();
                        }
                        progress.Done();
                        scans = null;
                    }


                }
            }

                      
        }
    }

    static class MetricsWriter
    {
        public static void WriteMatrix(RawMetricsDataDDA metrics, SearchMetricsContainer searchMetrics, string rawFileName, string outputDirectory = null)
        {
            string fileName = ReadWrite.GetPathToFile(outputDirectory, rawFileName, "_Metrics.txt");

            if (File.Exists(fileName))
            {
                while (Utilities.ReadWrite.IsFileLocked(fileName))
                {
                    Console.WriteLine("{0} is in use. Please close the file and press any key to continue.", fileName);
                    Console.ReadKey();
                }
            }

            using (StreamWriter f = new StreamWriter(fileName)) //Open a new file
            {
                f.WriteLine("RawFile:\t" + metrics.RawFileName);
                f.WriteLine("Instrument:\t" + metrics.Instrument);
                f.WriteLine("ExperimentMsOrder:\t" + metrics.MSOrder);
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
                f.WriteLine("NumMs1Scans:\t" + metrics.MS1Scans);
                f.WriteLine("NumMs2Scans:\t" + metrics.MS2Scans);
                f.WriteLine("NumMs3Scans:\t" + metrics.MS3Scans);
                f.WriteLine("MeanMs2TriggerRate(/Ms1Scan):\t" + Math.Round(metrics.MeanTopN, 4));
                f.WriteLine("Ms1ScanRate(/sec):\t" + Math.Round(metrics.MS1ScanRate / 60, 4));
                f.WriteLine("Ms2ScanRate(/sec):\t" + Math.Round(metrics.MS2ScanRate / 60, 4));
                f.WriteLine("MeanDutyCycle(s):\t" + Math.Round(metrics.MeanDutyCycle, 4));
                f.WriteLine("MedianMs1FillTime(ms):\t" + Math.Round(metrics.MedianMS1FillTime, 4));
                f.WriteLine("MedianMs2FillTime(ms):\t" + Math.Round(metrics.MedianMS2FillTime, 4));
                f.WriteLine("MedianMs3FillTime(ms):\t" + Math.Round(metrics.MedianMS3FillTime, 4));
                foreach (var item in metrics.FaimsVoltages)
                {
                    f.WriteLine("FaimsVoltage(CV):\t" + item);
                }                    
                f.WriteLine("Ms2MedianSummedIntensity:\t" + Math.Round(metrics.MedianSummedMS2Intensity, 4));
                f.WriteLine("MedianMS1IsolationInterference:\t" + Math.Round(metrics.MedianMs1IsolationInterference, 4));
                f.WriteLine("MedianPeakWidthAt10%H(s):\t" + Math.Round(metrics.MedianBaselinePeakWidth * 60, 4));
                f.WriteLine("MedianPeakWidthAt50%H(s):\t" + Math.Round(metrics.MedianHalfHeightPeakWidth * 60, 4));
                f.WriteLine("MedianAsymmetryFactor:\t" + Math.Round(metrics.MedianAsymmetryFactor, 4));
                f.WriteLine("PeakCapacity:\t" + Math.Round(metrics.PeakCapacity, 4));
                f.WriteLine("NumEsiInstabilityFlags:\t" + metrics.NumberOfEsiFlags);

                if (searchMetrics != null)
                {
                    f.Write(searchMetrics.MedianMassDrift + ",");

                    //f.Write(qcData.QcData[key].QuantMeta.medianReporterIntensity + ",");

                    f.WriteLine("IdentificationRate(IDs/Ms2Scan):\t" + Math.Round(searchMetrics.IdentificationRate, 4));
                    f.WriteLine("DigestionEfficiency\t" + Math.Round(searchMetrics.DigestionEfficiency, 4));
                    f.WriteLine("MissedCleavageRate(/PSM)\t" + Math.Round(searchMetrics.MissedCleavageRate, 4));

                    foreach (var mod in searchMetrics.ModificationFrequency)
                    {
                        f.WriteLine("{0}_ModificationFrequency: {1}", mod.Key, mod.Value);
                    }

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
        public static void WriteQcToTable(this QcDataCollection qcData)
        {
            string fileName = Path.Combine(qcData.QcDirectory, "QcDataTable.csv");
            ReadWrite.CheckFileAccessibility(fileName);

            HashSet<string> variableMods = new HashSet<string>();

            foreach (var item in qcData.QcData.Values)
            {
                foreach (var mod in item.SearchMetrics.ModificationFrequency.Keys.ToList()) variableMods.Add(mod);
            }

            List<string> vMods = variableMods.ToList();

            if (qcData.DataType == ExperimentType.DDA)
            {
                using (StreamWriter f = new StreamWriter(fileName, append: false))
                {
                    f.Write("DateAcquired,RawFile,Instrument,ExperimentMsOrder,Ms1Analyzer,Ms2Analyzer,Ms3Analyzer,TotalAnalysisTime(min),TotalScans,NumMs1Scans,NumMs2Scans," +
                        "NumMs3Scans,Ms1ScanRate(/s),Ms2ScanRate(/s),Ms3ScanRate(/s),MeanDutyCycle(s),MeanMs2TriggerRate(/Ms1Scan),Ms1MedianSummedIntensity,Ms2MedianSummedIntensity," +
                        "MedianPrecursorIntensity,MedianMs1IsolationInterference,MedianMs2PeakFractionConsumingTop80PercentTotalIntensity,NumEsiInstabilityFlags,MedianMassDrift(ppm)," +
                        "IdentificationRate(IDs/Ms2Scan),DigestionEfficiency,MissedCleavageRate(/PSM),");

                    foreach (var mod in vMods) f.Write("{0}_ModificationFrequency,", mod);

                    f.Write("MedianPeptideScore,CutoffDecoyScore(0.05FDR),NumberOfPSMs,NumberOfUniquePeptides,MedianMs1FillTime(ms),MedianMs2FillTime(ms),MedianMs3FillTime(ms),MedianPeakWidthAt10%H(s),MedianPeakWidthAt50%H(s),MedianAsymmetryAt10%H,MedianAsymmetryAt50%H,MeanCyclesPerAveragePeak," +
                        "PeakCapacity,TimeBeforeFirstExceedanceOf10%MaxIntensity,TimeAfterLastExceedanceOf10%MaxIntensity,FractionOfRunAbove10%MaxIntensity,PsmChargeRatio3to2,PsmChargeRatio4to2,SearchParameters\n");
                }

                List<DateTime> keys = qcData.QcData.Keys.ToList();
                keys.Sort();

                using (StreamWriter f = new StreamWriter(Path.Combine(qcData.QcDirectory, "QcDataTable.csv"), append: true))
                {
                    foreach (DateTime key in keys)
                    {
                        RawMetricsDataDDA rawMetrics = qcData.QcData[key].DDA;
                        SearchMetricsContainer searchMetrics = qcData.QcData[key].SearchMetrics;
                        f.Write(rawMetrics.DateAcquired + ",");
                        f.Write(rawMetrics.RawFileName + ",");
                        f.Write(rawMetrics.Instrument + ",");

                        f.Write(rawMetrics.MSOrder + ",");
                        f.Write(rawMetrics.MS1Analyzer + ",");
                        f.Write(rawMetrics.MS2Analyzer + ",");
                        f.Write(rawMetrics.MS3Analyzer + ",");

                        f.Write(rawMetrics.TotalAnalysisTime + ",");
                        f.Write(rawMetrics.TotalScans + ",");
                        f.Write(rawMetrics.MS1Scans + ",");
                        f.Write(rawMetrics.MS2Scans + ",");
                        f.Write(rawMetrics.MS3Scans + ",");

                        f.Write(rawMetrics.MS1ScanRate / 60 + ",");
                        f.Write(rawMetrics.MS2ScanRate / 60 + ",");
                        f.Write(rawMetrics.MS3ScanRate / 60 + ",");
                        f.Write(rawMetrics.MeanDutyCycle + ",");
                        f.Write(rawMetrics.MeanTopN + ",");

                        f.Write(rawMetrics.MedianSummedMS1Intensity + ",");
                        f.Write(rawMetrics.MedianSummedMS2Intensity + ",");
                        f.Write(rawMetrics.MedianPrecursorIntensity + ",");
                        f.Write(rawMetrics.MedianMs1IsolationInterference + ",");
                        f.Write(rawMetrics.MedianMs2FractionConsumingTop80PercentTotalIntensity + ",");

                        f.Write(rawMetrics.NumberOfEsiFlags + ",");
                        f.Write(searchMetrics.MedianMassDrift + ",");

                        //f.Write(qcData.QcData[key].QuantMeta.medianReporterIntensity + ",");

                        f.Write(searchMetrics.IdentificationRate + ",");
                        f.Write(searchMetrics.DigestionEfficiency + ",");
                        f.Write(searchMetrics.MissedCleavageRate + ",");

                        foreach (var frequency in vMods) f.Write(searchMetrics.ModificationFrequency.TryGetElseDefault(frequency) + ",");

                        f.Write(searchMetrics.MedianPeptideScore + ",");
                        f.Write(searchMetrics.CutoffDecoyScore + ",");
                        f.Write(searchMetrics.NumPSMs + ",");
                        f.Write(searchMetrics.NumUniquePeptides + ",");
                        
                        f.Write(rawMetrics.Ms1FillTimeDistribution.P50 + ",");
                        f.Write(rawMetrics.Ms2FillTimeDistribution.P50 + ",");
                        f.Write(rawMetrics?.Ms3FillTimeDistribution?.P50 + ",");

                        f.Write(rawMetrics.PeakShape.Width.P10 * 60 + ",");
                        f.Write(rawMetrics.PeakShape.Width.P50 * 60 + ",");
                        f.Write(rawMetrics.PeakShape.Asymmetry.P10 + ",");
                        f.Write(rawMetrics.PeakShape.Asymmetry.P50 + ",");
                        f.Write(rawMetrics.PeakShape.Width.P10 * 60 / rawMetrics.MeanDutyCycle + ",");
                        f.Write(rawMetrics.PeakCapacity + ",");
                        f.Write(rawMetrics.TimeBeforeFirstScanToExceedPoint1MaxIntensity + ",");
                        f.Write(rawMetrics.TimeAfterLastScanToExceedPoint1MaxIntensity + ",");
                        f.Write(rawMetrics.FractionOfRunAbovePoint1MaxIntensity + ",");

                        f.Write(searchMetrics.ChargeRatio3to2 + ",");
                        f.Write(searchMetrics.ChargeRatio4to2 + ",");

                        f.Write(searchMetrics.SearchParameters + "\n");
                    }
                }
            }
            else if (qcData.DataType == ExperimentType.DIA)
            {
                using (StreamWriter f = new StreamWriter(Path.Combine(qcData.QcDirectory, "QcDataTable.csv"), append: false))
                {
                    f.WriteLine("DateAcquired,FileName,Instrument,ExperimentMsOrder,Ms1Analyzer,Ms2Analyzer,TotalAnalysisTime,TotalScans,NumMs1Scans,NumMs2Scans," +
                        "Ms1ScanRate(/s),Ms2ScanRate(/s),MeanDutyCycle(s),Ms1MedianSummedIntensity,Ms2MedianSummedIntensity," +
                        "MedianMs2PeakFractionConsumingTop80PercentTotalIntensity,EsiInstabilityFlags(count)," +
                        "MedianMsFillTime(ms),MedianMs2FillTime(ms)," +
                        "TimeBeforeFirstExceedanceOf10%MaxIntensity,TimeAfterLastExceedanceOf10%MaxIntensity,FractionOfRunAbove10%MaxIntensity");
                }

                List<DateTime> keys = qcData.QcData.Keys.ToList();
                keys.Sort();

                using (StreamWriter f = new StreamWriter(Path.Combine(qcData.QcDirectory, "QcDataTable.csv"), append: true))
                {
                    foreach (DateTime key in keys)
                    {
                        RawMetricsDataDIA rawMetrics = qcData.QcData[key].DIA;

                        f.Write(rawMetrics.DateAcquired + ",");
                        f.Write(rawMetrics.RawFileName + ",");
                        f.Write(rawMetrics.Instrument + ",");

                        f.Write(rawMetrics.MSOrder + ",");
                        f.Write(rawMetrics.MS1Analyzer + ",");
                        f.Write(rawMetrics.MS2Analyzer + ",");

                        f.Write(rawMetrics.TotalAnalysisTime + ",");
                        f.Write(rawMetrics.TotalScans + ",");
                        f.Write(rawMetrics.MS1Scans + ",");
                        f.Write(rawMetrics.MS2Scans + ",");

                        f.Write(rawMetrics.MS1ScanRate / 60 + ",");
                        f.Write(rawMetrics.MS2ScanRate / 60 + ",");
                        f.Write(rawMetrics.MeanDutyCycle + ",");

                        f.Write(rawMetrics.MedianSummedMS1Intensity + ",");
                        f.Write(rawMetrics.MedianSummedMS2Intensity + ",");
                        f.Write(rawMetrics.MedianMs2FractionConsumingTop80PercentTotalIntensity + ",");

                        f.Write(rawMetrics.NumberOfEsiFlags + ",");

                        f.Write(rawMetrics.Ms1FillTimeDistribution.P50 + ",");
                        f.Write(rawMetrics.Ms2FillTimeDistribution.P50 + ",");

                        f.Write(rawMetrics.TimeBeforeFirstScanToExceedPoint1MaxIntensity + ",");
                        f.Write(rawMetrics.TimeAfterLastScanToExceedPoint1MaxIntensity + ",");
                        f.Write(rawMetrics.FractionOfRunAbovePoint1MaxIntensity + "\n");
                    }
                }
            }
            Console.WriteLine("Finished writing QC data to csv\n");
        }
    }


    static class Ms1ChromatogramWriter
    {
        public static void WriteMs1Chromatogram(CentroidStreamCollection centroids, SegmentScanCollection segments, RetentionTimeCollection retentionTimes, MethodDataContainer methodData, ScanIndex index, WorkflowParameters parameters, string rawFileName)
        {
            string chro = parameters.ParseParams.Chromatogram;
            int orderInt;
            MSOrderType order;

            foreach (var o in chro)
            {
                if (!Int32.TryParse(o.ToString(), out orderInt)) continue;
                else order = (MSOrderType)orderInt;

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
                    Console.WriteLine("Writing {0} TIC chromatogram", order);
                    string fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, "_" + order + "_TIC_chromatogram.txt");

                    ReadWrite.CheckFileAccessibility(fileName);

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
                            }
                        }
                    }
                }
                if (BP)
                {
                    Console.WriteLine("Writing {0} base peak chromatogram", order);

                    string fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, "_" + order + "_BP_chromatogram.txt");

                    ReadWrite.CheckFileAccessibility(fileName);

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
                            }
                        }
                    }
                }
            }
        }
    }


    static class ChromatogramWriter
    {
        public static void WriteChromatogram(CentroidStreamCollection centroids, SegmentScanCollection segments, RetentionTimeCollection retentionTimes, MethodDataContainer methodData, ScanIndex index, WorkflowParameters parameters, string rawFileName)
        {
            string chro = parameters.ParseParams.Chromatogram;
            int orderInt;
            MSOrderType order;

            foreach (var o in chro)
            {
                if (!Int32.TryParse(o.ToString(), out orderInt)) continue;
                else order = (MSOrderType)orderInt;

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
                    Console.WriteLine("Writing {0} TIC chromatogram", order);
                    string fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, "_" + order + "_TIC_chromatogram.txt");

                    ReadWrite.CheckFileAccessibility(fileName);

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
                            }
                        }
                    }
                }
                if (BP)
                {
                    Console.WriteLine("Writing {0} base peak chromatogram", order);

                    string fileName = ReadWrite.GetPathToFile(parameters.ParseParams.OutputDirectory, rawFileName, "_" + order + "_BP_chromatogram.txt");

                    ReadWrite.CheckFileAccessibility(fileName);

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
                            }
                        }
                    }
                }
            }            
        }
    }

    static class MatchedMgfWriter
    {
        static public void WriteMGF(WorkflowParameters parameters, List<(int scans1, int scans2)> scans,
            string outputFile, List<Ms1FeatureCollection> features, List<CentroidStreamCollection> centroids, List<SegmentScanCollection> segmentScans,
            List<RetentionTimeCollection> retentionTimes, List<PrecursorMassCollection> precursorMasses,
            List<TrailerExtraCollection> trailerExtras, MethodDataContainer methodData, List<string> rawFileNames)
        {
            string fileName;

            double intCutoff = 0;
            
            fileName = outputFile;

            ReadWrite.CheckFileAccessibility(fileName);

            parameters.MgfMassCutoff = 0;
            parameters.MgfIntensityCutoff = 0;

            MassAnalyzerType ms2MassAnalyzer = methodData.MassAnalyzers[MSOrderType.Ms2];

            const int BufferSize = 65536;  // 64 Kilobytes

            using (StreamWriter f = new StreamWriter(fileName, false, Encoding.UTF8, BufferSize)) //Open a new file, the MGF file
            {
                ProgressIndicator progress = new ProgressIndicator(scans.Count(), String.Format("Writing MGF file"));

                int MatchID = 0;
                foreach (var i in scans)
                {
                    MatchID++;

                    int[] scan = new int[] { i.scans1, i.scans2 };

                    foreach(int file in new int[] { 0, 1 })
                    {
                        f.WriteLine("\nBEGIN IONS");
                        f.WriteLine("RAWFILE={0}", rawFileNames[file]);
                        f.WriteLine("TITLE=Spectrum_{0}_File{1}", scan[file], file+1);
                        f.WriteLine("MATCH_ID={0}", MatchID);
                        f.WriteLine("SCAN={0}", scan[file]);
                        f.WriteLine("RTINSECONDS={0}", retentionTimes[file][scan[file]] * 60);
                        f.WriteLine("PEPMASS={0}", precursorMasses[file][scan[file]].MonoisotopicMZ);
                        f.WriteLine("CHARGE={0}", trailerExtras[file][scan[file]].ChargeState);

                        f.Write("HYPERSCORE=");
                        if (features[file][scan[file]].Identified)
                        {
                            f.WriteLine(features[file][scan[file]].PSM.Hyperscore);
                        }
                        else f.WriteLine();

                        f.Write("EXPECTATIONVALUE=");
                        if (features[file][scan[file]].Identified)
                        {
                            f.WriteLine(features[file][scan[file]].PSM.ExpectationValue);
                        }
                        else f.WriteLine();

                        if (ms2MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                        {
                            CentroidStreamData centroid = centroids[file][scan[file]];

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
                            SegmentedScanData segments = segmentScans[file][scan[file]];

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
                    }
                    progress.Update();
                }
                progress.Done();
            }
        }
    }

    static class SimpleCentroidMgfWriter
    {
        static public void WriteMGF(WorkflowParameters parameters, List<(int scans1, int scans2)> scans,
            string outputFile, List<Ms1FeatureCollection> features, List<CentroidStreamCollection> centroids, List<SegmentScanCollection> segmentScans,
            List<RetentionTimeCollection> retentionTimes, List<PrecursorMassCollection> precursorMasses,
            List<TrailerExtraCollection> trailerExtras, MethodDataContainer methodData, List<string> rawFileNames)
        {
            string fileName;

            double intCutoff = 0;

            fileName = outputFile;

            ReadWrite.CheckFileAccessibility(fileName);

            parameters.MgfMassCutoff = 0;
            parameters.MgfIntensityCutoff = 0;

            MassAnalyzerType ms2MassAnalyzer = methodData.MassAnalyzers[MSOrderType.Ms2];

            const int BufferSize = 65536;  // 64 Kilobytes

            using (StreamWriter f = new StreamWriter(fileName, false, Encoding.UTF8, BufferSize)) //Open a new file, the MGF file
            {
                ProgressIndicator progress = new ProgressIndicator(scans.Count(), String.Format("Writing MGF file"));

                int MatchID = 0;
                foreach (var i in scans)
                {
                    MatchID++;

                    int[] scan = new int[] { i.scans1, i.scans2 };

                    foreach (int file in new int[] { 0, 1 })
                    {
                        f.WriteLine("BEGIN IONS");
                        f.WriteLine("RAWFILE={0}", rawFileNames[file]);
                        f.WriteLine("TITLE=Spectrum_{0}_File{1}", scan[file], file + 1);
                        f.WriteLine("MATCH_ID={0}", MatchID);
                        f.WriteLine("SCANS={0}", scan[file]);
                        f.WriteLine("RTINSECONDS={0}", retentionTimes[file][scan[file]]);
                        f.WriteLine("PEPMASS={0}", precursorMasses[file][scan[file]].MonoisotopicMZ);
                        f.WriteLine("CHARGE={0}", trailerExtras[file][scan[file]].ChargeState);

                        f.Write("HYPERSCORE=");
                        if (features[file][scan[file]].Identified)
                        {
                            f.WriteLine(features[file][scan[file]].PSM.Hyperscore);
                        }
                        else f.WriteLine();

                        f.Write("EXPECTATIONVALUE=");
                        if (features[file][scan[file]].Identified)
                        {
                            f.WriteLine(features[file][scan[file]].PSM.ExpectationValue);
                        }
                        else f.WriteLine();

                        if (ms2MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                        {
                            CentroidStreamData centroid = centroids[file][scan[file]];

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
                            SegmentedScanData segments = segmentScans[file][scan[file]];

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
                        f.WriteLine("END IONS\n");
                    }
                    progress.Update();
                }
                progress.Done();
            }
        }
    }
}
