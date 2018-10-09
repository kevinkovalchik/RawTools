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

namespace RawTools.Data.IO
{
    static class Parse
    {
        public static void WriteMatrix(RawDataCollection rawData, ScanMetaDataCollection metaData, IRawDataPlus rawFile, QuantDataCollection quantData = null, string outputDirectory = null)
        {
            string fileName = ReadWrite.GetPathToFile(outputDirectory, rawData.rawFileName, "_Matrix.txt");

            CheckIfDone.Check(rawData, rawFile, new List<Operations> { Operations.ScanIndex, Operations.MethodData, Operations.PrecursorScans,
                Operations.RetentionTimes, Operations.PrecursorMasses, Operations.TrailerExtras, Operations.MetaData});

            using (StreamWriter f = new StreamWriter(fileName)) //Open a new file
            {
                List<int> scans;

                if (!rawData.isBoxCar)
                {
                    scans = rawData.scanIndex.ScanEnumerators[rawData.scanIndex.AnalysisOrder].ToList();
                }
                else
                {
                    scans = rawData.precursorScans.Keys.ToList();
                    scans.Sort();
                }                

                ProgressIndicator progress = new ProgressIndicator(scans.Count(), "Writing matrix to disk");

                f.Write("MS3ScanNumber\tMS2ScanNumber\tMS1ScanNumber\tQuantScanRetTime\tParentScanRetTime\tDutyCycle" +
                    "\tMS2ScansPerCycle\tParentIonMass\tMonoisotopicMass\tPrecursorCharge\tMS1IsolationInterference");

                if (!rawData.isBoxCar)
                {
                    f.Write("\tParentPeakFound");
                }

                if (rawData.Performed.Contains(Operations.PeakArea) & !rawData.isBoxCar)
                {
                    f.Write("\tParentPeakArea");
                }

                if(!rawData.isBoxCar)
                {
                    f.Write("\tPeakFirstScan\tPeakMaxScan\tPeakLastScan\tBaseLinePeakWidth(s)\tPeakParentScanIntensity\tPeakMaxIntensity");
                }
                f.Write("\tMS1IonInjectionTime\tMS2IonInjectionTime" +
                    "\tMS3IonInjectionTime\tHCDEnergy\tMS1MedianIntensity\tMS2MedianIntensity\t");

                if(quantData != null)
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

                foreach (int scan in scans)
                {
                    int ms3scan, ms2scan, masterScan;

                    if (rawData.scanIndex.AnalysisOrder == MSOrderType.Ms3)
                    {
                        ms3scan = rawData.precursorScans[scan].MS3Scan;
                        ms2scan = rawData.precursorScans[scan].MS2Scan;
                        masterScan = rawData.precursorScans[scan].MasterScan;
                    }
                    else
                    {
                        ms3scan = -1;
                        ms2scan = rawData.precursorScans[scan].MS2Scan;
                        masterScan = rawData.precursorScans[scan].MasterScan;
                    }

                    f.Write(ms3scan.ToString() + "\t" + ms2scan.ToString() + "\t" + masterScan.ToString() + "\t");

                    f.Write(rawData.retentionTimes[scan].ToString() + "\t" + rawData.retentionTimes[masterScan].ToString() + "\t");
                    f.Write(metaData[masterScan].DutyCycle.ToString() + "\t" + metaData[masterScan].MS2ScansPerCycle.ToString() + "\t");
                    
                    f.Write(rawData.precursorMasses[ms2scan].ParentMZ.ToString() + "\t");
                    f.Write(rawData.precursorMasses[ms2scan].MonoisotopicMZ.ToString() + "\t");

                    f.Write(rawData.trailerExtras[ms2scan].ChargeState.ToString() + "\t");

                    f.Write(rawData.metaData[scan].Ms1IsolationInterference.ToString() + "\t");

                    if (!rawData.isBoxCar)
                    {
                        f.Write(rawData.peakData[ms2scan].PeakFound.ToString() + "\t");
                    }

                    if (rawData.Performed.Contains(Operations.PeakArea) & !rawData.isBoxCar)
                    {
                        f.Write(rawData.peakData[ms2scan].Area.ToString() + "\t");
                    }

                    if (!rawData.isBoxCar)
                    {
                        f.Write(rawData.peakData[ms2scan].FirstScan.ToString() + "\t");
                        f.Write(rawData.peakData[ms2scan].MaxScan.ToString() + "\t");
                        f.Write(rawData.peakData[ms2scan].LastScan.ToString() + "\t");
                        f.Write((rawData.peakData[ms2scan].BaselineWidth * 60).ToString() + "\t");
                        f.Write(rawData.peakData[ms2scan].ParentIntensity.ToString() + "\t");
                        f.Write(rawData.peakData[ms2scan].MaximumIntensity.ToString() + "\t");
                    }

                    f.Write(rawData.trailerExtras[masterScan].InjectionTime.ToString() + "\t");

                    if (rawData.scanIndex.AnalysisOrder == MSOrderType.Ms3)
                    {
                        f.Write(rawData.trailerExtras[ms2scan].InjectionTime.ToString() + "\t");
                        f.Write(rawData.trailerExtras[ms3scan].InjectionTime.ToString() + "\t");
                    }
                    else
                    {
                        f.Write(rawData.trailerExtras[ms2scan].InjectionTime.ToString() + "\t");
                        f.Write("-1\t");
                    }

                    f.Write(rawData.trailerExtras[scan].HCDEnergy + "\t");

                    f.Write(metaData[masterScan].IntensityDistribution.P50 + "\t");
                    f.Write(metaData[ms2scan].IntensityDistribution.P50 + "\t");

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

                    progress.Update();
                }
                progress.Done();
            }
        }
    }

