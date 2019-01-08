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
    static class MetaDataProcessingDIA
    {
        public static ScanMetaDataCollectionDIA AggregateMetaDataDIA(CentroidStreamCollection centroidStreams, SegmentScanCollection segmentScans, MethodDataContainer methodData,
            TrailerExtraCollection trailerExtras, RetentionTimeCollection retentionTimes, ScanIndex index)
        {
            //ProgressIndicator progress = new ProgressIndicator(index.ScanEnumerators[MSOrderType.Any].Count(),
             //   "Formatting scan meta data");

            ScanMetaDataCollectionDIA metaData = new ScanMetaDataCollectionDIA();

            int[] scans = index.ScanEnumerators[MSOrderType.Any];

            double isoWindow = MetaDataCalculations.Ms1IsoWindow(methodData);

            Console.WriteLine("Aggregating meta data");
            
            metaData.FillTime = MetaDataCalculations.FillTimes(trailerExtras, index);

            metaData.DutyCycle = MetaDataCalculations.DutyCycle(retentionTimes, index);

            metaData.IntensityDistribution = MetaDataCalculations.IntensityDistributions(centroidStreams, segmentScans, index);

            metaData.SummedIntensity = MetaDataCalculations.SummedIntensities(centroidStreams, segmentScans, index);

            metaData.FractionConsumingTop80PercentTotalIntensity = MetaDataCalculations.Top80Frac(centroidStreams, segmentScans, index);

            //Task.WaitAll();

            return metaData;
        }

        public static RawMetricsDataDIA GetMetricsDataDIA(ScanMetaDataCollectionDIA metaData, MethodDataContainer methodData,
            string rawFileName, RetentionTimeCollection retentionTimes, ScanIndex index)
        {
            RawMetricsDataDIA metricsData = new RawMetricsDataDIA();
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
            
            metricsData.MSOrder = methodData.AnalysisOrder;

            metricsData.MedianSummedMS1Intensity = MetricsCalculations.GetMedianSummedMSIntensity(metaData.SummedIntensity, index, MSOrderType.Ms);
            metricsData.MedianSummedMS2Intensity = MetricsCalculations.GetMedianSummedMSIntensity(metaData.SummedIntensity, index, MSOrderType.Ms2);

            metricsData.MedianMS1FillTime = MetricsCalculations.GetMedianMSFillTime(metaData.FillTime, index, MSOrderType.Ms);
            metricsData.MedianMS2FillTime = MetricsCalculations.GetMedianMSFillTime(metaData.FillTime, index, MSOrderType.Ms2);

            metricsData.MeanDutyCycle = MetricsCalculations.GetMedianDutyCycle(metaData.DutyCycle, index);
            
            metricsData.MedianMs2FractionConsumingTop80PercentTotalIntensity =
                MetricsCalculations.GetMedianMs2FractionConsumingTop80PercentTotalIntensity(
                    metaData.FractionConsumingTop80PercentTotalIntensity, index);
            

            metricsData.MS1ScanRate = metricsData.MS1Scans / metricsData.TotalAnalysisTime;
            metricsData.MS2ScanRate = metricsData.MS2Scans / metricsData.TotalAnalysisTime;
            
            (double timeBefore, double timeAfter, double fracAbove) = MetricsCalculations.ChromIntensityMetrics(metaData, retentionTimes, index);
            metricsData.TimeBeforeFirstScanToExceedPoint1MaxIntensity = timeBefore;
            metricsData.TimeAfterLastScanToExceedPoint1MaxIntensity = timeAfter;
            metricsData.FractionOfRunAbovePoint1MaxIntensity = fracAbove;

            metricsData.Ms1FillTimeDistribution = new Distribution((from x in index.ScanEnumerators[MSOrderType.Ms] select metaData.FillTime[x]).ToArray());
            metricsData.Ms2FillTimeDistribution = new Distribution((from x in index.ScanEnumerators[MSOrderType.Ms2] select metaData.FillTime[x]).ToArray());
            
            return metricsData;
        }
    }
}
