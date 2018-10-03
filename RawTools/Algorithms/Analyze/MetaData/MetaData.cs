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
using RawTools.Algorithms;

namespace RawTools.Algorithms.Analyze
{
    static class MetaData
    {



        public static void AggregateMetaData(CentroidStreamCollection centroidStreams, SegmentScanCollection segmentScans, MethodDataContainer methodData,
            PrecursorScanCollection precursorScans, TrailerExtraCollection trailerExtras, PrecursorMassCollection precursorMasses,
            RetentionTimeCollection retentionTimes, ScanDependentsCollections scanDependents, ScanIndex index)
        {
            ProgressIndicator progress = new ProgressIndicator(index.ScanEnumerators[MSOrderType.Any].Count(),
                "Formatting scan meta data");

            // add a new ScanMetaData class for each scan
            foreach (int scan in index.ScanEnumerators[MSOrderType.Any])
            {
                metaData.Add(scan, new ScanMetaDataDDA());
            }

            // get isolation window
            double isoWindow;
            if (methodData.AnalysisOrder == MSOrderType.Ms2)
            {
                isoWindow = methodData.IsolationWindow.MS2;
            }
            else
            {
                isoWindow = methodData.IsolationWindow.MS3.MS1Window;
            }

            // get topN
            foreach (int scan in index.ScanEnumerators[MSOrderType.Ms])
            {
                // if the ms1 scan has no scan dependents then topN = 0
                if (scanDependents[scan] == null)
                {
                    metaData[scan].MS2ScansPerCycle = 0;
                }
                else
                {
                    metaData[scan].MS2ScansPerCycle = scanDependents[scan].ScanDependentDetailArray.Length;
                }
            }

            // go through scans for each ms order sequentially
            foreach (MSOrderType MSOrder in new List<MSOrderType> { MSOrderType.Ms, MSOrderType.Ms2, MSOrderType.Ms3 })
            {
                int[] scans = index.ScanEnumerators[MSOrder];
                for (int i = 0; i < scans.Length; i++)
                {
                    metaData[scans[i]].FillTime = trailerExtras[scans[i]].InjectionTime;

                    // populate duty cycle
                    if (i < scans.Length - 1)
                    {
                        metaData[scans[i]].DutyCycle = (retentionTimes[scans[i + 1]] - retentionTimes[scans[i]]) * 60;
                    }
                    else
                    {
                        metaData[scans[i]].DutyCycle = 0;
                    }

                    // populate scan rate
                    if ((MSOrder == MSOrderType.Ms2 | MSOrder == MSOrderType.Ms3) & rawData.ExpType == ExperimentType.DDA)
                    {
                        metaData[scans[i]].MS2ScansPerCycle = metaData[precursorScans[scans[i]].MasterScan].MS2ScansPerCycle;
                    }

                    // populate intensity distributions
                    if (methodData.MassAnalyzers[MSOrder] == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        metaData[scans[i]].IntensityDistribution = new Distribution(centroidStreams[scans[i]].Intensities);
                        metaData[scans[i]].SummedIntensity = centroidStreams[scans[i]].Intensities.Sum();
                    }
                    else
                    {
                        metaData[scans[i]].IntensityDistribution = new Distribution(segmentScans[scans[i]].Intensities);
                        metaData[scans[i]].SummedIntensity = segmentScans[scans[i]].Intensities.Sum();
                    }

                    // populate fraction of scans consuming 80% of total intensity

                    if (methodData.MassAnalyzers[MSOrder] == MassAnalyzerType.MassAnalyzerFTMS)
                    {
                        metaData[scans[i]].FractionConsumingTop80PercentTotalIntensity = centroidStreams[scans[i]].Intensities.FractionOfScansConsumingTotalIntensity(percent: 80);
                    }
                    else
                    {
                        metaData[scans[i]].FractionConsumingTop80PercentTotalIntensity = segmentScans[scans[i]].Intensities.FractionOfScansConsumingTotalIntensity(percent: 80);
                    }

                    // calculate ms1 isolation interference
                    if (methodData.AnalysisOrder == MSOrder & rawData.ExpType == ExperimentType.DDA)
                    {
                        int preScan = precursorScans[scans[i]].MasterScan;
                        metaData[scans[i]].Ms1IsolationInterference = Ms1Interference.CalculateForOneScan(centroidStreams[preScan],
                            precursorMasses[scans[i]].MonoisotopicMZ, isoWindow, trailerExtras[scans[i]].ChargeState);
                    }

                    progress.Update();
                }
            }
            progress.Done();



            rawData.Performed.Add(Operations.MetaData);
        }

