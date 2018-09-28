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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ThermoFisher;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;
using System.Collections;
using RawTools.Data.Containers;
using RawTools.Data.Collections;
using RawTools.Data.Extraction;
using RawTools.Utilities;

namespace RawTools.Data.Processing
{
    class QuantifyReporters
    {
        (string[] Labels, double[] Masses) reporters;
        public QuantData quantData = new QuantData();

        // first instance method is for FTMS data
        public QuantifyReporters(CentroidStreamData centroidStream, string labelingReagent)
        {
            reporters = new LabelingReagents().Reagents[labelingReagent];
            int index;
            double mass;
            double[] massDiff;

            for (int i = 0; i < reporters.Masses.Length; i++)
            {
                mass = reporters.Masses[i];
                int[] indices = centroidStream.Masses.FindAllIndex1((x) =>
                {
                    if (Math.Abs(x - mass) / mass * 1e6 < 10)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                if (indices.Length>1)
                {
                    massDiff = new double[indices.Length];
                    for (int ix = 0; ix < indices.Length; ix++)
                    {
                        massDiff[ix] = Math.Abs(centroidStream.Masses[indices[ix]] - mass);
                    }

                    index = -1;
                    for (int j = 0; j < indices.Length; j++)
                    {
                        if (massDiff[j] == massDiff.Min())
                        {
                            index = indices[j];
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    //index = Array.Find(indices, x => centroidStream.Masses[indices[x]] - mass == massDiff.Min());

                    if (index != -1)
                    {
                        quantData.Add(reporters.Labels[i], new ReporterIon(centroidStream.Masses[index], centroidStream.Intensities[index],
                                                                   centroidStream.Noises[index], centroidStream.Resolutions[index],
                                                                   centroidStream.Baselines[index]));
                    }
                    else
                    {
                        quantData.Add(reporters.Labels[i], new ReporterIon(0, 0, 0, 0, 0));
                    }
                    

                }
                else
                {
                    if (indices.Length == 1)
                    {
                        index = indices.Single();
                        quantData.Add(reporters.Labels[i], new ReporterIon(centroidStream.Masses[index], centroidStream.Intensities[index],
                                                                   centroidStream.Noises[index], centroidStream.Resolutions[index],
                                                                   centroidStream.Baselines[index]));
                    }
                    else
                    {
                        quantData.Add(reporters.Labels[i], new ReporterIon(0, 0, 0, 0, 0));
                    }
                    
                }

                
            }
        }

        // second instance method is for ITMS data
        public QuantifyReporters(SegmentedScanData segmentedScan, string labelingReagent)
        {
            reporters = new LabelingReagents().Reagents[labelingReagent];
            int index;

            for (int i = 0; i < reporters.Masses.Length; i++)
            {
                double mass = reporters.Masses[i];
                int[] indices = segmentedScan.Positions.FindAllIndex1((x) =>
                {
                    if (Math.Abs(x - mass) / mass * 1e6 < 10)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                if (indices.Length > 1)
                {
                    double[] massDiff = new double[indices.Length];
                    for (int ix = 0; ix < indices.Length; ix++)
                    {
                        massDiff[ix] = Math.Abs(segmentedScan.Positions[indices[ix]] - mass);
                    }

                    index = Array.Find(indices, (x) => { return massDiff[x] == massDiff.Min(); });

                    quantData.Add(reporters.Labels[i], new ReporterIon(segmentedScan.Positions[index], segmentedScan.Intensities[index]));

                }
                else
                {
                    if (indices.Length == 1)
                    {
                        index = indices[0];
                        quantData.Add(reporters.Labels[i], new ReporterIon(segmentedScan.Positions[index], segmentedScan.Intensities[index]));
                    }
                    else
                    {
                        quantData.Add(reporters.Labels[i], new ReporterIon(0, 0));
                    }

                }


            }
        }
    }

    static class Quantification
    {        
        public static void Quantify(this QuantDataCollection quantData, RawDataCollection rawData, IRawDataPlus rawFile, string labelingReagent)
        {
            MassAnalyzerType quantAnalyzer = rawData.methodData.QuantAnalyzer;

            if (quantAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
            {
                rawData.ExtractCentroidStreams(rawFile, rawData.methodData.AnalysisOrder);
            }
            else
            {
                rawData.ExtractSegmentScans(rawFile, rawData.methodData.AnalysisOrder);
            }

            int[] scans;

            ScanIndex scanIndex = rawData.scanIndex;
            Dictionary<int, CentroidStreamData> centroidScans = rawData.centroidStreams;
            Dictionary<int, SegmentedScanData> segmentScans = rawData.segmentedScans;

            scans = scanIndex.ScanEnumerators[scanIndex.AnalysisOrder];

            ProgressIndicator progress = new ProgressIndicator(scans.Length, "Quantifying reporter ions");

            quantData.LabelingReagents = labelingReagent;

            foreach (int scan in scans)
            {
                if (quantAnalyzer == MassAnalyzerType.MassAnalyzerFTMS)
                {
                    quantData.Add(scan, new QuantifyReporters(centroidScans[scan], labelingReagent).quantData);
                }
                else
                {
                    quantData.Add(scan, new QuantifyReporters(segmentScans[scan], labelingReagent).quantData);
                }

                progress.Update();
            }
            progress.Done();
            rawData.Performed.Add(Operations.Quantification);
        }
    }

    static class MetaDataProcessing
    {
        public static void AggregateMetaData(this ScanMetaDataCollection metaData, RawDataCollection rawData, IRawDataPlus rawFile)
        {
            List<Operations> operations = new List<Operations>() { Operations.ScanIndex, Operations.MethodData,
                Operations.RetentionTimes, Operations.TrailerExtras, Operations.PrecursorScans };

            if (rawData.methodData.MassAnalyzers[MSOrderType.Ms]==MassAnalyzerType.MassAnalyzerFTMS)
            {
                operations.Add(Operations.Ms1CentroidStreams);
            }
            else
            {
                operations.Add(Operations.Ms1SegmentedScans);
            }
            if (rawData.methodData.MassAnalyzers[MSOrderType.Ms2] == MassAnalyzerType.MassAnalyzerFTMS)
            {
                operations.Add(Operations.Ms2CentroidStreams);
            }
            else
            {
                operations.Add(Operations.Ms2SegmentedScans);
            }
            if (rawData.methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                if (rawData.methodData.MassAnalyzers[MSOrderType.Ms3] == MassAnalyzerType.MassAnalyzerFTMS)
                {
                    operations.Add(Operations.Ms3CentroidStreams);
                }
                else
                {
                    operations.Add(Operations.Ms3SegmentedScans);
                }
            }

            rawData.Check(rawFile, operations);

            ProgressIndicator progress = new ProgressIndicator(rawData.scanIndex.ScanEnumerators[MSOrderType.Any].Count(),
                "Formatting scan meta data");
            
            // add a new ScanMetaData class for each scan
            foreach (int scan in rawData.scanIndex.ScanEnumerators[MSOrderType.Any])
            {
                metaData.Add(scan, new ScanMetaData());
            }

            // get isolation window
            double isoWindow;
            if (rawData.methodData.AnalysisOrder == MSOrderType.Ms2)
            {
                isoWindow = rawData.methodData.IsolationWindow.MS2;
            }
            else
            {
                isoWindow = rawData.methodData.IsolationWindow.MS3.MS1Window;
            }

            // get topN
            foreach (int scan in rawData.scanIndex.ScanEnumerators[MSOrderType.Ms])
            {
                // if the ms1 scan has no scan dependents then topN = 0
                if (rawFile.GetScanDependents(scan, 0) == null)
                {
                    metaData[scan].MS2ScansPerCycle = 0;
                }
                else
                {
                    metaData[scan].MS2ScansPerCycle = rawFile.GetScanDependents(scan, 0).ScanDependentDetailArray.Length;
                }
            }

            // go through scans for each ms order sequentially
            foreach (MSOrderType MSOrder in new List<MSOrderType> { MSOrderType.Ms, MSOrderType.Ms2, MSOrderType.Ms3 })
            {
                int[] scans = rawData.scanIndex.ScanEnumerators[MSOrder];
                for (int i = 0; i < scans.Length; i++)
                {
                    metaData[scans[i]].FillTime = rawData.trailerExtras[scans[i]].InjectionTime;

                    // populate duty cycle
                    if (i < scans.Length - 1)
                    {
                        metaData[scans[i]].DutyCycle = (rawData.retentionTimes[scans[i + 1]] - rawData.retentionTimes[scans[i]])*60;
                    }
                    else
                    {
                        metaData[scans[i]].DutyCycle = 0;
                    }

                    // populate scan rate
                    if (MSOrder == MSOrderType.Ms2 | MSOrder == MSOrderType.Ms3)
                    {
                        metaData[scans[i]].MS2ScansPerCycle = metaData[rawData.precursorScans[scans[i]].MasterScan].MS2ScansPerCycle;
                    }

                    // populate intensity distributions
                    if (rawData.methodData.MassAnalyzers[MSOrder] == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        metaData[scans[i]].IntensityDistribution = new Distribution(rawData.centroidStreams[scans[i]].Intensities);
                        metaData[scans[i]].SummedIntensity = rawData.centroidStreams[scans[i]].Intensities.Sum();
                    }
                    else
                    {
                        metaData[scans[i]].IntensityDistribution = new Distribution(rawData.segmentedScans[scans[i]].Intensities);
                        metaData[scans[i]].SummedIntensity = rawData.segmentedScans[scans[i]].Intensities.Sum();
                    }

                    // populate fraction of scans consuming 80% of total intensity

                    if (rawData.methodData.MassAnalyzers[MSOrder] == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        metaData[scans[i]].FractionConsumingTop80PercentTotalIntensity = rawData.centroidStreams[scans[i]].Intensities.FractionOfScansConsumingTotalIntensity(percent: 80);
                    }
                    else
                    {
                        metaData[scans[i]].FractionConsumingTop80PercentTotalIntensity = rawData.segmentedScans[scans[i]].Intensities.FractionOfScansConsumingTotalIntensity(percent: 80);
                    }
                    
                    // calculate ms1 isolation interference
                    if (rawData.methodData.AnalysisOrder == MSOrder)
                    {
                        int preScan = rawData.precursorScans[scans[i]].MasterScan;
                        metaData[scans[i]].Ms1IsolationInterference = Ms1Interference.CalculateForOneScan(rawData.centroidStreams[preScan],
                            rawData.precursorMasses[scans[i]].MonoisotopicMZ, isoWindow, rawData.trailerExtras[scans[i]].ChargeState);
                    }
                    
                    progress.Update();
                }
            }
            progress.Done();

            

            rawData.Performed.Add(Operations.MetaData);
        }

        public static void GetMetricsData(this MetricsData metricsData, ScanMetaDataCollection metaData, RawDataCollection rawData, IRawDataPlus rawFile, QuantDataCollection quantData = null)
        {
            List<Operations> operations = new List<Operations> { Operations.ScanIndex, Operations.RetentionTimes, Operations.MethodData, Operations.MetaData};
            if (!rawData.isBoxCar)
            {
                operations.Add(Operations.PeakRetAndInt);
                operations.Add(Operations.PeakShape);
            }

            rawData.Check(rawFile, operations);

            metricsData.RawFileName = rawData.rawFileName;
            metricsData.Instrument = rawData.instrument;
            metricsData.MS1Analyzer = rawData.methodData.MassAnalyzers[MSOrderType.Ms];
            metricsData.MS2Analyzer = rawData.methodData.MassAnalyzers[MSOrderType.Ms2];

            metricsData.TotalAnalysisTime = rawData.retentionTimes[rawData.scanIndex.ScanEnumerators[MSOrderType.Any].Last()] -
                rawData.retentionTimes[rawData.scanIndex.ScanEnumerators[MSOrderType.Any].First()];

            metricsData.TotalScans = rawData.scanIndex.allScans.Count();
            metricsData.MS1Scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms].Length;
            metricsData.MS2Scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2].Length;

            if (rawData.methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                metricsData.MS3Analyzer = rawData.methodData.MassAnalyzers[MSOrderType.Ms3];
                metricsData.MS3Scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms3].Length;
            }
            else
            {
                metricsData.MS3Analyzer = MassAnalyzerType.Any;
                metricsData.MS3Scans = 0;
            }

            metricsData.MSOrder = rawData.methodData.AnalysisOrder;
            
            List<double> ms2intensities = new List<double>();
            List<double> precursorIntensities = new List<double>();
            List<double> ms1fillTimes = new List<double>();
            List<double> ms2fillTimes = new List<double>();
            List<double> ms3fillTimes = new List<double>();
            List<double> ms2scansPerCycle = new List<double>();
            List<double> dutyCycles = new List<double>();
            List<double> fractionConsuming80 = new List<double>();

            foreach (int scan in rawData.scanIndex.ScanEnumerators[MSOrderType.Ms])
            {
                ms1fillTimes.Add(metaData[scan].FillTime);
                ms2scansPerCycle.Add(metaData[scan].MS2ScansPerCycle);
                dutyCycles.Add(metaData[scan].DutyCycle);
            }

            foreach (int scan in rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2])
            {
                precursorIntensities.Add(rawData.peakData[scan].ParentIntensity);
                ms2intensities.Add(metaData[scan].SummedIntensity);
                ms2fillTimes.Add(metaData[scan].FillTime);
                fractionConsuming80.Add(metaData[scan].FractionConsumingTop80PercentTotalIntensity);
            }

            if (rawData.methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                foreach(int scan in rawData.scanIndex.ScanEnumerators[MSOrderType.Ms3])
                {
                    ms3fillTimes.Add(metaData[scan].FillTime);
                }
            }

            metricsData.MedianPrecursorIntensity = precursorIntensities.ToArray().Percentile(50);
            metricsData.MedianMs2FractionConsumingTop80PercentTotalIntensity = fractionConsuming80.ToArray().Percentile(50);

            metricsData.MedianSummedMS2Intensity = ms2intensities.ToArray().Percentile(50);
            metricsData.MedianMS1FillTime = ms1fillTimes.ToArray().Percentile(50);
            metricsData.MedianMS2FillTime = ms2fillTimes.ToArray().Percentile(50);

            if (rawData.methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                metricsData.MedianMS3FillTime = ms3fillTimes.ToArray().Percentile(50);
            }
            else
            {
                metricsData.MedianMS3FillTime = -1;
            }

            metricsData.MeanTopN = ms2scansPerCycle.Average();
            metricsData.MeanDutyCycle = dutyCycles.Average();
            metricsData.MS1ScanRate = metricsData.MS1Scans / metricsData.TotalAnalysisTime;
            metricsData.MS2ScanRate = metricsData.MS2Scans / metricsData.TotalAnalysisTime;
            metricsData.MS3ScanRate = metricsData.MS3Scans / metricsData.TotalAnalysisTime;

            // only do the following if it isn't a boxcar experiment
            if (!rawData.isBoxCar)
            {
                metricsData.MedianBaselinePeakWidth = rawData.peakData.PeakShapeMedians.Width.P10;
                metricsData.MedianHalfHeightPeakWidth = rawData.peakData.PeakShapeMedians.Width.P50;

                // we can't access the instrument method in Linux, so we will assume the gradient length is the length of the MS acquisition
                metricsData.Gradient = rawData.retentionTimes[rawData.scanIndex.allScans.Keys.Max()];
                metricsData.PeakCapacity = metricsData.Gradient / metricsData.MedianHalfHeightPeakWidth;

                metricsData.MedianAsymmetryFactor = rawData.peakData.PeakShapeMedians.Asymmetry.P10;
            }

            // add isolation interference
            metricsData.MedianMs1IsolationInterference = (from scan in rawData.scanIndex.ScanEnumerators[rawData.methodData.AnalysisOrder]
                                                          select rawData.metaData[scan].Ms1IsolationInterference).ToArray().Percentile(50);

            // now add the quant meta data, if quant was performed
            double medianReporterIntensity = 0;
            QuantMetaData quantMetaData = new QuantMetaData();
            SerializableDictionary<string, double> medianReporterIntensityByChannel = new SerializableDictionary<string, double>();
            if (quantData != null & rawData.Performed.Contains(Operations.Quantification))
            {
                string reagent = quantData.LabelingReagents;
                string[] allTags = new LabelingReagents().Reagents[reagent].Labels;
                List<double> allChannels = new List<double>();
                Dictionary<string, List<double>> byChannel = new Dictionary<string, List<double>>();
                foreach (string tag in allTags)
                {
                    byChannel.Add(tag, new List<double>());
                }
                foreach (int scan in rawData.scanIndex.ScanEnumerators[rawData.methodData.AnalysisOrder])
                {
                    foreach (string tag in allTags)
                    {
                        byChannel[tag].Add(quantData[scan][tag].Intensity);
                        allChannels.Add(quantData[scan][tag].Intensity);
                    }

                }
                medianReporterIntensity = allChannels.ToArray().Percentile(50);

                foreach (string tag in allTags)
                {
                    medianReporterIntensityByChannel[tag] = byChannel[tag].ToArray().Percentile(50);
                }

                quantMetaData.medianReporterIntensity = medianReporterIntensity;
                quantMetaData.medianReporterIntensityByChannel = medianReporterIntensityByChannel;
                quantMetaData.quantTags = allTags;
                metricsData.QuantMeta = quantMetaData;
                metricsData.IncludesQuant = true;
            }
        }

        /* The instrument method is inaccesible on Linux and Mac, so we won't use this method to get the gradient time. Instead we will
         * approximate that it is the retention time of the last scan, which should usually be a safe assumption. */
        public static double GradientTime(string LcMethod)
        {
            Regex rx = new Regex(@"(?:Mixture\s+\[%\S+\]\s+)([\S\s]+)(?:\n\nPre)");
            Match gradientMatch = rx.Match(LcMethod);
            string gradient = gradientMatch.Groups[1].Value;
            string[] lines = gradient.ToString().Split('\n');
            List<string[]> gradientStrings = new List<string[]>();

            foreach (var line in lines)
            {
                gradientStrings.Add(line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }

            double gradientStart = 0;
            double gradientEnd = 0;

            for (int i = 0; i < gradientStrings.Count() - 1; i++)
            {
                if (gradientStrings.ElementAt(i + 1)[3] != gradientStrings.ElementAt(i)[3])
                {
                    gradientStart = Convert.ToDouble(gradientStrings.ElementAt(i)[0].Replace(':','.'));
                    break;
                }
            }
            for (int i = gradientStrings.Count() - 1; i >= 0; i--)
            {
                if (gradientStrings.ElementAt(i - 1)[3] == gradientStrings.ElementAt(i)[3])
                {
                    gradientEnd = Convert.ToDouble(gradientStrings.ElementAt(i - 1)[0].Replace(':', '.'));
                    break;
                }
            }
            return gradientEnd - gradientStart;
        }
    }

    static class AnalyzePeaks
    {
        public static PrecursorPeakData OnePeak(RawDataCollection rawData, double monoIsoMass, int parentScan, int ddScan)
        {
            PrecursorPeakData peak = new PrecursorPeakData();

            int firstScan = parentScan,
                lastScan = parentScan,
                maxScan = parentScan,
                currentScan = parentScan,
                previousMS1scan, nextMS1scan;

            bool containsFirstMS1Scan = false,
                containsLastMS1Scan = false;

            int[] MS1Scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms];

            double minMassDiff, maxIntensity, parentIntensity;

            List<int> scans = new List<int>();
            List<double> profileTimes = new List<double>();
            List<double> profileIntensities = new List<double>();

            double[] masses, intensities, massDiff;

            Dictionary<int, double> indexedIntensities = new Dictionary<int, double>();

            // first take care of the parent scan data. In QE data sometimes the parent mass is missing from the parent spectrum, so we need to deal with that.

            masses = rawData.centroidStreams[currentScan].Masses;//.Where(i => (i > parentMass - 1 & i < parentMass + 1)).ToArray();
            //masses = (from mass in rawData.centroidStreams[currentScan].Masses where mass > parentMass - 1 & mass < parentMass + 1 select mass).ToArray();
            //masses = masses.Where(i => (i > parentMass - 1 & i < parentMass + 1)).ToArray();

            if (masses.Length == 0)
            {
                peak.PeakFound = false;
                return peak;
            }

            massDiff = new double[masses.Length];

            for (int i = 0; i < masses.Length; i++)
            {
                massDiff[i] = Math.Abs(masses[i] - monoIsoMass);
            }

            minMassDiff = massDiff.Min();

            if (minMassDiff / monoIsoMass * 1e6 < 4)
            {
                peak.PeakFound = true;
            }
            else
            {
                peak.PeakFound = false;
                return peak;
            }
            
            int scanIndex = Array.IndexOf(MS1Scans, parentScan);

            // now find the first ms1 scan of the peak, just follow the mass (within tolerance) accross scans until it goes to baseline
            while (true)
            {
                currentScan = MS1Scans[scanIndex];
                masses = rawData.centroidStreams[currentScan].Masses;
                intensities = rawData.centroidStreams[currentScan].Intensities;

                massDiff = new double[masses.Length];

                for (int i = 0; i < masses.Length; i++)
                {
                    massDiff[i] = Math.Abs(masses[i] - monoIsoMass);
                }

                minMassDiff = massDiff.Min();

                if (minMassDiff / monoIsoMass * 1e6 < 4)
                {
                    scans.Add(currentScan);
                    scanIndex -= 1;
                    indexedIntensities.Add(currentScan, intensities[Array.IndexOf(massDiff, minMassDiff)]);
                    if (scanIndex<0)
                    {
                        previousMS1scan = currentScan;
                        break;
                    }
                }
                else
                {
                    if(scanIndex == 0)
                    {
                        previousMS1scan = currentScan;
                    }
                    else
                    {
                        previousMS1scan = MS1Scans[scanIndex - 1];
                    }                    
                    break;
                }
            }

            // now find the last ms1 scan of the peak
            scanIndex = Array.IndexOf(MS1Scans, parentScan) + 1; // reset the ms1 scan indexer, add 1 so we don't replicate the parent scan
            
            while (true)
            {
                // Check to make sure the ms1 scan isn't the last one....
                if (scanIndex >= MS1Scans.Length)
                {
                    nextMS1scan = currentScan;
                    break;
                }

                currentScan = MS1Scans[scanIndex];
                masses = rawData.centroidStreams[currentScan].Masses;
                intensities = rawData.centroidStreams[currentScan].Intensities;

                massDiff = new double[masses.Length];

                for (int i = 0; i < masses.Length; i++)
                {
                    massDiff[i] = Math.Abs(masses[i] - monoIsoMass);
                }

                minMassDiff = massDiff.Min();

                if (minMassDiff / monoIsoMass * 1e6 < 4)
                {
                    scans.Add(currentScan);
                    scanIndex += 1;
                    indexedIntensities.Add(currentScan, intensities[Array.IndexOf(massDiff, minMassDiff)]);
                    if (scanIndex >= MS1Scans.Length)
                    {
                        nextMS1scan = currentScan;
                        break;
                    }
                }
                else
                {
                    if (scanIndex == MS1Scans.Length - 1)
                    {
                        nextMS1scan = currentScan;
                    }
                    else
                    {
                        nextMS1scan = MS1Scans[scanIndex + 1];
                    }
                    break;
                }
            }
            // We need to add an index and intensity for the scans before and after the peak. Otherwise fitting and other calculations later will be a huge pain.
            // We make note of the peaks which contain the first or last MS1 scans. This edge cases will probably need special treatment.

            if (previousMS1scan != scans.Min())
            {
                scans.Add(previousMS1scan);
                indexedIntensities.Add(previousMS1scan, 0);
            }
            else
            {
                containsFirstMS1Scan = true;
            }
            if (nextMS1scan != scans.Max())
            {
                scans.Add(nextMS1scan);
                indexedIntensities.Add(nextMS1scan, 0);
            }
            else
            {
                containsLastMS1Scan = true;
            }

            scans.Sort();
            firstScan = scans.First();
            lastScan = scans.Last();


            // add the retention times and intensities
            
            foreach (int scan in scans)
            {
                profileTimes.Add(rawData.retentionTimes[scan]);
                profileIntensities.Add(indexedIntensities[scan]);
            }

            maxIntensity = profileIntensities.Max();
            parentIntensity = indexedIntensities[parentScan];

            maxScan = scans[profileIntensities.IndexOf(maxIntensity)];

            peak.FirstScan = firstScan;
            peak.LastScan = lastScan;
            peak.MaxScan = maxScan;
            //peak.PreviousScan = previousMS1scan;
            //peak.NextScan = nextMS1scan;
            peak.ParentScan = parentScan;
            peak.NScans = scans.Count();
            peak.Scans = scans.ToArray();
            peak.ContainsFirstMS1Scan = containsFirstMS1Scan;
            peak.ContainsLastMS1Scan = containsLastMS1Scan;

            peak.ParentIntensity = parentIntensity;
            peak.MaximumIntensity = maxIntensity;

            peak.MaximumRetTime = rawData.retentionTimes[maxScan];
            peak.ParentRetTime = rawData.retentionTimes[parentScan];

            peak.BaselineWidth = profileTimes.Last() - profileTimes.First();

            peak.Intensities = profileIntensities.ToArray();
            peak.RetTimes = profileTimes.ToArray();

            return peak;
        }

        public static void CalcPeakRetTimesAndInts(this RawDataCollection rawData, IRawDataPlus rawFile)
        {
            CheckIfDone.Check(rawData, rawFile, new List<Operations> { Operations.ScanIndex, Operations.PrecursorMasses, Operations.PrecursorScans, Operations.Ms1CentroidStreams, Operations.RetentionTimes});

            if (rawData.Performed.Contains(Operations.PeakRetAndInt))
            {
                return;
            }

            int[] scans = rawData.scanIndex.ScanEnumerators[MSOrderType.Ms2];

            PrecursorPeakDataCollection peaks = new PrecursorPeakDataCollection();

            ProgressIndicator P = new ProgressIndicator(total: scans.Length, message: "Analyzing precursor peaks");
            
            foreach (int scan in scans)
            {
                peaks.Add(scan, OnePeak(rawData: rawData, monoIsoMass: rawData.precursorMasses[scan].MonoisotopicMZ, parentScan: rawData.precursorScans[scan].MasterScan, ddScan: scan));
                P.Update();
            }
            P.Done();

            rawData.peakData = peaks;
            rawData.Performed.Add(Operations.PeakRetAndInt);
            rawData.Performed.RemoveWhere(x => x == Operations.PeakArea);
        }

        public static double InterpolateLinear((double x, double y) point0, (double x, double y) point1, double y)
        {
            return point0.x + (y - point0.y) * (point1.x - point0.x) / (point1.y - point0.y);
        }

        public static Containers.PeakShape GetPeakShape(RawDataCollection rawData, int scan)
        {
            double[] Intensities, RetTimes;
            int maxIndex, currentIndex;
            Dictionary<int, double> a, b;
            Asymmetry asymmetry = new Asymmetry();
            Width width = new Width();
            a = new Dictionary<int, double>();
            b = new Dictionary<int, double>();

            Intensities = rawData.peakData[scan].Intensities;
            RetTimes = rawData.peakData[scan].RetTimes;
            maxIndex = Array.FindIndex(Intensities, x => { return x == Intensities.Max(); });


            foreach (int percent in new int[4] { 10, 25, 50, 75 })
            {
                currentIndex = maxIndex;
                // first we find the asymmetery parameter from the left side of the peak
                while (true)
                {
                    if (Intensities[currentIndex] < Intensities[maxIndex] * percent / 100)
                    {
                        a.Add(percent, RetTimes[maxIndex] - InterpolateLinear(point0: (RetTimes[currentIndex], Intensities[currentIndex]),
                            point1: (RetTimes[currentIndex + 1], Intensities[currentIndex + 1]), y: Intensities[maxIndex] * percent / 100));
                        break;
                    }
                    else
                    {
                        currentIndex -= 1;

                        /*
                         * Now the peak MUST contain intensities of 0 at the edges, so this case shouldn't come up anymore
                        if (currentIndex == -1)
                        {                            
                            double newTime = rawData.retentionTimes[rawData.peakData[scan].PreviousScan];
                            a.Add(percent, RetTimes[maxIndex] - InterpolateLinear(point0: (newTime, 0),
                                point1: (RetTimes.First(), Intensities.First()), y: Intensities[maxIndex] * percent / 100));
                            break;
                        }
                        */
                    }
                }

                currentIndex = maxIndex;
                // and now the right side of the peak
                while (true)
                {
                    if (Intensities[currentIndex] < Intensities[maxIndex] * percent / 100)
                    {
                        b.Add(percent, InterpolateLinear(point0: (RetTimes[currentIndex - 1], Intensities[currentIndex - 1]),
                            point1: (RetTimes[currentIndex], Intensities[currentIndex]), y: Intensities[maxIndex] * percent / 100) - RetTimes[maxIndex]);
                        break;
                    }
                    else
                    {
                        currentIndex += 1;
                        /*
                         * Now the peak MUST contain intensities of 0 at the edges, so this case shouldn't come up anymore
                        if (currentIndex == RetTimes.Count())
                        {
                            double newTime = rawData.retentionTimes[rawData.peakData[scan].NextScan];
                            b.Add(percent, InterpolateLinear(point0: (RetTimes.Last(), Intensities.Last()), point1: (newTime, 0),
                                y: Intensities[maxIndex] * percent / 100) - RetTimes[maxIndex]);
                            break;
                        }
                        */
                    }
                }
            }
            asymmetry.P10 = b[10] / a[10];
            asymmetry.P25 = b[25] / a[25];
            asymmetry.P50 = b[50] / a[50];
            asymmetry.P75 = b[75] / a[75];
            width.P10 = a[10] + b[10];
            width.P25 = a[25] + b[25];
            width.P50 = a[50] + b[50];
            width.P75 = a[75] + b[75];

            Containers.PeakShape peak = new Containers.PeakShape(asymmetry: asymmetry, width: width, peakMax: RetTimes[maxIndex]);

            return peak;
        }

        public static void CalculatePeakShapes(this RawDataCollection rawData, IRawDataPlus rawFile, int percentile = 90)
        {
            List<Operations> list = new List<Operations>() { Operations.PeakRetAndInt };
            CheckIfDone.Check(rawData, rawFile, list);

            // in this first step we chose scans which are in the ith percentile
            List<double> Intensities = (from x in rawData.peakData.Keys.ToArray() select rawData.peakData[x].MaximumIntensity).ToList();
            Intensities.Sort();
            Intensities.Reverse();
            Intensities = Intensities.GetRange(0, Intensities.Count() / (100 - percentile));

            int[] scans = (from x in rawData.peakData.Keys.ToArray() where Intensities.Contains(rawData.peakData[x].MaximumIntensity) select x).ToArray();

            DistributionMultiple allPeaksAsymmetry = new DistributionMultiple();
            DistributionMultiple allPeaksWidths = new DistributionMultiple();
            ProgressIndicator P = new ProgressIndicator(scans.Length, "Calculating peak symmetries");
            
            for (int i = 0; i < scans.Count(); i++)
            {
                if (rawData.peakData[scans[i]].NScans < 5 | rawData.peakData[scans[i]].PeakFound == false |
                    rawData.peakData[scans[i]].ContainsFirstMS1Scan | rawData.peakData[scans[i]].ContainsLastMS1Scan)
                {
                    rawData.peakData[scans[i]].PeakShape = null;
                    P.Update();
                    continue;
                }
                rawData.peakData[scans[i]].PeakShape = GetPeakShape(rawData, scans[i]);

                allPeaksAsymmetry.Add(rawData.peakData[scans[i]].PeakShape.Asymmetry);
                allPeaksWidths.Add(rawData.peakData[scans[i]].PeakShape.Width);
                P.Update();
            }
            P.Done();

            rawData.Performed.Add(Operations.PeakShape);
            rawData.peakData.PeakShapeMedians = new Containers.PeakShape(width: allPeaksWidths.GetMedians(), asymmetry: allPeaksAsymmetry.GetMedians(), peakMax: 0);
        }
        /*
        static double FindMaxRetTime(PrecursorPeakData peakData, alglib.spline1dinterpolant splineInterpolant)
        {
            double[] rets = new double[10];
            double[] ints = new double[10];
            double firstRet = peakData.RetTimes.First();
            double lastRet = peakData.RetTimes.Last();
            int maxIndex = 0;

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    rets[i] = firstRet + (lastRet - firstRet) * (i / 9);
                    ints[i] = alglib.spline1dcalc(splineInterpolant, rets[i]);
                }

                maxIndex = Array.FindIndex(ints, x => x == ints.Max());
                if (maxIndex == 0)
                {
                    return firstRet;
                }
                
                firstRet = rets[maxIndex - 1];
                lastRet = rets[maxIndex + 1];
            }

            return rets[maxIndex];
        }
        
        static (double a, double b) FindShoulderRetTimes(PrecursorPeakData peakData, double centerRetTime, alglib.spline1dinterpolant splineInterpolant, int percentHeight)
        {
            double[] rets = new double[10];
            double[] ints = new double[10];
            double[] diffs = new double[10];
            double firstRet = peakData.RetTimes.First();
            double lastRet = peakData.RetTimes.Last();
            int leftIndex = 0,
                rightIndex = 0;
            double a, b;
            double maxIntensity = alglib.spline1dcalc(splineInterpolant, centerRetTime);
            double findIntensity = maxIntensity * percentHeight / 100;

            // FindLeftShoulder
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    rets[i] = firstRet + (centerRetTime - firstRet) * (i / 9);
                    ints[i] = alglib.spline1dcalc(splineInterpolant, rets[i]);
                    diffs[i] = Math.Abs(ints[i] - findIntensity);
                }

                leftIndex = Array.FindIndex(diffs, x => x == diffs.Min());
                if (leftIndex == 0)
                {
                    break;
                }
                firstRet = rets[leftIndex - 1];
                lastRet = rets[leftIndex + 1];
            }
            a = rets[leftIndex];

            // FindRightShoulder
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    rets[i] = centerRetTime + (lastRet - centerRetTime) * (i / 9);
                    ints[i] = alglib.spline1dcalc(splineInterpolant, rets[i]);
                    diffs[i] = Math.Abs(ints[i] - findIntensity);
                }

                rightIndex = Array.FindIndex(diffs, x => x == diffs.Min());
                if (rightIndex == rets.Length - 1 | rightIndex == 0)
                {
                    break;
                }
                firstRet = rets[rightIndex - 1];
                lastRet = rets[rightIndex + 1];
            }
            b = rets[rightIndex];

            return (a : rets[leftIndex], b : rets[rightIndex]);
        }
        /*
        static alglib.spline1dinterpolant FitPeak(RawDataCollection rawData, int scan)
        {
            int info;
            double v;
            alglib.spline1dinterpolant s;
            alglib.spline1dfitreport rep;
            double rho = 2;
            alglib.spline1dfitpenalized(rawData.peakData[scan].RetTimes, rawData.peakData[scan].Intensities, 10, rho, out info, out s, out rep);

            return s;
        }
        
        public static (Distribution Asymmetry, Distribution Width, double PeakMaxRetTime) SplinePeakShape(RawDataCollection rawData, int scan)
        {
            alglib.spline1dinterpolant fit = FitPeak(rawData, scan);
            Dictionary<int, (double a, double b)> PeakShoulders = new Dictionary<int, (double a, double b)>();
            Distribution Asymmetry = new Distribution();
            Distribution Width = new Distribution();

            double PeakMaxRetTime = FindMaxRetTime(rawData.peakData[scan], fit);

            foreach (int percent in new List<int> { 16, 25, 50, 75, 84 })
            {
                PeakShoulders.Add(percent, FindShoulderRetTimes(rawData.peakData[scan], PeakMaxRetTime, fit, percent));
            }

            Asymmetry.P16 = PeakShoulders[16].b / PeakShoulders[16].a;
            Asymmetry.P25 = PeakShoulders[25].b / PeakShoulders[25].a;
            Asymmetry.P50 = PeakShoulders[50].b / PeakShoulders[50].a;
            Asymmetry.P75 = PeakShoulders[75].b / PeakShoulders[75].a;
            Asymmetry.P84 = PeakShoulders[84].b / PeakShoulders[84].a;

            Width.P16 = PeakShoulders[16].b + PeakShoulders[16].a;
            Width.P25 = PeakShoulders[25].b + PeakShoulders[25].a;
            Width.P50 = PeakShoulders[50].b + PeakShoulders[50].a;
            Width.P75 = PeakShoulders[75].b + PeakShoulders[75].a;
            Width.P84 = PeakShoulders[84].b + PeakShoulders[84].a;

            return (Asymmetry: Asymmetry, Width: Width, PeakMaxRetTime: PeakMaxRetTime);
        }

        public static void AddPeakShapesToPeakData(RawDataCollection rawData)
        {
            // Not implemented. This method is for using a spline fitting, which we are not doing right now.
            (Distribution Asymmetry, Distribution Width, double PeakMaxRetTime) peak;
            ProgressIndicator P = new ProgressIndicator(rawData.peakData.Keys.Count, "Analyzing peak shapes");
            Containers.PeakShape PeakShape = new Containers.PeakShape();

            foreach (int scan in rawData.peakData.Keys)
            {
                if (!rawData.peakData[scan].PeakFound | rawData.peakData[scan].NScans < 3)
                {
                    continue;
                }
                peak = SplinePeakShape(rawData, scan);

                PeakShape.Asymmetry = peak.Asymmetry;
                PeakShape.MaxRetTime = peak.PeakMaxRetTime;
                PeakShape.Width = peak.Width;

                rawData.peakData[scan].PeakShape = PeakShape;

                P.Update();
            }
            P.Done();
        }
        */
        public static double CalculatePeakArea(PrecursorPeakData peak)
        {
            double area = 0;

            for (int i = 1; i < peak.NScans; i++)
            {
                area += (peak.Intensities[i - 1] + peak.Intensities[i]) * (peak.RetTimes[i] - peak.RetTimes[i - 1]) / 2;
            }

            return area;
        }

        public static void QuantifyPrecursorPeaks(this RawDataCollection rawData, IRawDataPlus rawFile)
        {
            List<Operations> list = new List<Operations>() { Operations.PeakRetAndInt};
            CheckIfDone.Check(rawData, rawFile, list);
            int[] scans = rawData.peakData.Keys.ToArray();
            ProgressIndicator P = new ProgressIndicator(scans.Length, "Integrating precursor peaks");

            for (int i = 0; i < scans.Count(); i++)
            {
                if (rawData.peakData[scans[i]].PeakFound == false)
                {
                    P.Update();
                    continue;
                }

                rawData.peakData[scans[i]].Area = CalculatePeakArea(rawData.peakData[scans[i]]);
                P.Update();
            }
            P.Done();

            rawData.Performed.Add(Operations.PeakArea);
        }
        
    }

    static class Ms1Interference
    {
        static (double[], double[]) SubsetCentroidScan(this CentroidStreamData Ms1Scan, double parentMass, double isoWindow)
        {
            List<double> masses = new List<double>();
            List<double> intensities = new List<double>();

            double lower = parentMass - 0.5 * isoWindow;
            double upper = parentMass + 0.5 * isoWindow;

            for (int i = 0; i < Ms1Scan.Masses.Length; i++)
            {
                if (Ms1Scan.Masses[i] > lower & Ms1Scan.Masses[i] < upper)
                {
                    masses.Add(Ms1Scan.Masses[i]);
                    intensities.Add(Ms1Scan.Intensities[i]);
                }
            }

            return (masses.ToArray(), intensities.ToArray());
        }

        static bool withinTolerance(this List<double> isotopes, double ion, double ppmTolerance)
        {
            double[] ppm = new double[isotopes.Count()];

            for (int i = 0; i < ppm.Length; i++)
            {
                ppm[i] = Math.Abs(ion - isotopes[i]) / isotopes[i] * 1e6;
            }

            return ppm.Min() < ppmTolerance;
        }

        public static double CalculateForOneScan(CentroidStreamData Ms1Scan, double monoIsoMass, double isoWindow, int charge, double ppm = 4)
        {
            double[] masses;
            double[] intensities;
            double[] interferences;
            double currentIsotope;
            List<double> isotopes = new List<double>();

            // subset the ms1 scan to be the isolation window
            (masses, intensities) = SubsetCentroidScan(Ms1Scan, monoIsoMass, isoWindow);

            // make a copy of the intensities array to represent the interference intensities
            interferences = (double[])intensities.Clone();

            // make a list of the possible isotopes
            currentIsotope = monoIsoMass;
            while (currentIsotope < monoIsoMass + 0.5*isoWindow)
            {
                isotopes.Add(currentIsotope);
                currentIsotope += 1.003356 / charge;
            }

            for (int i = 0; i < interferences.Length; i++)
            {
                double ion = masses[i];

                if (isotopes.withinTolerance(ion, 10))
                {
                    interferences[i] = 0;
                }
            }

            return interferences.Sum() / intensities.Sum();
        }
    }
}
