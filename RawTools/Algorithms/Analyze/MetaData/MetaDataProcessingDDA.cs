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
using System.Threading;
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
using RawTools.WorkFlows;
using RawTools.Algorithms.Analyze.Metrics;

namespace RawTools.Algorithms.Analyze
{
    static class MetaDataProcessingDDA
    {
        public static ScanMetaDataCollectionDDA AggregateMetaDataDDA(CentroidStreamCollection centroidStreams, SegmentScanCollection segmentScans, MethodDataContainer methodData,
            PrecursorScanCollection precursorScans, TrailerExtraCollection trailerExtras, PrecursorMassCollection precursorMasses,
            RetentionTimeCollection retentionTimes, ScanDependentsCollections scanDependents, ScanEventReactionCollection reactions, ScanIndex index, int maxProcesses)
        {
            //ProgressIndicator progress = new ProgressIndicator(index.ScanEnumerators[MSOrderType.Any].Count(),
             //   "Formatting scan meta data");

            ScanMetaDataCollectionDDA metaData = new ScanMetaDataCollectionDDA();

            int[] scans = index.ScanEnumerators[MSOrderType.Any];

            double isoWindow = MetaDataCalculations.Ms1IsoWindow(methodData);

            Console.WriteLine("Calculating meta data");

            Console.WriteLine("  MS1 isolation interference");
            metaData.Ms1IsolationInterference = MetaDataCalculations.Ms1Interference(centroidStreams, precursorMasses, trailerExtras,
                precursorScans, reactions, index, maxProcesses);

            Console.WriteLine("  MS2 scan cycle density");
            metaData.MS2ScansPerCycle = MetaDataCalculations.MS2ScansPerCycle(scanDependents, index);

            Console.WriteLine("  Ion injection time");
            metaData.FillTime = MetaDataCalculations.FillTimes(trailerExtras, index);

            Console.WriteLine("  FAIMS voltages");
            metaData.FaimsVoltage = MetaDataCalculations.FaimsVoltages(trailerExtras, index);

            Console.WriteLine("  Duty cycle");
            metaData.DutyCycle = MetaDataCalculations.DutyCycle(retentionTimes, index);

            Console.WriteLine("  Intensity distribution");
            metaData.IntensityDistribution = MetaDataCalculations.IntensityDistributions(centroidStreams, segmentScans, index, maxProcesses);

            Console.WriteLine("  Summed intensities");
            metaData.SummedIntensity = MetaDataCalculations.SummedIntensities(centroidStreams, segmentScans, index, maxProcesses);

            metaData.FractionConsumingTop80PercentTotalIntensity = MetaDataCalculations.Top80Frac(centroidStreams, segmentScans, index, maxProcesses);

            //Task.WaitAll();

            return metaData;
        }