    static class MGF
    {
        public static void WriteMGF(RawDataCollection rawData, IRawDataPlus rawFile, string outputDirectory, double cutoff = 0, int[] scans = null, double intensityCutoff = 0.01)
        {
            double intCutoff = 0;
            string fileName = ReadWrite.GetPathToFile(outputDirectory, rawData.rawFileName, ".mgf");

            MassAnalyzerType ms2MassAnalyzer = rawData.methodData.MassAnalyzers[MSOrderType.Ms2];

            List<Operations> operations = new List<Operations> { Operations.ScanIndex, Operations.MethodData, Operations.TrailerExtras, Operations.RetentionTimes , Operations.PrecursorMasses};

            if (ms2MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
            {
                operations.Add(Operations.Ms2CentroidStreams);
            }
            else
            {
                operations.Add(Operations.Ms2SegmentedScans);
            }

            CheckIfDone.Check(rawData, rawFile, operations);

            const int BufferSize = 65536;  // 64 Kilobytes

            using (StreamWriter f = new StreamWriter(fileName, false, Encoding.UTF8, BufferSize)) //Open a new file, the MGF file
            {
                // if the scans argument is null, use all scans
                if (scans == null)
                {
                    scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2];
                }
                
                ProgressIndicator progress = new ProgressIndicator(scans.Count(), String.Format("Writing MGF file"));

                foreach (int i in scans)
                {
                    f.WriteLine("\nBEGIN IONS");
                    f.WriteLine("RAWFILE={0}", rawData.rawFileName);
                    f.WriteLine("TITLE=Spectrum_{0}", i);
                    f.WriteLine("SCAN={0}", i);
                    f.WriteLine("RTINSECONDS={0}", rawData.retentionTimes[i]);

                    if (rawData.precursorMasses[i].MonoisotopicMZ != 0)
                    {
                        f.WriteLine("PEPMASS={0}", rawData.precursorMasses[i].MonoisotopicMZ);
                    }
                    else
                    {
                        f.WriteLine("PEPMASS={0}", rawData.precursorMasses[i].ParentMZ);
                    }

                    f.WriteLine("CHARGE={0}", rawData.trailerExtras[i].ChargeState);
                    
                    if (ms2MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        CentroidStreamData centroid = rawData.centroidStreams[i];

                        if (centroid.Intensities.Length > 0)
                        {
                            intCutoff = centroid.Intensities.Max() * intensityCutoff;
                        }
                        else
                        {
                            intCutoff = 0;
                        }

                        for (int j = 0; j < centroid.Masses.Length; j++)
                        {
                            //f.WriteLine(Math.Round(centroid.Masses[j], 4).ToString() + " " + Math.Round(centroid.Intensities[j], 4).ToString());
                            if (centroid.Masses[j] > cutoff & centroid.Intensities[j] > intCutoff)
                            {
                                f.WriteLine("{0} {1}", Math.Round(centroid.Masses[j], 5), Math.Round(centroid.Intensities[j], 4));
                            }
                        }
                    }
                    else
                    {
                        SegmentedScanData segments = rawData.segmentedScans[i];

                        if (segments.Intensities.Length > 0)
                        {
                            intCutoff = segments.Intensities.Max() * intensityCutoff;
                        }
                        else
                        {
                            intCutoff = 0;
                        }

                        for (int j = 0; j < segments.Positions.Length; j++)
                        {
                            if (segments.Positions[j] > cutoff & segments.Intensities[j] > intCutoff)
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
            Utilities.ConsoleUtils.ClearLastLine();
        }
    }

    static class Metrics
    {
        public static void WriteMatrix(RawDataCollection rawData, MetricsData metrics, string outputDirectory = null)
        {
            string fileName = ReadWrite.GetPathToFile(outputDirectory, rawData.rawFileName, "_Metrics.txt");

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
                if (!rawData.isBoxCar)
                {
                    f.WriteLine("MedianPeakWidthAt10Percent(s):\t" + Math.Round(metrics.MedianBaselinePeakWidth * 60, 4));
                    f.WriteLine("MedianPeakWidthAtHalfMax(s):\t" + Math.Round(metrics.MedianHalfHeightPeakWidth * 60, 4));
                    f.WriteLine("MedianAsymmetryFactor:\t" + Math.Round(metrics.MedianAsymmetryFactor, 4));
                    f.WriteLine("ColumnCapacity:\t" + Math.Round(metrics.PeakCapacity, 4));
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
    }

    static class QC
    {
        public static void WriteQcToTable(this QcDataCollection qcData)
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
                    f.Write(qcData.QcData[key].DateAcquired + ",");
                    f.Write(qcData.QcData[key].RawFile + ",");
                    f.Write(qcData.QcData[key].Instrument + ",");

                    f.Write(qcData.QcData[key].ExperimentMsOrder + ",");
                    f.Write(qcData.QcData[key].Ms1Analyzer + ",");
                    f.Write(qcData.QcData[key].Ms2Analyzer + ",");
                    f.Write(qcData.QcData[key].Ms3Analyzer + ",");

                    f.Write(qcData.QcData[key].GradientTime + ",");
                    f.Write(qcData.QcData[key].TotalScans + ",");
                    f.Write(qcData.QcData[key].NumMs1Scans + ",");
                    f.Write(qcData.QcData[key].NumMs2Scans + ",");
                    f.Write(qcData.QcData[key].NumMs3Scans + ",");

                    f.Write(qcData.QcData[key].Ms1ScanRate / 60 + ",");
                    f.Write(qcData.QcData[key].Ms2ScanRate / 60 + ",");
                    f.Write(qcData.QcData[key].Ms3ScanRate / 60 + ",");
                    f.Write(qcData.QcData[key].MeanDutyCycle + ",");
                    f.Write(qcData.QcData[key].MeanTopN + ",");

                    f.Write(qcData.QcData[key].MedianSummedMs1Intensity + ",");
                    f.Write(qcData.QcData[key].MedianSummedMs2Intensity + ",");
                    f.Write(qcData.QcData[key].MedianPrecursorIntensity + ",");
                    f.Write(qcData.QcData[key].MedianMs1IsolationInterference + ",");
                    f.Write(qcData.QcData[key].MedianMs2FractionConsumingTop80PercentTotalIntensity + ",");

                    f.Write(qcData.QcData[key].NumEsiStabilityFlags + ",");
                    f.Write(qcData.QcData[key].MedianMassDrift + ",");

                    //f.Write(qcData.QcData[key].QuantMeta.medianReporterIntensity + ",");

                    f.Write(qcData.QcData[key].IdentificationRate + ",");
                    f.Write(qcData.QcData[key].DigestionEfficiency + ",");
                    f.Write(qcData.QcData[key].MissedCleavageRate + ",");
                    
                    f.Write(qcData.QcData[key].LabelingEfficiencyAtNTerm + ",");
                    f.Write(qcData.QcData[key].LabelingEfficiencyAtK + ",");
                    f.Write(qcData.QcData[key].LabelingEfficiencyAtX + ",");

                    f.Write(qcData.QcData[key].Ms1FillTime.P50 + ",");
                    f.Write(qcData.QcData[key].Ms2FillTime.P50 + ",");
                    f.Write(qcData.QcData[key].Ms3FillTime.P50 + ",");

                    f.Write(qcData.QcData[key].PeakShape.Width.P10 * 60 + ",");
                    f.Write(qcData.QcData[key].PeakShape.Width.P50 * 60 + ",");
                    f.Write(qcData.QcData[key].PeakShape.Asymmetry.P10 + ",");
                    f.Write(qcData.QcData[key].PeakShape.Asymmetry.P50 + ",");
                    f.Write(qcData.QcData[key].ColumnPeakCapacity + ",");
                    f.Write(qcData.QcData[key].TimeBeforeFirstScanToExceedPoint1MaxIntensity + ",");
                    f.Write(qcData.QcData[key].TimeAfterLastScanToExceedPoint1MaxIntensity + ",");
                    f.Write(qcData.QcData[key].FractionOfRunAbovePoint1MaxIntensity + ",");

                    f.Write(qcData.QcData[key].ChargeRatio3to2 + ",");
                    f.Write(qcData.QcData[key].ChargeRatio4to2 + ",");

                    f.Write(qcData.QcData[key].IdentipyParameters + "\n");
                }
            }
            Console.WriteLine("Finished writing QC data to csv\n");
        }
    }

    static class Chromatogram
    {
        public static void WriteChromatogram(this RawDataCollection rawData, IRawDataPlus rawFile, MSOrderType order, bool TIC, bool BP, string outputDirectory)
        {
            List<Operations> operations = new List<Operations>() { Operations.RetentionTimes};
            //MSOrderType order = (MSOrderType)msOrder;
            MassAnalyzerType analyzer = rawData.methodData.MassAnalyzers[order];

            if (analyzer == MassAnalyzerType.MassAnalyzerFTMS)
            {
                if (order == MSOrderType.Ms)
                {
                    operations.Add(Operations.Ms1CentroidStreams);
                }
                if (order == MSOrderType.Ms2)
                {
                    operations.Add(Operations.Ms2CentroidStreams);
                }
                if (order == MSOrderType.Ms3)
                {
                    operations.Add(Operations.Ms3CentroidStreams);
                }
            }
            else
            {
                if (order == MSOrderType.Ms)
                {
                    operations.Add(Operations.Ms1SegmentedScans);
                }
                if (order == MSOrderType.Ms2)
                {
                    operations.Add(Operations.Ms2SegmentedScans);
                }
                if (order == MSOrderType.Ms3)
                {
                    operations.Add(Operations.Ms3SegmentedScans);
                }
            }

            CheckIfDone.Check(rawData, rawFile, operations);

            int[] scans = rawData.scanIndex.ScanEnumerators[order];

            if (TIC)
            {
                ProgressIndicator progress = new ProgressIndicator(scans.Length, String.Format("Writing {0} TIC chromatogram", order));
                progress.Start();
                string fileName = ReadWrite.GetPathToFile(outputDirectory, rawData.rawFileName, "_" + order + "_TIC_chromatogram.txt");

                using (StreamWriter f = new StreamWriter(fileName))
                {
                    f.WriteLine("RetentionTime\tIntensity");

                    if (analyzer == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        foreach (int scan in scans)
                        {
                            if (rawData.centroidStreams[scan].Intensities.Length > 0)
                            {
                                f.WriteLine("{0}\t{1}", rawData.retentionTimes[scan], rawData.centroidStreams[scan].Intensities.Sum());
                            }
                            else
                            {
                                f.WriteLine("{0}\t{1}", rawData.retentionTimes[scan], 0);
                            }
                            progress.Update();
                        }
                    }
                    else
                    {
                        foreach (int scan in scans)
                        {
                            if (rawData.segmentedScans[scan].Intensities.Length > 0)
                            {
                                f.WriteLine("{0}\t{1}", rawData.retentionTimes[scan], rawData.segmentedScans[scan].Intensities.Sum());
                            }
                            else
                            {
                                f.WriteLine("{0}\t{1}", rawData.retentionTimes[scan], 0);
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

                string fileName = ReadWrite.GetPathToFile(outputDirectory, rawData.rawFileName, "_" + order + "_BP_chromatogram.txt");

                using (StreamWriter f = new StreamWriter(fileName))
                {
                    f.WriteLine("RetentionTime\tIntensity");

                    if (analyzer == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        foreach (int scan in scans)
                        {
                            if (rawData.centroidStreams[scan].Intensities.Length > 0)
                            {
                                f.WriteLine("{0}\t{1}", rawData.retentionTimes[scan], rawData.centroidStreams[scan].Intensities.Max());
                            }
                            else
                            {
                                f.WriteLine("{0}\t{1}", rawData.retentionTimes[scan], 0);
                            }
                            progress.Update();
                        }
                    }
                    else
                    {
                        foreach (int scan in scans)
                        {
                            if (rawData.segmentedScans[scan].Intensities.Length > 0)
                            {
                                f.WriteLine("{0}\t{1}", rawData.retentionTimes[scan], rawData.segmentedScans[scan].Intensities.Max());
                            }
                            else
                            {
                                f.WriteLine("{0}\t{1}", rawData.retentionTimes[scan], 0);
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