        public static void GetMetricsData(this MetricsData metricsData, ScanMetaDataCollectionDDA metaData, RawDataCollection rawData, IRawDataPlus rawFile, QuantDataCollection quantData = null)
        {
            List<Operations> operations = new List<Operations> { Operations.ScanIndex, Operations.RetentionTimes, Operations.MethodData, Operations.MetaData };
            if (!rawData.isBoxCar)
            {
                operations.Add(Operations.PeakRetAndInt);
                operations.Add(Operations.PeakShape);
            }

            rawData.Check(rawFile, operations);

            metricsData.RawFileName = rawData.rawFileName;
            metricsData.Instrument = rawData.instrument;
            metricsData.MS1Analyzer = methodData.MassAnalyzers[MSOrderType.Ms];
            metricsData.MS2Analyzer = methodData.MassAnalyzers[MSOrderType.Ms2];

            metricsData.TotalAnalysisTime = retentionTimes[index.ScanEnumerators[MSOrderType.Any].Last()] -
                retentionTimes[index.ScanEnumerators[MSOrderType.Any].First()];

            metricsData.TotalScans = index.allScans.Count();
            metricsData.MS1Scans = index.ScanEnumerators[MSOrderType.Ms].Length;
            metricsData.MS2Scans = index.ScanEnumerators[MSOrderType.Ms2].Length;

            if (methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                metricsData.MS3Analyzer = methodData.MassAnalyzers[MSOrderType.Ms3];
                metricsData.MS3Scans = index.ScanEnumerators[MSOrderType.Ms3].Length;
            }
            else
            {
                metricsData.MS3Analyzer = MassAnalyzerType.Any;
                metricsData.MS3Scans = 0;
            }

            metricsData.MSOrder = methodData.AnalysisOrder;

            List<double> ms2intensities = new List<double>();
            List<double> precursorIntensities = new List<double>();
            List<double> ms1fillTimes = new List<double>();
            List<double> ms2fillTimes = new List<double>();
            List<double> ms3fillTimes = new List<double>();
            List<double> ms2scansPerCycle = new List<double>();
            List<double> dutyCycles = new List<double>();
            List<double> fractionConsuming80 = new List<double>();

            foreach (int scan in index.ScanEnumerators[MSOrderType.Ms])
            {
                ms1fillTimes.Add(metaData[scan].FillTime);
                ms2scansPerCycle.Add(metaData[scan].MS2ScansPerCycle);
                dutyCycles.Add(metaData[scan].DutyCycle);
            }

            foreach (int scan in index.ScanEnumerators[MSOrderType.Ms2])
            {
                precursorIntensities.Add(rawData.peakData[scan].ParentIntensity);
                ms2intensities.Add(metaData[scan].SummedIntensity);
                ms2fillTimes.Add(metaData[scan].FillTime);
                fractionConsuming80.Add(metaData[scan].FractionConsumingTop80PercentTotalIntensity);
            }

            if (methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                foreach (int scan in index.ScanEnumerators[MSOrderType.Ms3])
                {
                    ms3fillTimes.Add(metaData[scan].FillTime);
                }
            }

            metricsData.MedianPrecursorIntensity = precursorIntensities.ToArray().Percentile(50);
            metricsData.MedianMs2FractionConsumingTop80PercentTotalIntensity = fractionConsuming80.ToArray().Percentile(50);

            metricsData.MedianSummedMS2Intensity = ms2intensities.ToArray().Percentile(50);
            metricsData.MedianMS1FillTime = ms1fillTimes.ToArray().Percentile(50);
            metricsData.MedianMS2FillTime = ms2fillTimes.ToArray().Percentile(50);

            if (methodData.AnalysisOrder == MSOrderType.Ms3)
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
                metricsData.Gradient = retentionTimes[index.allScans.Keys.Max()];
                metricsData.PeakCapacity = metricsData.Gradient / metricsData.MedianHalfHeightPeakWidth;

                metricsData.MedianAsymmetryFactor = rawData.peakData.PeakShapeMedians.Asymmetry.P10;
            }

            // add isolation interference
            metricsData.MedianMs1IsolationInterference = (from scan in index.ScanEnumerators[methodData.AnalysisOrder]
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
                foreach (int scan in index.ScanEnumerators[methodData.AnalysisOrder])
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
                    gradientStart = Convert.ToDouble(gradientStrings.ElementAt(i)[0].Replace(':', '.'));
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
}