        public static RawMetricsDataDDA GetMetricsDataDDA(ScanMetaDataCollectionDDA metaData, MethodDataContainer methodData,
            string rawFileName, RetentionTimeCollection retentionTimes, ScanIndex index, PrecursorPeakCollection peakData,
            PrecursorScanCollection precursorScans, QuantDataCollection quantData = null)
        {
            RawMetricsDataDDA metricsData = new RawMetricsDataDDA();
            metricsData.DateAcquired = methodData.CreationDate;
            metricsData.Instrument = methodData.Instrument;
            Console.WriteLine("Calculating metrics");
            
            metricsData.RawFileName = rawFileName;
            metricsData.Instrument = methodData.Instrument;
            metricsData.MS1Analyzer = methodData.MassAnalyzers[MSOrderType.Ms];
            metricsData.MS2Analyzer = methodData.MassAnalyzers[MSOrderType.Ms2];
            metricsData.TotalAnalysisTime = retentionTimes[index.ScanEnumerators[MSOrderType.Any].Last()] -
                retentionTimes[index.ScanEnumerators[MSOrderType.Any].First()];
            
            metricsData.NumberOfEsiFlags = MetricsCalculations.NumberOfEsiFlags(metaData, index);

            metricsData.TotalScans = index.TotalScans;
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

            var pickedMs1 = new HashSet<int>((from x in index.ScanEnumerators[methodData.AnalysisOrder]
                                              select precursorScans[x].MasterScan)).ToList();
            
            metricsData.MSOrder = methodData.AnalysisOrder;

            metricsData.MedianSummedMS1Intensity = MetricsCalculations.GetMedianSummedMSIntensity(metaData.SummedIntensity, index, MSOrderType.Ms);
            metricsData.MedianSummedMS2Intensity = MetricsCalculations.GetMedianSummedMSIntensity(metaData.SummedIntensity, index, MSOrderType.Ms2);

            metricsData.MedianPrecursorIntensity = (from x in peakData.Keys select peakData[x].ParentIntensity).ToArray().Percentile(50);

            metricsData.MedianMS1FillTime = MetricsCalculations.GetMedianMSFillTime(metaData.FillTime, index, MSOrderType.Ms);
            metricsData.MedianMS2FillTime = MetricsCalculations.GetMedianMSFillTime(metaData.FillTime, index, MSOrderType.Ms2);

            if (methodData.AnalysisOrder == MSOrderType.Ms3)
            {
                metricsData.MedianMS3FillTime = MetricsCalculations.GetMedianMSFillTime(metaData.FillTime, index, MSOrderType.Ms3);
            }

            metricsData.FaimsVoltages = MetricsCalculations.GetFaimsVoltages(metaData.FaimsVoltage, index, MSOrderType.Ms);

            metricsData.MeanTopN = MetricsCalculations.GetMeanMs2ScansPerCycle(metaData.MS2ScansPerCycle);

            metricsData.MeanDutyCycle = MetricsCalculations.GetMedianDutyCycle(metaData.DutyCycle, index);
            
            metricsData.MedianMs2FractionConsumingTop80PercentTotalIntensity =
                MetricsCalculations.GetMedianMs2FractionConsumingTop80PercentTotalIntensity(
                    metaData.FractionConsumingTop80PercentTotalIntensity, index);


            metricsData.MS1ScanRate = metricsData.MS1Scans / metricsData.TotalAnalysisTime;
            metricsData.MS2ScanRate = metricsData.MS2Scans / metricsData.TotalAnalysisTime;
            if (methodData.AnalysisOrder == MSOrderType.Ms3)  metricsData.MS3ScanRate = metricsData.MS3Scans / metricsData.TotalAnalysisTime;

            metricsData.MedianBaselinePeakWidth = peakData.PeakShapeMedians.Width.P10;
            metricsData.MedianHalfHeightPeakWidth = peakData.PeakShapeMedians.Width.P50;

            // we can't access the instrument method in Linux, so we will assume the gradient length is the length of the MS acquisition
            metricsData.Gradient = retentionTimes[index.allScans.Keys.Max()];
            metricsData.PeakCapacity = metricsData.Gradient / metricsData.MedianHalfHeightPeakWidth;

            metricsData.MedianAsymmetryFactor = peakData.PeakShapeMedians.Asymmetry.P10;

            // add isolation interference
            metricsData.MedianMs1IsolationInterference = (from scan in index.ScanEnumerators[methodData.AnalysisOrder]
                                                          select metaData.Ms1IsolationInterference[scan]).ToArray().Percentile(50);

            (double timeBefore, double timeAfter, double fracAbove) = MetricsCalculations.ChromIntensityMetrics(metaData, retentionTimes, index);
            metricsData.TimeBeforeFirstScanToExceedPoint1MaxIntensity = timeBefore;
            metricsData.TimeAfterLastScanToExceedPoint1MaxIntensity = timeAfter;
            metricsData.FractionOfRunAbovePoint1MaxIntensity = fracAbove;

            metricsData.Ms1FillTimeDistribution = new Distribution((from x in index.ScanEnumerators[MSOrderType.Ms] select metaData.FillTime[x]).ToArray());
            metricsData.Ms2FillTimeDistribution = new Distribution((from x in index.ScanEnumerators[MSOrderType.Ms2] select metaData.FillTime[x]).ToArray());
            if (methodData.AnalysisOrder == MSOrderType.Ms3) metricsData.Ms3FillTimeDistribution = new Distribution((from x in index.ScanEnumerators[MSOrderType.Ms3] select metaData.FillTime[x]).ToArray());

            metricsData.PeakShape.Asymmetry.P10 = peakData.PeakShapeMedians.Asymmetry.P10;
            metricsData.PeakShape.Asymmetry.P50 = peakData.PeakShapeMedians.Asymmetry.P50;

            metricsData.PeakShape.Width.P10 = peakData.PeakShapeMedians.Width.P10;
            metricsData.PeakShape.Width.P50 = peakData.PeakShapeMedians.Width.P50;

            // now add the quant meta data, if quant was performed
            double medianReporterIntensity = 0;
            QuantMetaData quantMetaData = new QuantMetaData();
            SerializableDictionary<string, double> medianReporterIntensityByChannel = new SerializableDictionary<string, double>();
            if (quantData != null)
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
            return metricsData;
        }
    }
}
